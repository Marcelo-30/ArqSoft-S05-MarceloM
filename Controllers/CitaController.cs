using Microsoft.AspNetCore.Mvc;
using CitasApp.Models;

namespace CitasApp.Controllers
{
    public class CitaController : Controller
    {
        private static List<Cita> citas = new List<Cita>
        {
            new Cita
            {
                Id = "C1",
                PacienteId = "P1",
                MedicoId = "M1",
                Fecha = new DateOnly(2026, 5, 30),
                Hora = new TimeOnly(10, 30),
                Motivo = "Dolor en el pecho",
                Estado = "Pendiente"
            },
            new Cita
            {
                Id = "C2",
                PacienteId = "P2",
                MedicoId = "M2",
                Fecha = new DateOnly(2026, 5, 31),
                Hora = new TimeOnly(12, 00),
                Motivo = "Consulta general",
                Estado = "Confirmada"
            },
            new Cita
            {
                Id = "C3",
                PacienteId = "P1",
                MedicoId = "M2",
                Fecha = new DateOnly(2026, 6, 1),
                Hora = new TimeOnly(9, 00),
                Motivo = "Revisión",
                Estado = "Pendiente"
            }
        };

        public IActionResult Index()
        {
            return View(citas);
        }

        public IActionResult PorPaciente(string pacienteId)
        {
            var citasPaciente = citas
                .Where(c => c.PacienteId == pacienteId)
                .ToList();

            return View(citasPaciente);
        }
    }
}
