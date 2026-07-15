namespace CitasApp.Domain.Models
{
    public static class EstadosCita
    {
        public const string Pendiente = "Pendiente";
        public const string Confirmada = "Confirmada";
        public const string Cancelada = "Cancelada";

        public static readonly IReadOnlySet<string> Todos = new HashSet<string>(
            [Pendiente, Confirmada, Cancelada],
            StringComparer.OrdinalIgnoreCase);
    }
}
