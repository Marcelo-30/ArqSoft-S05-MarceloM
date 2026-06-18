namespace CitasApp.Application.Interfaces
{
    public interface IRepository<T>
    {
        List<T> Leer();

        void Guardar(List<T> datos);
    }
}
