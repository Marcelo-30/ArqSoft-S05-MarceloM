using CitasApp.Domain.Interfaces;
using CitasApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CitasApp.Infrastructure.Repositories
{
    public class PostgreSqlRepository<T> : IRepository<T>
        where T : class
    {
        private readonly CitasAppDbContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly Func<T, string> _obtenerId;
        private readonly Action<T, string> _asignarId;

        public PostgreSqlRepository(
            CitasAppDbContext context,
            Func<T, string> obtenerId,
            Action<T, string> asignarId)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _obtenerId = obtenerId;
            _asignarId = asignarId;
        }

        public List<T> Leer()
        {
            return _dbSet
                .AsNoTracking()
                .ToList();
        }

        public void Guardar(List<T> datos)
        {
            ArgumentNullException.ThrowIfNull(datos);

            GenerarIdsFaltantes(datos);
            ValidarIdsDuplicados(datos);

            List<T> registrosExistentes = _dbSet.ToList();

            Dictionary<string, T> registrosPorId =
                registrosExistentes.ToDictionary(
                    registro => _obtenerId(registro));

            HashSet<string> idsRecibidos = datos
                .Select(registro => _obtenerId(registro))
                .ToHashSet();

            // Elimina de PostgreSQL los registros que ya no están
            // en la lista recibida.
            List<T> registrosAEliminar = registrosExistentes
                .Where(registro =>
                    !idsRecibidos.Contains(_obtenerId(registro)))
                .ToList();

            _dbSet.RemoveRange(registrosAEliminar);

            foreach (T registro in datos)
            {
                string id = _obtenerId(registro);

                if (registrosPorId.TryGetValue(
                    id,
                    out T? registroExistente))
                {
                    _context.Entry(registroExistente)
                        .CurrentValues
                        .SetValues(registro);
                }
                else
                {
                    _dbSet.Add(registro);
                }
            }

            _context.SaveChanges();
        }

        private void GenerarIdsFaltantes(List<T> datos)
        {
            foreach (T registro in datos)
            {
                string id = _obtenerId(registro);

                if (string.IsNullOrWhiteSpace(id))
                {
                    _asignarId(
                        registro,
                        Guid.NewGuid().ToString());
                }
            }
        }

        private void ValidarIdsDuplicados(List<T> datos)
        {
            string? idDuplicado = datos
                .GroupBy(registro => _obtenerId(registro))
                .Where(grupo => grupo.Count() > 1)
                .Select(grupo => grupo.Key)
                .FirstOrDefault();

            if (idDuplicado is not null)
            {
                throw new InvalidOperationException(
                    $"Existen registros duplicados con el ID '{idDuplicado}'.");
            }
        }
    }
}
