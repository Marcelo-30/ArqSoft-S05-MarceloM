# Arquitectura C4 del Proyecto

## Descripción del proyecto

CitasApp es un sistema desarrollado con ASP.NET Core para gestionar pacientes, médicos y citas médicas. El proyecto incluye una aplicación web MVC para administrar la información y una API REST para consultar agendas médicas, revisar recordatorios pendientes y generar enlaces simulados de recordatorio por WhatsApp.

El sistema ayuda a organizar la atención médica básica, centralizando el registro de pacientes, médicos y citas en archivos JSON usados como mecanismo simple de persistencia.

## C4 Nivel 1 — Contexto

### ¿Para quién es este nivel?

Este nivel está dirigido a personas que necesitan entender el sistema de forma general, como usuarios, profesores, clientes, compañeros de equipo o evaluadores técnicos. No requiere conocer detalles internos de programación.

### ¿Qué pregunta responde?

Responde la pregunta: ¿qué es el sistema, quién lo usa y con qué sistemas externos se comunica?

```mermaid
C4Context
    title Diagrama de Contexto - C4 Nivel 1

    Person(personalAdministrativo, "Personal administrativo", "Registra y administra pacientes, medicos y citas medicas.")
    Person(medico, "Medico", "Consulta su agenda de citas medicas.")
    Person(paciente, "Paciente", "Recibe informacion relacionada con sus citas y recordatorios.")

    System(citasApp, "CitasApp", "Sistema web y API REST para gestionar pacientes, medicos y citas medicas.")
    System_Ext(whatsapp, "WhatsApp", "Canal externo usado para abrir mensajes de recordatorio generados por el sistema.")

    Rel(personalAdministrativo, citasApp, "Gestiona pacientes, medicos y citas")
    Rel(medico, citasApp, "Consulta agenda medica")
    Rel(paciente, citasApp, "Recibe informacion y recordatorios")
    Rel(citasApp, whatsapp, "Genera enlace de recordatorio", "wa.me")
```
