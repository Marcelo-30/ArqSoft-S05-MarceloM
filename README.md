# CitasApp

## Descripción del proyecto

CitasApp es una aplicación desarrollada con ASP.NET Core que permite gestionar pacientes, médicos y citas médicas.

El proyecto fue reorganizado desde una estructura MVC tradicional hacia una arquitectura hexagonal. En esta rama se agrega una nueva capa llamada `CitasApp.Api`, que funciona como una API REST para permitir que clientes externos consuman información del sistema.

La aplicación conserva la interfaz web MVC mediante `CitasApp.Web`, pero además expone endpoints REST para que los médicos puedan consultar su agenda desde un celular y para preparar recordatorios de citas por WhatsApp para los pacientes.

## Funcionalidades principales

* Visualización de pacientes registrados.
* Visualización del detalle de un paciente.
* Registro de nuevos pacientes.
* Edición de pacientes existentes.
* Eliminación de pacientes.
* Visualización de médicos disponibles.
* Visualización del detalle de un médico.
* Registro de nuevos médicos.
* Edición de médicos existentes.
* Eliminación de médicos.
* Visualización de la agenda completa de citas.
* Creación de nuevas citas médicas.
* Edición de citas médicas existentes.
* Eliminación de citas médicas.
* Filtrado de citas por paciente.
* Persistencia de datos mediante archivos JSON.
* Uso de interfaces en Domain como puertos del dominio.
* Uso de servicios en Application para coordinar las operaciones de la aplicación.
* Uso de repositorios como adaptadores de infraestructura.
* Aplicación MVC en `CitasApp.Web`.
* API REST en `CitasApp.Api`.
* Consulta de agenda médica mediante endpoints HTTP.
* Consulta de recordatorios pendientes.
* Generación simulada de recordatorios por WhatsApp.

## Arquitectura del proyecto

La solución está dividida en cinco proyectos:

```txt
CitasApp
│
├── CitasApp.Domain
├── CitasApp.Application
├── CitasApp.Infrastructure
├── CitasApp.Web
└── CitasApp.Api
```

## Capas de la arquitectura

### CitasApp.Domain

Contiene los modelos principales del sistema y las interfaces de repositorio. Esta capa representa el núcleo del dominio y no depende de las demás capas.

```txt
CitasApp.Domain
├── Models
│   ├── Paciente.cs
│   ├── Medico.cs
│   └── Cita.cs
│
└── Interfaces
    ├── IRepository.cs
    ├── IPacienteRepository.cs
    ├── IMedicoRepository.cs
    └── ICitaRepository.cs
```

### CitasApp.Application

Contiene los servicios de aplicación. Estos servicios usan las interfaces definidas en `CitasApp.Domain` para coordinar las operaciones de pacientes, médicos y citas sin depender directamente de implementaciones concretas de infraestructura.

```txt
CitasApp.Application
└── Services
    ├── PacienteService.cs
    ├── MedicoService.cs
    └── CitaService.cs
```

### CitasApp.Infrastructure

Contiene los adaptadores concretos de persistencia. En esta capa están los repositorios que implementan las interfaces definidas en `CitasApp.Domain`.

```txt
CitasApp.Infrastructure
└── Repositories
    ├── JsonRepository.cs
    ├── JsonPacienteRepository.cs
    ├── JsonMedicoRepository.cs
    ├── JsonCitaRepository.cs
    └── MemoriaPacienteRepository.cs
```

`JsonPacienteRepository` usa archivos JSON para guardar pacientes.

`MemoriaPacienteRepository` implementa la misma interfaz `IPacienteRepository`, pero guarda los datos en memoria. Esto permite cambiar el adaptador registrado sin modificar el dominio, los servicios de aplicación ni los controladores.

### CitasApp.Web

Contiene la aplicación ASP.NET Core MVC: controladores, vistas, archivos estáticos, configuración y archivos JSON de datos.

```txt
CitasApp.Web
├── Controllers
│   ├── PacienteController.cs
│   ├── MedicoController.cs
│   ├── CitaController.cs
│   └── HomeController.cs
│
├── Views
│   ├── Paciente
│   ├── Medico
│   ├── Cita
│   ├── Home
│   └── Shared
│
├── Data
│   ├── pacientes.json
│   ├── medicos.json
│   └── citas.json
│
├── Models
│   └── ErrorViewModel.cs
│
├── wwwroot
├── Program.cs
└── appsettings.json
```

### CitasApp.Api

