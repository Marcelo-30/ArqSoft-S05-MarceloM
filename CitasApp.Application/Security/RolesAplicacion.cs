namespace CitasApp.Application.Security
{
    public static class RolesAplicacion
    {
        public const string Administrador = "Administrador";
        public const string Recepcionista = "Recepcionista";
        public const string Medico = "Medico";

        public static readonly IReadOnlySet<string> Todos = new HashSet<string>(
            [Administrador, Recepcionista, Medico],
            StringComparer.Ordinal);
    }
}
