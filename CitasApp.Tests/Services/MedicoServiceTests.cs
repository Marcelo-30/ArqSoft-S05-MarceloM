using CitasApp.Application.Services;
using CitasApp.Domain.Models;
using CitasApp.Tests.Fakes;

namespace CitasApp.Tests.Services;

public sealed class MedicoServiceTests
{
    [Fact]
    public async Task CrearAsync_ValidDoctor_AssignsIdentifierAndPersistsDoctor()
    {
        // Arrange
        InMemoryMedicoRepository doctorRepository = new();
        MedicoService service = new(doctorRepository, new InMemoryCitaRepository());
        Medico doctor = CreateDoctor();

        // Act
        await service.CrearAsync(doctor);

        // Assert
        Assert.True(Guid.TryParse(doctor.Id, out _));
        Assert.Same(doctor, Assert.Single(doctorRepository.Doctors));
    }

    [Fact]
    public async Task EliminarAsync_ExistingDoctor_RemovesAppointmentsBeforeDoctor()
    {
        // Arrange
        Medico doctor = CreateDoctor();
        doctor.Id = "doctor-1";
        InMemoryMedicoRepository doctorRepository = new([doctor]);
        InMemoryCitaRepository appointmentRepository = new(
        [
            CreateAppointment("appointment-1", doctor.Id),
            CreateAppointment("appointment-2", "doctor-2")
        ]);
        MedicoService service = new(doctorRepository, appointmentRepository);

        // Act
        bool deleted = await service.EliminarAsync(doctor.Id);

        // Assert
        Assert.True(deleted);
        Assert.Empty(doctorRepository.Doctors);
        Assert.Equal(1, appointmentRepository.DeleteByDoctorCallCount);
        Assert.DoesNotContain(
            appointmentRepository.Appointments,
            appointment => appointment.MedicoId == doctor.Id);
        Assert.Contains(
            appointmentRepository.Appointments,
            appointment => appointment.MedicoId == "doctor-2");
    }

    [Fact]
    public async Task EliminarAsync_UnknownDoctor_ReturnsFalseWithoutDeletingAppointments()
    {
        // Arrange
        InMemoryMedicoRepository doctorRepository = new();
        InMemoryCitaRepository appointmentRepository = new(
            [CreateAppointment("appointment-1", "doctor-missing")]);
        MedicoService service = new(doctorRepository, appointmentRepository);

        // Act
        bool deleted = await service.EliminarAsync("doctor-missing");

        // Assert
        Assert.False(deleted);
        Assert.Equal(0, appointmentRepository.DeleteByDoctorCallCount);
        Assert.Single(appointmentRepository.Appointments);
    }

    private static Medico CreateDoctor()
    {
        return new Medico
        {
            Nombre = "Elena",
            Apellido = "Torres",
            Especialidad = "Medicina interna",
            NumeroLicencia = "TEST-002"
        };
    }

    private static Cita CreateAppointment(string id, string doctorId)
    {
        return new Cita
        {
            Id = id,
            PacienteId = "patient-1",
            MedicoId = doctorId,
            Fecha = new DateOnly(2026, 9, 11),
            Hora = new TimeOnly(11, 0),
            Motivo = "Consulta",
            Estado = EstadosCita.Pendiente
        };
    }
}
