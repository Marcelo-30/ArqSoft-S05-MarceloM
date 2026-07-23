using CitasApp.Application.Services;
using CitasApp.Domain.Models;
using CitasApp.Tests.Fakes;

namespace CitasApp.Tests.Services;

public sealed class PacienteServiceTests
{
    [Fact]
    public async Task CrearAsync_ValidPatient_AssignsIdentifierAndPersistsPatient()
    {
        // Arrange
        InMemoryPacienteRepository patientRepository = new();
        PacienteService service = new(patientRepository, new InMemoryCitaRepository());
        Paciente patient = CreatePatient();

        // Act
        await service.CrearAsync(patient);

        // Assert
        Assert.True(Guid.TryParse(patient.Id, out _));
        Assert.Same(patient, Assert.Single(patientRepository.Patients));
    }

    [Fact]
    public async Task EliminarAsync_ExistingPatient_RemovesAppointmentsBeforePatient()
    {
        // Arrange
        Paciente patient = CreatePatient();
        patient.Id = "patient-1";
        InMemoryPacienteRepository patientRepository = new([patient]);
        InMemoryCitaRepository appointmentRepository = new(
        [
            CreateAppointment("appointment-1", patient.Id),
            CreateAppointment("appointment-2", "patient-2")
        ]);
        PacienteService service = new(patientRepository, appointmentRepository);

        // Act
        bool deleted = await service.EliminarAsync(patient.Id);

        // Assert
        Assert.True(deleted);
        Assert.Empty(patientRepository.Patients);
        Assert.Equal(1, appointmentRepository.DeleteByPatientCallCount);
        Assert.DoesNotContain(
            appointmentRepository.Appointments,
            appointment => appointment.PacienteId == patient.Id);
        Assert.Contains(
            appointmentRepository.Appointments,
            appointment => appointment.PacienteId == "patient-2");
    }

    [Fact]
    public async Task EliminarAsync_UnknownPatient_ReturnsFalseWithoutDeletingAppointments()
    {
        // Arrange
        InMemoryPacienteRepository patientRepository = new();
        InMemoryCitaRepository appointmentRepository = new(
            [CreateAppointment("appointment-1", "patient-missing")]);
        PacienteService service = new(patientRepository, appointmentRepository);

        // Act
        bool deleted = await service.EliminarAsync("patient-missing");

        // Assert
        Assert.False(deleted);
        Assert.Equal(0, appointmentRepository.DeleteByPatientCallCount);
        Assert.Single(appointmentRepository.Appointments);
    }

    private static Paciente CreatePatient()
    {
        return new Paciente
        {
            Nombre = "Sofia",
            Apellido = "Ramirez",
            Email = "sofia.tests@citasapp.local",
            Telefono = "+525500000002"
        };
    }

    private static Cita CreateAppointment(string id, string patientId)
    {
        return new Cita
        {
            Id = id,
            PacienteId = patientId,
            MedicoId = "doctor-1",
            Fecha = new DateOnly(2026, 9, 10),
            Hora = new TimeOnly(9, 0),
            Motivo = "Consulta",
            Estado = EstadosCita.Pendiente
        };
    }
}
