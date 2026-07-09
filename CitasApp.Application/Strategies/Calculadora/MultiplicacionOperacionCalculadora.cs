namespace CitasApp.Application.Strategies.Calculadora
{
    public sealed class MultiplicacionOperacionCalculadora : IOperacionCalculadora
    {
        public string Nombre => "multiplicacion";

        public string Simbolo => "x";

        public ResultadoOperacionCalculadora Ejecutar(double numero1, double numero2)
        {
            return ResultadoOperacionCalculadora.Exito(numero1 * numero2);
        }
    }
}