Contiene una API REST como adaptador de entrada adicional. Esta capa permite que otros clientes, como una aplicación móvil o un navegador desde celular, consuman datos de la agenda médica sin depender de las vistas MVC.

```txt
CitasApp.Api
├── Controllers
│   ├── AgendaMedicoController.cs
│   └── RecordatoriosController.cs
│
├── Dtos
│   ├── AgendaMedicoDto.cs
│   ├── RecordatorioWhatsappDto.cs
│   └── EnviarWhatsappResponseDto.cs
│
├── Program.cs
├── appsettings.json
└── CitasApp.Api.csproj
```

## Endpoints de la API

### Agenda médica

```txt
GET /api/medicos/{medicoId}/agenda
GET /api/medicos/{medicoId}/agenda/hoy
GET /api/medicos/{medicoId}/agenda/fecha/{fecha}
```

Ejemplos:

```txt
GET /api/medicos/M1/agenda
GET /api/medicos/M1/agenda/hoy
GET /api/medicos/M1/agenda/fecha/2026-06-10
```

Estos endpoints permiten consultar la agenda de un médico por su identificador.

### Recordatorios por WhatsApp

```txt
GET /api/recordatorios/pendientes?dias=1
POST /api/recordatorios/whatsapp/{citaId}
```

Ejemplos:

```txt
GET /api/recordatorios/pendientes?dias=7
POST /api/recordatorios/whatsapp/C1
```

El envío por WhatsApp queda simulado. El endpoint genera el mensaje y una URL de WhatsApp (`wa.me`). Para un envío real se debe integrar un proveedor externo como Meta WhatsApp Cloud API o Twilio.

## Referencias entre proyectos

```txt
CitasApp.Web → CitasApp.Application
CitasApp.Web → CitasApp.Infrastructure
CitasApp.Web → CitasApp.Domain

CitasApp.Api → CitasApp.Application
CitasApp.Api → CitasApp.Infrastructure
CitasApp.Api → CitasApp.Domain

CitasApp.Infrastructure → CitasApp.Domain

CitasApp.Application → CitasApp.Domain

CitasApp.Domain → sin dependencias externas del proyecto
```

## Persistencia de datos

La persistencia JSON se guarda en la carpeta `Data` dentro del proyecto web:

```txt
CitasApp.Web/Data/pacientes.json
CitasApp.Web/Data/medicos.json
CitasApp.Web/Data/citas.json
```

Los repositorios JSON leen y guardan información en esos archivos. La API utiliza la misma información para consultar agendas y generar recordatorios.

## Cómo ejecutar el proyecto MVC

Desde la raíz de la solución:

```bash
dotnet run --project CitasApp.Web
```

También se puede abrir la solución en Visual Studio y establecer `CitasApp.Web` como proyecto de inicio.

## Cómo ejecutar la API REST

Desde la raíz de la solución:

```bash
dotnet run --project CitasApp.Api
```

Para probar la API desde otro dispositivo de la misma red, como un celular, se puede ejecutar escuchando en todas las interfaces de red:

```bash
dotnet run --project CitasApp.Api --urls "http://0.0.0.0:5088"
```

Después se puede abrir desde el celular usando la IP local de la computadora:

```txt
http://TU-IP:5088/api/recordatorios/pendientes?dias=7
```

Ejemplo:

```txt
http://192.168.1.68:5088/api/recordatorios/pendientes?dias=7
```

## Cómo probar el endpoint POST de WhatsApp

Desde PowerShell se puede usar:

```powershell
Invoke-RestMethod -Method POST -Uri "http://localhost:5088/api/recordatorios/whatsapp/C1"
```

También se puede usar `curl.exe`:

```bash
curl.exe -X POST http://localhost:5088/api/recordatorios/whatsapp/C1
```

Si se quiere probar desde otro dispositivo de la red:

```bash
curl.exe -X POST http://TU-IP:5088/api/recordatorios/whatsapp/C1
```

`C1` debe ser reemplazado por el identificador real de una cita existente.

## Tecnologías usadas

* C#
* ASP.NET Core MVC
* ASP.NET Core Web API
* Razor Views
* REST API
* HTML
* CSS
* Bootstrap
* JSON
* Git
* GitHub
* Visual Studio

## Nota sobre uso de IA

Durante el desarrollo de este proyecto se utilizó apoyo de inteligencia artificial como herramienta de asistencia para estructurar ideas, revisar código, implementar mejoras, migrar a arquitectura hexagonal, agregar servicios de aplicación, crear repositorios, agregar una API REST y resolver errores.

