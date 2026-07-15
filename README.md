# CitasApp

CitasApp administra pacientes, medicos y citas mediante una aplicacion MVC y una API REST. La solucion usa .NET 10, arquitectura hexagonal, Entity Framework Core, PostgreSQL y ASP.NET Core Identity.

## Arquitectura

- `CitasApp.Domain`: entidades y puertos de persistencia. No depende de EF Core, PostgreSQL, Identity, JWT ni ASP.NET Core.
- `CitasApp.Application`: casos de uso, validaciones compartidas, roles y reglas de agenda.
- `CitasApp.Infrastructure`: adaptadores PostgreSQL, JSON y memoria, `CitasAppDbContext`, Identity, JWT y migraciones.
- `CitasApp.Web`: adaptador MVC con autenticacion por cookie.
- `CitasApp.Api`: adaptador HTTP con autenticacion JWT Bearer.
- `CitasApp.Tests`: pruebas de integracion de autenticacion, autorizacion y persistencia.

PostgreSQL es el adaptador activo y se registra con alcance `Scoped`. Los adaptadores JSON y memoria se conservan como alternativas, pero no se registran simultaneamente para las mismas interfaces.

## Requisitos

- .NET SDK 10.
- PostgreSQL accesible desde los dos hosts.
- `dotnet-ef` 10 para administrar migraciones.
- Certificado HTTPS local confiable (`dotnet dev-certs https --trust`) para probar login y JWT.

Si `dotnet ef` no esta disponible:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.9
```

## Configurar PostgreSQL

Los `appsettings.json` versionados no contienen credenciales. Configure la misma base en ambos proyectos mediante User Secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:PostgreSql" "Host=localhost;Port=5432;Database=citasapp;Username=<USUARIO>;Password=<PASSWORD>" --project CitasApp.Web
dotnet user-secrets set "ConnectionStrings:PostgreSql" "Host=localhost;Port=5432;Database=citasapp;Username=<USUARIO>;Password=<PASSWORD>" --project CitasApp.Api
```

En servidores use la variable `ConnectionStrings__PostgreSql`. La contrasena PostgreSQL que estuvo versionada antes de esta refactorizacion debe rotarse; eliminarla del estado actual de Git no la elimina del historial previo.

## Aplicar migraciones

La solucion conserva la migracion inicial y agrega:

- `20260715023757_AddIdentityAuthentication`: tablas, indices y relaciones de Identity, relacion opcional unica entre usuario y medico e indice de conflicto de agenda.
- `20260715025234_CascadeAppointmentDeletes`: borrado atomico de citas al eliminar su paciente o medico mediante FK de PostgreSQL.

El contexto de diseno lee la conexion desde una variable de entorno:

```powershell
$env:ConnectionStrings__PostgreSql = "Host=localhost;Port=5432;Database=citasapp;Username=<USUARIO>;Password=<PASSWORD>"
dotnet ef migrations list --project CitasApp.Infrastructure --startup-project CitasApp.Infrastructure
dotnet ef database update --project CitasApp.Infrastructure --startup-project CitasApp.Infrastructure
```

Revise la base y un respaldo antes de ejecutar `database update`. Este repositorio no ejecuta migraciones automaticamente al iniciar y no usa `EnsureCreated()` en produccion.

## Configurar JWT

La API valida issuer, audience, firma, vigencia y expiracion. Genere una clave aleatoria y guardela solo en User Secrets:

```powershell
$jwtKey = [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(48))
dotnet user-secrets set "Jwt:Issuer" "CitasApp.Api" --project CitasApp.Api
dotnet user-secrets set "Jwt:Audience" "CitasApp.Clients" --project CitasApp.Api
dotnet user-secrets set "Jwt:Key" $jwtKey --project CitasApp.Api
dotnet user-secrets set "Jwt:ExpirationMinutes" "60" --project CitasApp.Api
```

En despliegues use `Jwt__Issuer`, `Jwt__Audience`, `Jwt__Key` y `Jwt__ExpirationMinutes`. La clave debe tener al menos 32 bytes y la expiracion debe estar entre 5 y 1440 minutos.

## Inicializar roles y administrador

El inicializador crea de forma idempotente los roles `Administrador`, `Recepcionista` y `Medico`. Para crear el primer administrador, habilitelo temporalmente en un solo host; Web y API comparten la misma base:

