using Microsoft.EntityFrameworkCore;
using Veterinaria.Logica.Data;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories.Interfaces;

namespace Veterinaria.Logica.Repositories
{
    public class TipoAtencionRepository : ITipoAtencionRepository
    {
        private readonly VeterinariaDbContext db;

        public TipoAtencionRepository(VeterinariaDbContext db)
        {
            this.db = db;
        }

        public List<TipoAtencion> GetAll(bool incluirInactivos)
        {
            if (incluirInactivos)
            {
                return db.TiposAtencion.OrderBy(t => t.Nombre).ToList();
            }
            return db.TiposAtencion.Where(t => t.Activo == true).OrderBy(t => t.Nombre).ToList();
        }

        public TipoAtencion? GetById(int id)
        {
            return db.TiposAtencion.Find(id);
        }

        public void Add(TipoAtencion tipo)
        {
            db.TiposAtencion.Add(tipo);
            db.SaveChanges();
        }

        public void Update(TipoAtencion tipo)
        {
            db.Entry(tipo).State = EntityState.Modified;
            db.SaveChanges();
        }

        public bool ExisteNombre(string nombre, int idExcluir)
        {
            return db.TiposAtencion.Any(t => t.Nombre == nombre && t.Id != idExcluir);
        }

        public bool TieneAtencionesActivas(int idTipo)
        {
            return db.Atenciones.Any(a => a.IdTipoAtencion == idTipo && a.Activo == true);
        }
    }
}
