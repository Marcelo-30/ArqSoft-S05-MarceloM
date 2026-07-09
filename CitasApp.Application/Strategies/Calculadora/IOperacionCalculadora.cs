namespace CitasApp.Application.Strategies.Calculadora
{
    public interface IOperacionCalculadora
    {
        string Nombre { get; }

        string Simbolo { get; }

        ResultadoOperacionCalculadora Ejecutar(double numero1, double numero2);
    }
}
