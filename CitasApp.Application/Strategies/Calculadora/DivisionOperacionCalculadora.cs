namespace CitasApp.Application.Strategies.Calculadora
{
    public sealed class DivisionOperacionCalculadora : IOperacionCalculadora
    {
        public string Nombre => "division";

        public string Simbolo => "/";

        public ResultadoOperacionCalculadora Ejecutar(double numero1, double numero2)
        {
            if (numero2 == 0)
            {
                return ResultadoOperacionCalculadora.Error("No se puede dividir entre cero.");
            }

            return ResultadoOperacionCalculadora.Exito(numero1 / numero2);
        }
    }
}
