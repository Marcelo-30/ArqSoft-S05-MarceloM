namespace CitasApp.Application.Strategies.Calculadora
{
    public sealed class SumaOperacionCalculadora : IOperacionCalculadora
    {
        public string Nombre => "suma";

        public string Simbolo => "+";

        public ResultadoOperacionCalculadora Ejecutar(double numero1, double numero2)
        {
            return ResultadoOperacionCalculadora.Exito(numero1 + numero2);
        }
    }
}
