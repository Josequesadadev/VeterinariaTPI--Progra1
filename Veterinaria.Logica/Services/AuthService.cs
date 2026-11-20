using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Repositories.Interfaces;
using Veterinaria.Logica.Seguridad;
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

        // Devuelve el usuario si las credenciales son correctas, o null si no
        public Usuario? ValidarCredenciales(LoginRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreUsuario) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return null;
            }

            Usuario? usuario = repo.GetByNombreUsuario(dto.NombreUsuario.Trim());
            if (usuario == null || !usuario.Activo)
            {
                return null;
            }

            return PasswordHasher.Verificar(dto.Password, usuario.PasswordHash) ? usuario : null;
        }

        // Si la tabla Usuarios esta vacia, crea un administrador y un empleado para poder ingresar
        public void CrearUsuariosIniciales()
        {
            if (repo.HayUsuarios())
            {
                return;
            }

            Usuario admin = new Usuario(0, "admin", "admin@veterinaria.com", Rol.Admin);
            admin.PasswordHash = PasswordHasher.Hashear("Admin123");
            repo.Add(admin);

            Usuario empleado = new Usuario(0, "empleado", "empleado@veterinaria.com", Rol.Empleado);
            empleado.PasswordHash = PasswordHasher.Hashear("Empleado123");
            repo.Add(empleado);
        }
    }
}
