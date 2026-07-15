# Diagnostico de code smells

Fecha del analisis: 2026-07-14

Rama analizada: `Code-smells` (`c2b2264`)

Alcance: proyectos Domain, Application, Infrastructure, Web y Api, configuracion y migraciones.

## Linea base

- `dotnet restore CitasApp.slnx`: correcto despues de habilitar el acceso a la configuracion local de NuGet.
- `dotnet build CitasApp.slnx --no-restore`: correcto, 0 advertencias y 0 errores.
- `dotnet test CitasApp.slnx --no-build`: correcto, pero no descubrio pruebas porque la solucion no contiene proyectos de test.
- La persistencia activa en Web y Api es PostgreSQL. Los adaptadores JSON y memoria existen, pero no estan registrados.

## Hallazgos

| Archivo | Clase o metodo | Smell | Evidencia y problema | Riesgo | Refactorizacion propuesta | Decision inicial |
|---|---|---|---|---|---|---|
| `CitasApp.Web/appsettings.json`, `CitasApp.Api/appsettings.json` | `ConnectionStrings:PostgreSql` | Secreto versionado | Ambos archivos contienen usuario y contrasena reales en texto plano. Expone acceso a la base y duplica configuracion sensible. | Alto | Eliminar credenciales del repositorio, dejar una plantilla vacia y documentar User Secrets o variables de entorno. La credencial expuesta debe rotarse manualmente. | Aplicar |
| `CitasApp.Domain/CitasApp.Domain.csproj`, `CitasApp.Application/CitasApp.Application.csproj` | Referencias de paquetes | Violacion de arquitectura hexagonal | Domain y Application referencian EF Core, herramientas de EF y Npgsql aunque no usan esos paquetes. El nucleo queda acoplado a PostgreSQL y al ORM. | Alto | Mantener esos paquetes exclusivamente en Infrastructure; Domain solo contiene modelo y puertos, Application casos de uso. | Aplicar |
| `CitasApp.Domain/Interfaces/IRepository.cs` | `Leer`, `Guardar` | Repositorio generico inadecuado / interfaz pobre | La misma interfaz obliga a leer una coleccion completa y reemplazarla para cualquier consulta o mutacion. No expresa consultas por id ni operaciones CRUD. | Alto | Reemplazarla por puertos explicitos y asincronos para pacientes, medicos y citas. | Aplicar |
| `CitasApp.Infrastructure/Repositories/PostgreSqlRepository.cs` | `Guardar` | Sincronizacion destructiva / shotgun surgery | Cada guardado carga toda la tabla, elimina toda fila ausente de la lista recibida, actualiza el resto y guarda en bloque. Una lista parcial puede borrar datos ajenos. | Alto | Sustituir el repositorio generico por adaptadores concretos que inserten, actualicen o eliminen una sola entidad y usen consultas especificas. | Aplicar |
| `CitasApp.Infrastructure/Repositories/PostgreSqlRepository.cs` | `Leer`, `Guardar` | I/O de base de datos sincrono y contencion | `ToList()` y `SaveChanges()` bloquean el hilo de la solicitud. Las lecturas completas amplifican memoria, latencia y ventanas de carrera. | Alto | Usar `ToListAsync`, `SingleOrDefaultAsync`, `SaveChangesAsync` y `CancellationToken`. | Aplicar |
| `PacienteService.Eliminar`, `MedicoService.Eliminar`; `CitasAppDbContext.ConfigurarCita` | Flujo de eliminacion | Orden incompatible con llaves foraneas | Los servicios guardan primero la lista de padres sin el paciente/medico. PostgreSQL tiene FK `Restrict`, por lo que falla antes de que el servicio elimine las citas relacionadas. | Alto | Eliminar primero las citas dependientes y despues el padre dentro de una operacion controlada; conservar las FK definitivas. | Aplicar |
| `PacienteService.Crear`, `MedicoService.Crear`, `CitaService.Crear` | `GenerarSiguienteId` | Condicion de carrera / identificadores inconsistentes | Dos solicitudes pueden calcular el mismo `P<n>`, `M<n>` o `C<n>`. Ademas, el modelo inicializa un GUID que luego el servicio reemplaza por otro esquema. | Alto | Usar un unico esquema de GUID generado en Application cuando el id esta vacio. Conservar el tipo `string` para no romper datos existentes. | Aplicar parcialmente |
| `PacienteService`, `MedicoService`, `CitaService`; controladores MVC | Crear y actualizar | Validacion insuficiente y repetida | Se aceptan cadenas vacias, ids inexistentes y modelos invalidos. Los POST MVC no revisan `ModelState`; la validacion paciente/medico solo existe en un controlador API. | Alto | Agregar restricciones declarativas al modelo, validar `ModelState` en los adaptadores y mover reglas de integridad compartidas a Application. | Aplicar |
| `CitasApp.Api/Controllers/*`, `CitasApp.Web/Controllers/*` | Todos los endpoints funcionales | Falta de autenticacion y autorizacion | `UseAuthorization` esta presente, pero no hay autenticacion, usuarios, roles ni atributos `Authorize`; todos los CRUD estan expuestos. | Alto | Integrar ASP.NET Core Identity en Infrastructure, cookies en Web, JWT en Api y politicas/roles en los adaptadores de entrada. | Aplicar |
| `CitasApp.Web/Program.cs`, `CitasApp.Api/Program.cs` | Registro de DbContext, repositorios y servicios | Configuracion duplicada | Ambos puntos de entrada repiten la conexion, DbContext, tres repositorios y tres servicios. Un cambio exige editar dos archivos y facilita configuraciones divergentes. | Medio | Crear extensiones de registro en Infrastructure y conservar en cada host solo su autenticacion y pipeline especificos. | Aplicar |
| `CitasApp.Api/Program.cs` | Politica `ApiCors` | Configuracion insegura para produccion | `AllowAnyOrigin`, `AllowAnyHeader` y `AllowAnyMethod` permiten cualquier origen. Es util durante desarrollo, pero demasiado amplio en produccion. | Medio | Hacer los origenes configurables; permitir origenes abiertos solo de forma explicita para desarrollo y documentar el riesgo. | Aplicar |
| Modelos `Paciente`, `Medico`, `Cita` | Propiedades `string` de id y estado | Primitive obsession | Identificadores y estados son cadenas libres; `Estado` permite valores arbitrarios y los ids vacios llegan a persistencia. | Medio | Centralizar estados validos y generacion de ids. No convertir ahora todos los ids a value objects porque implicaria una migracion y cambios amplios sin beneficio proporcional. | Aplicar parcialmente |
| `AgendaMedicoController.ObtenerAgendaMedico`, `RecordatoriosController.CrearRecordatorios` | Proyeccion de agenda/recordatorios | Feature envy / carga completa | Los controladores combinan tablas, crean DTOs y hacen busquedas lineales en memoria despues de cargar colecciones completas. Esto mezcla caso de uso con transporte HTTP. | Medio | Los nuevos puertos evitaran lecturas innecesarias. Mover estas proyecciones a consultas de Application queda fuera de este cambio para evitar una reescritura del modulo de recordatorios. | No modificar completamente |
| `JsonRepository<T>` y `JsonRepositoryFactory<TRepository>` | Lectura/escritura y resolucion de ruta | Acoplamiento y concurrencia de archivos | El adaptador generico reemplaza todo el archivo sin bloqueo ni escritura atomica; la fabrica conoce la ruta fisica de `CitasApp.Web`. Actualmente no esta activo. | Medio | Adaptar sus contratos al CRUD nuevo y serializar el acceso. Mantenerlo como adaptador alternativo, sin registrarlo junto con PostgreSQL. La ruta compartida se conserva por compatibilidad. | Aplicar parcialmente |
| `CitasApp.Api/Controllers/RecordatoriosController.cs` | `EnviarRecordatorioWhatsapp` | Nombre/resultado enganoso | La accion devuelve `Enviado = true`, aunque solo construye una URL y declara que el envio es una simulacion. | Bajo | Renombrar el concepto o devolver un estado de simulacion. No es esencial para persistencia/autenticacion y cambiar el contrato puede romper clientes. | No modificar |
| `README.md` | Persistencia y patrones | Documentacion obsoleta | Describe JSON y repositorios Singleton como implementacion activa, aunque los hosts registran PostgreSQL con alcance `Scoped`. No explica migraciones, secretos ni autenticacion. | Medio | Actualizar configuracion, ejecucion, migraciones, login, JWT, roles y adaptadores activos. | Aplicar |
| Solucion | Cobertura automatizada | Falta de red de seguridad | No hay pruebas de servicios, persistencia, autenticacion, roles ni endpoints protegidos. `dotnet test` termina sin ejecutar casos. | Alto | Agregar pruebas unitarias/integracion para CRUD, relaciones, seed de roles, login y autorizacion. | Aplicar |

