# Actividad 35: pruebas automatizadas e integración continua

## Objetivo

Incorporar pruebas xUnit deterministas para reglas de negocio de CitasApp y validar automáticamente la solución .NET 10 mediante GitHub Actions en cada `push` y `pull_request`.

## Clases probadas

### `CitaService`

- Crea una cita válida, genera su identificador y normaliza el estado.
- Rechaza una cita cuando el médico ya tiene una reserva activa en el mismo horario.
- Rechaza una cita asociada a un paciente inexistente.

### `PacienteService`

- Crea un paciente válido, genera su identificador y lo persiste.
- Elimina las citas dependientes antes de eliminar un paciente existente.
- Devuelve `false` sin alterar citas cuando el paciente no existe.

### `MedicoService`

- Crea un médico válido, genera su identificador y lo persiste.
- Elimina las citas dependientes antes de eliminar un médico existente.
- Devuelve `false` sin alterar citas cuando el médico no existe.

Las nueve pruebas nuevas usan `[Fact]`, nombres descriptivos y secciones Arrange-Act-Assert. Los repositorios en memoria se encuentran en `CitasApp.Tests/Fakes` y no requieren PostgreSQL, archivos locales, servicios externos ni variables privadas.

## Integración continua

- Workflow: `.github/workflows/dotnet-tests.yml`
- Nombre del workflow: `CitasApp CI`
- Nombre del check/job: `Build and Test`
- Eventos: `push` y `pull_request`
- Solución validada: `CitasApp.slnx`
- SDK: .NET 10
- Etapas: restore, build Release y test Release

Comando local:

```bash
dotnet test --configuration Release
```

## Evidencia

- Pull Request: [#1 - Test/actividad 35 xunit ci](https://github.com/Marcelo-30/ArqSoft-S05-MarceloM/pull/1)
- Commit rojo: [`df3eab4`](https://github.com/Marcelo-30/ArqSoft-S05-MarceloM/commit/df3eab4da2f6f7db058b236015a6dba714ea2138) — `test: add intentional failure for CI verification`
- Ejecución roja: [CitasApp CI #1](https://github.com/Marcelo-30/ArqSoft-S05-MarceloM/actions/runs/29977236211) — restore y build correctos; fallo controlado en `CitaServiceTests` por esperar `Cancelada` cuando el resultado real era `Confirmada`.
- Commit verde: [`2989c9c`](https://github.com/Marcelo-30/ArqSoft-S05-MarceloM/commit/2989c9c7a396ede807b08c91fc24270664d7fbfb) — `test: fix intentional CI test failure`
- Ejecución verde: [CitasApp CI #2](https://github.com/Marcelo-30/ArqSoft-S05-MarceloM/actions/runs/29977992159) — restore, build y test correctos; 16 pruebas aprobadas.

La versión final no conserva ninguna prueba intencionalmente fallida.
