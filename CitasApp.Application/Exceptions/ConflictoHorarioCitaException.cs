namespace CitasApp.Application.Exceptions
{
    public sealed class ConflictoHorarioCitaException : Exception
    {
        public ConflictoHorarioCitaException()
            : base("El medico ya tiene una cita programada en ese horario.")
        {
        }
    }
}
