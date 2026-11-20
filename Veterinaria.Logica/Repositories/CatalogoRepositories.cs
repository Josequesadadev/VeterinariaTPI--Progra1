using Veterinaria.Logica.Data;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories.Interfaces;

namespace Veterinaria.Logica.Repositories
{
    // Clase abstracta: el codigo comun de los catalogos esta aca,
    // y cada hijo decide que significa "tener registros activos".
    public abstract class CatalogoRepository<T> : Repository<T>, ICatalogoRepository<T> where T : Catalogo
    {
        protected CatalogoRepository(VeterinariaDbContext db) : base(db) { }

        public override List<T> GetAll(bool incluirInactivos)
        {
            return Query()
                .Where(e => incluirInactivos || e.Activo)
                .OrderBy(e => e.Nombre)
                .ToList();
        }

        public bool ExisteNombre(string nombre, int idExcluir)
        {
            return Query().Any(e => e.Nombre == nombre && e.Id != idExcluir);
        }

        public abstract bool TieneRegistrosActivos(int id);
    }

    public class TipoMascotaRepository : CatalogoRepository<TipoMascota>
    {
        public TipoMascotaRepository(VeterinariaDbContext db) : base(db) { }

        public override bool TieneRegistrosActivos(int id)
        {
            return db.Mascotas.Any(m => m.IdTipoMascota == id && m.Activo);
        }
    }

    public class TipoAtencionRepository : CatalogoRepository<TipoAtencion>
    {
        public TipoAtencionRepository(VeterinariaDbContext db) : base(db) { }

        public override bool TieneRegistrosActivos(int id)
        {
            return db.Atenciones.Any(a => a.IdTipoAtencion == id && a.Activo);
        }
    }
}
