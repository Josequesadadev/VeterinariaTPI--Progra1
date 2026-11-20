using Microsoft.EntityFrameworkCore;
using Veterinaria.Logica.Data;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories.Interfaces;

namespace Veterinaria.Logica.Repositories
{
    public class ClienteRepository : Repository<Cliente>, IClienteRepository
    {
        private const string Collation = "Modern_Spanish_CI_AI";

        public ClienteRepository(VeterinariaDbContext db) : base(db) { }

        public override List<Cliente> GetAll(bool incluirInactivos)
        {
            return GetByFiltro(null, incluirInactivos);
        }

        public List<Cliente> GetByFiltro(string? texto, bool incluirInactivos)
        {
            IQueryable<Cliente> consulta = Query().Where(c => incluirInactivos || c.Activo);

            if (!string.IsNullOrWhiteSpace(texto))
            {
                // Collation CI_AI: no distingue mayusculas ni acentos ("gomez" encuentra "Gómez")
                consulta = consulta.Where(c => EF.Functions.Collate(c.Codigo, Collation).Contains(texto)
                                            || EF.Functions.Collate(c.Nombre, Collation).Contains(texto)
                                            || EF.Functions.Collate(c.Apellido, Collation).Contains(texto));
            }

            return consulta.OrderBy(c => c.Apellido).ThenBy(c => c.Nombre).ToList();
        }

        public bool ExisteCodigo(string codigo, int idExcluir)
        {
            return db.Clientes.Any(c => c.Codigo == codigo && c.Id != idExcluir);
        }

        public bool TieneMascotasActivas(int idCliente)
        {
            return db.Mascotas.Any(m => m.IdCliente == idCliente && m.Activo);
        }
    }
}
