using System.Security.Cryptography;
using System.Text;
using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories.Interfaces;
using Veterinaria.Logica.Services.Interfaces;

namespace Veterinaria.Logica.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository repo;

        public AuthService(IUsuarioRepository repo)
        {
            this.repo = repo;
        }

        // devuelve el usuario si los datos son correctos, si no devuelve null
        public Usuario? Login(LoginDto dto)
        {
            Usuario? usuario = repo.GetByNombreUsuario(dto.NombreUsuario);

            if (usuario == null || usuario.Activo == false)
            {
                return null;
            }

            if (usuario.PasswordHash != HashPassword(dto.Password))
            {
                return null;
            }

            return usuario;
        }

        // la contraseña se guarda como hash SHA256, nunca en texto plano
        public static string HashPassword(string password)
        {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }
    }
}
