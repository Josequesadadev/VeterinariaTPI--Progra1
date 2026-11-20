using Microsoft.EntityFrameworkCore;
using Veterinaria.Logica.Data;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories.Interfaces;

namespace Veterinaria.Logica.Repositories
{
    public class MascotaRepository : IMascotaRepository
    {
        private readonly VeterinariaDbContext db;

        public MascotaRepository(VeterinariaDbContext db)
        {
            this.db = db;
        }

        public List<Mascota> GetAll(int? idCliente, int? idTipoMascota, string? nombre, bool incluirInactivos)
        {
            IQueryable<Mascota> consulta = db.Mascotas.Include(m => m.Cliente).Include(m => m.TipoMascota);

            if (!incluirInactivos)
            {
                consulta = consulta.Where(m => m.Activo == true);
            }
            if (idCliente != null)
            {
                consulta = consulta.Where(m => m.IdCliente == idCliente);
            }
            if (idTipoMascota != null)
            {
                consulta = consulta.Where(m => m.IdTipoMascota == idTipoMascota);
            }
            if (!string.IsNullOrEmpty(nombre))
            {
                consulta = consulta.Where(m => m.Nombre.Contains(nombre));
            }

            return consulta.OrderBy(m => m.Nombre).ToList();
        }

        public Mascota? GetById(int id)
        {
            return db.Mascotas.Include(m => m.Cliente).Include(m => m.TipoMascota).FirstOrDefault(m => m.Id == id);
        }

        public void Add(Mascota mascota)
        {
            db.Mascotas.Add(mascota);
            db.SaveChanges();
        }

        public void Update(Mascota mascota)
        {
            db.Entry(mascota).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
