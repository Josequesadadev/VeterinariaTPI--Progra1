using Microsoft.EntityFrameworkCore;
using Veterinaria.Logica.Data;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories.Interfaces;

namespace Veterinaria.Logica.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly VeterinariaDbContext db;

        public ClienteRepository(VeterinariaDbContext db)
        {
            this.db = db;
        }

        public List<Cliente> GetAll(string? texto, bool incluirInactivos)
        {
            IQueryable<Cliente> consulta = db.Clientes;

            if (!incluirInactivos)
            {
                consulta = consulta.Where(c => c.Activo == true);
            }

            if (!string.IsNullOrEmpty(texto))
            {
                consulta = consulta.Where(c => c.Codigo.Contains(texto) || c.Nombre.Contains(texto) || c.Apellido.Contains(texto));
            }

            return consulta.OrderBy(c => c.Apellido).ThenBy(c => c.Nombre).ToList();
        }

        public Cliente? GetById(int id)
        {
            return db.Clientes.Find(id);
        }

        public void Add(Cliente cliente)
        {
            db.Clientes.Add(cliente);
            db.SaveChanges();
        }

        public void Update(Cliente cliente)
        {
            db.Entry(cliente).State = EntityState.Modified;
            db.SaveChanges();
        }

        public bool ExisteCodigo(string codigo, int idExcluir)
        {
            return db.Clientes.Any(c => c.Codigo == codigo && c.Id != idExcluir);
        }

        public bool TieneMascotasActivas(int idCliente)
        {
            return db.Mascotas.Any(m => m.IdCliente == idCliente && m.Activo == true);
        }
    }
}
