using CitasApp.Application.Exceptions;
using CitasApp.Application.Services;
using CitasApp.Domain.Models;
using CitasApp.Infrastructure.Persistence;
using CitasApp.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CitasApp.Tests;

public sealed class PersistenceIntegrationTests
{
    [Fact]
    public async Task Services_PersistRelationshipsAndRemoveDependentAppointments()
    {
        await using SqliteConnection connection = new("Data Source=:memory:");
        await connection.OpenAsync();

        DbContextOptions<CitasAppDbContext> options =
            new DbContextOptionsBuilder<CitasAppDbContext>()
                .UseSqlite(connection)
                .Options;

        await using CitasAppDbContext context = new(options);
        await context.Database.EnsureCreatedAsync();

        PostgreSqlPacienteRepository pacienteRepository = new(context);
        PostgreSqlMedicoRepository medicoRepository = new(context);
        PostgreSqlCitaRepository citaRepository = new(context);
        PacienteService pacienteService = new(pacienteRepository, citaRepository);
        MedicoService medicoService = new(medicoRepository, citaRepository);
        CitaService citaService = new(citaRepository, pacienteRepository, medicoRepository);

        Paciente paciente = new()
        {
            Nombre = "Ana",
            Apellido = "Pruebas",
            Email = "ana.persistence@citasapp.local",
            Telefono = "+525500000001"
        };
        Medico medico = new()
        {
            Nombre = "Luis",
            Apellido = "Pruebas",
            Especialidad = "Cardiologia",
            NumeroLicencia = "TEST-PERSISTENCE-001"
        };

        await pacienteService.CrearAsync(paciente);
        await medicoService.CrearAsync(medico);

        Cita cita = new()
        {
            PacienteId = paciente.Id,
            MedicoId = medico.Id,
            Fecha = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            Hora = new TimeOnly(10, 30),
            Motivo = "Consulta de integracion",
            Estado = EstadosCita.Pendiente
        };
        await citaService.CrearAsync(cita);

        Assert.False(string.IsNullOrWhiteSpace(paciente.Id));
        Assert.False(string.IsNullOrWhiteSpace(medico.Id));
        Assert.False(string.IsNullOrWhiteSpace(cita.Id));
        Assert.NotNull(await citaService.ObtenerPorIdAsync(cita.Id));
        Assert.Equal(
            2,
            context.Model.FindEntityType(typeof(Cita))!.GetForeignKeys().Count());
        Assert.All(
            context.Model.FindEntityType(typeof(Cita))!.GetForeignKeys(),
            foreignKey => Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior));

        Cita conflictingAppointment = new()
        {
            PacienteId = paciente.Id,
            MedicoId = medico.Id,
            Fecha = cita.Fecha,
            Hora = cita.Hora,
            Motivo = "Horario duplicado",
            Estado = EstadosCita.Confirmada
        };

        await Assert.ThrowsAsync<ConflictoHorarioCitaException>(
            () => citaService.CrearAsync(conflictingAppointment));

        Assert.True(await pacienteService.EliminarAsync(paciente.Id));
        Assert.Null(await pacienteService.ObtenerPorIdAsync(paciente.Id));
        Assert.Null(await citaService.ObtenerPorIdAsync(cita.Id));
        Assert.NotNull(await medicoService.ObtenerPorIdAsync(medico.Id));
    }
}