```powershell
dotnet user-secrets set "IdentitySeed:Enabled" "true" --project CitasApp.Web
dotnet user-secrets set "IdentitySeed:Admin:Email" "<EMAIL_ADMIN>" --project CitasApp.Web
dotnet user-secrets set "IdentitySeed:Admin:Password" "<PASSWORD_ADMIN_SEGURO>" --project CitasApp.Web
dotnet run --project CitasApp.Web
```

Despues del primer inicio correcto, detenga el host y deshabilite el seed:

```powershell
dotnet user-secrets set "IdentitySeed:Enabled" "false" --project CitasApp.Web
```

La contrasena debe tener al menos 12 caracteres, mayuscula, minuscula, numero y caracter no alfanumerico. Identity almacena el hash, nunca la contrasena. Un administrador puede crear los demas usuarios desde `/Usuarios` o `POST /api/auth/users`.

Un usuario con rol `Medico` requiere una relacion explicita con un registro `Medico`; no se infiere por nombre o correo. Un medico solo puede vincularse con un usuario.

## Ejecutar

```powershell
dotnet restore CitasApp.slnx
dotnet build CitasApp.slnx --no-restore
dotnet run --project CitasApp.Web --launch-profile https
```

La Web usa `https://localhost:7138`. El login esta en `/Cuenta/IniciarSesion` y el cierre de sesion aparece en la navegacion para usuarios autenticados. HTTPS es necesario porque la cookie se emite siempre con el atributo `Secure`.

En otra terminal:

```powershell
dotnet run --project CitasApp.Api --launch-profile https
```

La API usa `https://localhost:7099`.

## Probar login y JWT

`POST /api/auth/login` recibe correo y contrasena, y devuelve token, expiracion UTC, usuario no sensible y roles:

```powershell
$body = @{
  email = "<EMAIL>"
  password = "<PASSWORD>"
} | ConvertTo-Json

$login = Invoke-RestMethod `
  -Method Post `
  -Uri "https://localhost:7099/api/auth/login" `
  -ContentType "application/json" `
  -Body $body

$headers = @{ Authorization = "Bearer $($login.token)" }
Invoke-RestMethod -Uri "https://localhost:7099/api/pacientes" -Headers $headers
```

En Postman seleccione `Authorization > Bearer Token` y use el valor `token` de la respuesta. No incluya la palabra `Bearer` dentro del campo de token. La API no habilita Swagger UI actualmente.

## Permisos

| Rol | Permisos principales |
|---|---|
| `Administrador` | Usuarios, pacientes, medicos, citas, agendas y recordatorios. |
| `Recepcionista` | Pacientes, consulta de medicos, CRUD de citas, agendas y recordatorios. |
| `Medico` | Su propia agenda y cambio de estado de sus propias citas. |

Los endpoints CRUD requieren JWT. La calculadora permanece anonima porque no opera datos del negocio. La creacion de usuarios por API requiere `Administrador`; no existe registro anonimo.

## CORS

En Development, la API permite cualquier origen para facilitar pruebas locales. Fuera de Development debe configurar una lista explicita, por ejemplo:

```powershell
$env:Cors__AllowedOrigins__0 = "https://app.example.com"
```

`AllowAnyOrigin()` no es apropiado para produccion.

## Compilar y probar

```powershell
dotnet restore CitasApp.slnx
dotnet build CitasApp.slnx --no-restore
dotnet test CitasApp.slnx --no-build --no-restore
```

Las pruebas usan SQLite en memoria, exclusivamente como sustituto controlado de PostgreSQL. Cubren roles, seed idempotente, hash, login correcto e incorrecto, JWT, respuestas 401/403, CRUD, conflicto de horario y relaciones.

## Diagnostico tecnico

El inventario de smells, evidencia, riesgos, decisiones y resultado aplicado se encuentra en `docs/code-smells-report.md`.


## Uso de inteligencia artificial

Durante el desarrollo y la documentación de este proyecto se utilizaron herramientas de inteligencia artificial como apoyo para consultar conceptos, detectar posibles errores, proponer mejoras de código y redactar parte de la documentación técnica.

Las respuestas generadas por estas herramientas no fueron incorporadas de manera automática. Cada sugerencia fue revisada, adaptada y validada mediante compilación, pruebas y análisis del funcionamiento de la aplicación.

La inteligencia artificial se utilizó únicamente como herramienta de asistencia. Las decisiones de arquitectura, implementación, seguridad, configuración y diseño final son responsabilidad de los autores del proyecto.
