namespace CitasApp.Application.Exceptions
{
    public sealed class ValidacionCitaException : Exception
    {
        public ValidacionCitaException(string message)
            : base(message)
        {
        }
    }
}
