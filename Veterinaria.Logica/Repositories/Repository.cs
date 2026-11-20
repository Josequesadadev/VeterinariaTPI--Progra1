using Microsoft.EntityFrameworkCore;
using Veterinaria.Logica.Data;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories.Interfaces;

namespace Veterinaria.Logica.Repositories
{
    // Implementacion generica del patron Repository.
    // Las clases hijas reutilizan este codigo y solo agregan lo propio.
    public class Repository<T> : IRepository<T> where T : Entidad
    {
        protected readonly VeterinariaDbContext db;

        public Repository(VeterinariaDbContext db)
        {
            this.db = db;
        }

        // Consulta base. Las clases hijas la sobrescriben para incluir relaciones (Include).
        protected virtual IQueryable<T> Query()
        {
            return db.Set<T>();
        }

        public virtual List<T> GetAll(bool incluirInactivos)
        {
            return Query()
                .Where(e => incluirInactivos || e.Activo)
                .OrderBy(e => e.Id)
                .ToList();
        }

        public virtual T? GetById(int id)
        {
            return Query().FirstOrDefault(e => e.Id == id);
        }

        public void Add(T entidad)
        {
            db.Set<T>().Add(entidad);
            db.SaveChanges();
        }

        public void Update(T entidad)
        {
            // Si la entidad vino de GetById ya esta "trackeada" por EF y alcanza con SaveChanges
            if (db.Entry(entidad).State == EntityState.Detached)
            {
                db.Set<T>().Update(entidad);
            }
            db.SaveChanges();
        }
    }
}
