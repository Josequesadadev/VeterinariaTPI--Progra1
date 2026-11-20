using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Veterinaria.Logica.Exceptions;

namespace Veterinaria.Logica.Models
{
    public class Cliente : Entidad
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public Sexo Sexo { get; set; }
        public string? Telefono { get; set; }

        [JsonIgnore]
        public virtual ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();

        public Cliente()
        {
        }

        public Cliente(int id, string codigo, string nombre, string apellido, Sexo sexo, string? telefono)
        {
            Id = id;
            Codigo = codigo;
            Nombre = nombre;
            Apellido = apellido;
            Sexo = sexo;
            Telefono = telefono;
        }

        public override void Validar()
        {
            if (string.IsNullOrWhiteSpace(Codigo))
            {
                throw new NegocioException("El codigo es obligatorio.");
            }
            if (Codigo.Length > 10)
            {
                throw new NegocioException("El codigo no puede tener mas de 10 caracteres.");
            }
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellido))
            {
                throw new NegocioException("El nombre y el apellido son obligatorios.");
            }
            // el telefono no es obligatorio, pero si se carga tiene que ser valido
            if (!string.IsNullOrWhiteSpace(Telefono) && !Regex.IsMatch(Telefono, @"^[0-9\- ]{6,20}$"))
            {
                throw new NegocioException("El telefono solo puede tener numeros y guiones (entre 6 y 20 caracteres).");
            }
        }

        public override string ToString()
        {
            return Codigo + " - " + Apellido + ", " + Nombre;
        }
    }
}
