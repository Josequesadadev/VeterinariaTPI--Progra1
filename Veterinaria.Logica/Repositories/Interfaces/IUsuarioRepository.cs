using Veterinaria.Logica.Models;

namespace Veterinaria.Logica.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Usuario? GetByNombreUsuario(string nombreUsuario);
    }
}
