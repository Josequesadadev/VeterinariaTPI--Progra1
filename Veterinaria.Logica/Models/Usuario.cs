using System.Text.Json.Serialization;
using Veterinaria.Logica.Exceptions;

namespace Veterinaria.Logica.Models
{
    public class Usuario : Entidad
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;

        public Rol Rol { get; set; } = Rol.Empleado;
        public DateTime FechaAlta { get; set; } = DateTime.Now;

        public Usuario()
        {
        }

        public Usuario(int id, string nombreUsuario, string email, Rol rol)
        {
            Id = id;
            NombreUsuario = nombreUsuario;
            Email = email;
            Rol = rol;
        }

        public bool EsAdmin()
        {
            return Rol == Rol.Admin;
        }

        public override void Validar()
        {
            if (string.IsNullOrWhiteSpace(NombreUsuario))
            {
                throw new NegocioException("El nombre de usuario es obligatorio.");
            }
            if (!Email.Contains('@'))
            {
                throw new NegocioException("El email no es valido.");
            }
        }

        public override string ToString()
        {
            return NombreUsuario + " (" + Rol + ")";
        }
    }
}
