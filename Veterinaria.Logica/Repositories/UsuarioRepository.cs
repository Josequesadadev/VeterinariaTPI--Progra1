using Veterinaria.Logica.Data;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories.Interfaces;

namespace Veterinaria.Logica.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(VeterinariaDbContext db) : base(db) { }

        public Usuario? GetByNombreUsuario(string nombreUsuario)
        {
            return db.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);
        }

        public bool HayUsuarios()
        {
            return db.Usuarios.Any();
        }
    }
}
