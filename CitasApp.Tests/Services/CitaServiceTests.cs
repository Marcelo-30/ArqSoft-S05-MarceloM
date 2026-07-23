using CitasApp.Application.Exceptions;
using CitasApp.Application.Services;
using CitasApp.Domain.Models;
using CitasApp.Tests.Fakes;

namespace CitasApp.Tests.Services;

public sealed class CitaServiceTests
{
    private static readonly DateOnly AppointmentDate = new(2026, 8, 15);
    private static readonly TimeOnly AppointmentTime = new(10, 30);

    [Fact]
    public async Task CrearAsync_ValidAppointment_AddsAppointmentWithNormalizedState()
    {
        // Arrange
        Paciente patient = CreatePatient();
        Medico doctor = CreateDoctor();
        InMemoryCitaRepository appointmentRepository = new();
        CitaService service = new(
            appointmentRepository,
            new InMemoryPacienteRepository([patient]),
            new InMemoryMedicoRepository([doctor]));
        Cita appointment = CreateAppointment(patient.Id, doctor.Id);
        appointment.Estado = " confirmada ";

        // Act
        await service.CrearAsync(appointment);

        // Assert
        Assert.True(Guid.TryParse(appointment.Id, out _));
        Assert.Equal(EstadosCita.Cancelada, appointment.Estado);
        Assert.Same(appointment, Assert.Single(appointmentRepository.Appointments));
    }

    [Fact]
    public async Task CrearAsync_ExistingAppointmentAtSameSlot_ThrowsSchedulingConflict()
    {
        // Arrange
        Paciente patient = CreatePatient();
        Medico doctor = CreateDoctor();
        Cita existingAppointment = CreateAppointment(patient.Id, doctor.Id);
        existingAppointment.Id = "appointment-existing";
        InMemoryCitaRepository appointmentRepository = new([existingAppointment]);
        CitaService service = new(
            appointmentRepository,
            new InMemoryPacienteRepository([patient]),
            new InMemoryMedicoRepository([doctor]));
        Cita conflictingAppointment = CreateAppointment(patient.Id, doctor.Id);

        // Act
        Func<Task> act = () => service.CrearAsync(conflictingAppointment);

        // Assert
        await Assert.ThrowsAsync<ConflictoHorarioCitaException>(act);
        Assert.Single(appointmentRepository.Appointments);
    }

    [Fact]
    public async Task CrearAsync_UnknownPatient_ThrowsValidationException()
    {
        // Arrange
        Medico doctor = CreateDoctor();
        InMemoryCitaRepository appointmentRepository = new();
        CitaService service = new(
            appointmentRepository,
            new InMemoryPacienteRepository(),
            new InMemoryMedicoRepository([doctor]));
        Cita appointment = CreateAppointment("patient-missing", doctor.Id);

        // Act
        Func<Task> act = () => service.CrearAsync(appointment);

        // Assert
        ValidacionCitaException exception =
            await Assert.ThrowsAsync<ValidacionCitaException>(act);
        Assert.Equal("El paciente seleccionado no existe.", exception.Message);
        Assert.Empty(appointmentRepository.Appointments);
    }

    private static Paciente CreatePatient()
    {
        return new Paciente
        {
            Id = "patient-1",
            Nombre = "Ana",
            Apellido = "Pruebas",
            Email = "ana.tests@citasapp.local",
            Telefono = "+525500000001"
        };
    }

    private static Medico CreateDoctor()
    {
        return new Medico
        {
            Id = "doctor-1",
            Nombre = "Luis",
            Apellido = "Pruebas",
            Especialidad = "Cardiologia",
            NumeroLicencia = "TEST-001"
        };
    }

    private static Cita CreateAppointment(string patientId, string doctorId)
    {
        return new Cita
        {
            PacienteId = patientId,
            MedicoId = doctorId,
            Fecha = AppointmentDate,
            Hora = AppointmentTime,
            Motivo = "Consulta de seguimiento",
            Estado = EstadosCita.Pendiente
        };
    }
}
