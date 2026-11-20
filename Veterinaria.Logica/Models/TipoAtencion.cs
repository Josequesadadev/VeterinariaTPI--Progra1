using System.Text.Json.Serialization;
using Veterinaria.Logica.Exceptions;

namespace Veterinaria.Logica.Models
{
    public class TipoAtencion : Entidad
    {
        public string Nombre { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual ICollection<Atencion> Atenciones { get; set; } = new List<Atencion>();

        public TipoAtencion()
        {
        }

        public TipoAtencion(int id, string nombre)
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
