using Microsoft.EntityFrameworkCore;
using Veterinaria.Logica.Data;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories.Interfaces;

namespace Veterinaria.Logica.Repositories
{
    public class MascotaRepository : Repository<Mascota>, IMascotaRepository
    {
        public MascotaRepository(VeterinariaDbContext db) : base(db) { }

        // Siempre traemos el cliente y el tipo junto con la mascota
        protected override IQueryable<Mascota> Query()
        {
            return db.Mascotas
                .Include(m => m.Cliente)
                .Include(m => m.TipoMascota);
        }

        public override List<Mascota> GetAll(bool incluirInactivos)
        {
            return GetByFiltro(null, null, null, incluirInactivos);
        }

        public List<Mascota> GetByFiltro(int? idCliente, int? idTipoMascota, string? nombre, bool incluirInactivos)
        {
            IQueryable<Mascota> consulta = Query().Where(m => incluirInactivos || m.Activo);

            if (idCliente.HasValue)
            {
                consulta = consulta.Where(m => m.IdCliente == idCliente.Value);
            }
            if (idTipoMascota.HasValue)
            {
                consulta = consulta.Where(m => m.IdTipoMascota == idTipoMascota.Value);
            }
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                consulta = consulta.Where(m => EF.Functions.Collate(m.Nombre, "Modern_Spanish_CI_AI").Contains(nombre));
            }

            return consulta.OrderBy(m => m.Nombre).ToList();
        }
    }
}
