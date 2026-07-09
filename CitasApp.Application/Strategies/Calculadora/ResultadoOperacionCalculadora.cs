namespace CitasApp.Application.Strategies.Calculadora
{
    public sealed class ResultadoOperacionCalculadora
    {
        private ResultadoOperacionCalculadora(bool exitoso, double resultado, string? mensajeError)
        {
            Exitoso = exitoso;
            Resultado = resultado;
            MensajeError = mensajeError;
        }

        public bool Exitoso { get; }

        public double Resultado { get; }

        public string? MensajeError { get; }

        public static ResultadoOperacionCalculadora Exito(double resultado)
        {
            return new ResultadoOperacionCalculadora(true, resultado, null);
        }

        public static ResultadoOperacionCalculadora Error(string mensajeError)
        {
            return new ResultadoOperacionCalculadora(false, 0, mensajeError);
        }
    }
}
