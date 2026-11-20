using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Models;

namespace Veterinaria.Logica.Services.Interfaces
{
    public interface IAuthService
    {
        Usuario? Login(LoginDto dto);
    }
}