## Aspectos revisados sin smell confirmado

- Las estrategias de calculadora son inmutables y no dependen de servicios con alcance menor; registrarlas como `Singleton` es valido.
- `CitasAppDbContext` y los repositorios PostgreSQL estan registrados como `Scoped`, que es la duracion correcta para EF Core.
- Las llaves foraneas de citas y los indices unicos de email/licencia son controles de integridad apropiados y deben conservarse.
- No se usa `EnsureCreated()` ni se borraron migraciones previas.
- Los repositorios JSON y memoria no se eliminan: son adaptadores alternativos utiles, aunque hoy no esten activos.
- No se encontro manejo de excepciones que exponga deliberadamente trazas internas; el problema es la ausencia de traduccion controlada para conflictos de persistencia, que se abordara en los adaptadores HTTP.

## Alcance de la refactorizacion

Se priorizan riesgos de seguridad, integridad y concurrencia. No se hara una reescritura completa ni una conversion masiva de ids a value objects. Los controladores de agenda y recordatorios conservaran su contrato; se reducira su costo indirectamente mediante puertos de consulta especificos y se documentara la deuda restante.

El estado `Decision inicial` registra la decision tomada antes de modificar codigo. Al finalizar se agregara una seccion con las refactorizaciones realmente aplicadas y cualquier desviacion justificada.
