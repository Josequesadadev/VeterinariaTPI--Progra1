using Microsoft.EntityFrameworkCore;
using Veterinaria.Logica.Data;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories.Interfaces;

namespace Veterinaria.Logica.Repositories
{
    public class TipoMascotaRepository : ITipoMascotaRepository
    {
        private readonly VeterinariaDbContext db;

        public TipoMascotaRepository(VeterinariaDbContext db)
        {
            this.db = db;
        }

        public List<TipoMascota> GetAll(bool incluirInactivos)
        {
            if (incluirInactivos)
            {
                return db.TiposMascota.OrderBy(t => t.Nombre).ToList();
            }
            return db.TiposMascota.Where(t => t.Activo == true).OrderBy(t => t.Nombre).ToList();
        }

        public TipoMascota? GetById(int id)
        {
            return db.TiposMascota.Find(id);
        }

        public void Add(TipoMascota tipo)
        {
            db.TiposMascota.Add(tipo);
            db.SaveChanges();
        }

        public void Update(TipoMascota tipo)
        {
            db.Entry(tipo).State = EntityState.Modified;
            db.SaveChanges();
        }

        public bool ExisteNombre(string nombre, int idExcluir)
        {
            return db.TiposMascota.Any(t => t.Nombre == nombre && t.Id != idExcluir);
        }

        public bool TieneMascotasActivas(int idTipo)
        {
            return db.Mascotas.Any(m => m.IdTipoMascota == idTipo && m.Activo == true);
        }
    }
}
