using System.Text.Json.Serialization;
using Veterinaria.Logica.Exceptions;

namespace Veterinaria.Logica.Models
{
    public class TipoMascota : Entidad
    {
        public string Nombre { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();

        public TipoMascota()
        {
        }

        public TipoMascota(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }

        public override void Validar()
        {
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                throw new NegocioException("El nombre es obligatorio.");
            }
            if (Nombre.Length > 50)
            {
                throw new NegocioException("El nombre no puede tener mas de 50 caracteres.");
            }
        }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
