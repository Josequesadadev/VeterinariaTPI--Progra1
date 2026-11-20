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

        public Cliente() { }

        public Cliente(int id, string codigo, string nombre, string apellido, Sexo sexo, string? telefono)
        {
            Id = id;
            Codigo = codigo;
            Nombre = nombre;
            Apellido = apellido;
            Sexo = sexo;
            Telefono = telefono;
        }

        public string NombreCompleto => $"{Apellido}, {Nombre}";

        public override void Validar()
        {
            if (string.IsNullOrWhiteSpace(Codigo))
            {
                throw new NegocioException("El código es obligatorio.");
            }
            if (Codigo.Trim().Length > 10)
            {
                throw new NegocioException("El código no puede superar los 10 caracteres.");
            }
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Apellido))
            {
                throw new NegocioException("El nombre y el apellido son obligatorios.");
            }
            if (Nombre.Trim().Length > 50 || Apellido.Trim().Length > 50)
            {
                throw new NegocioException("El nombre y el apellido no pueden superar los 50 caracteres.");
            }
            if (!Enum.IsDefined(typeof(Sexo), Sexo))
            {
                throw new NegocioException("El sexo indicado no es válido.");
            }
            // Telefono opcional, pero si se carga: solo numeros, espacios, guiones o + (entre 6 y 20 caracteres)
            if (!string.IsNullOrWhiteSpace(Telefono) && !Regex.IsMatch(Telefono.Trim(), @"^\+?[0-9\- ]{6,20}$"))
            {
                throw new NegocioException("El teléfono solo puede tener números, espacios o guiones (entre 6 y 20 caracteres).");
            }
        }

        public override string ToString()
        {
            return $"{Codigo} - {NombreCompleto}";
        }
    }
}
