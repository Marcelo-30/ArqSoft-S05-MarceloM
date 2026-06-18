namespace CitasApp.Services
{
    public interface IJsonFileService<T>
    {
        List<T> Leer();
        void Guardar(List<T> datos);
    }
}
