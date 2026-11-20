using Veterinaria.Logica.Data;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories.Interfaces;

namespace Veterinaria.Logica.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly VeterinariaDbContext db;

        public UsuarioRepository(VeterinariaDbContext db)
        {
            this.db = db;
        }

        public Usuario? GetByNombreUsuario(string nombreUsuario)
        {
            return db.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);
        }
    }
}
