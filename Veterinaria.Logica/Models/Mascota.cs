using System.Text.Json.Serialization;
using Veterinaria.Logica.Exceptions;

namespace Veterinaria.Logica.Models
{
    public class Mascota : Entidad
    {
        public string Nombre { get; set; } = string.Empty;
        public int Edad { get; set; }
        public int IdTipoMascota { get; set; }
        public int IdCliente { get; set; }

        public virtual TipoMascota? TipoMascota { get; set; }
        public virtual Cliente? Cliente { get; set; }

        [JsonIgnore]
        public virtual ICollection<Atencion> Atenciones { get; set; } = new List<Atencion>();

        public Mascota()
        {
        }

        public Mascota(int id, string nombre, int edad, int idTipoMascota, int idCliente)
        {
            Id = id;
            Nombre = nombre;
            Edad = edad;
            IdTipoMascota = idTipoMascota;
            IdCliente = idCliente;
        }

        public bool PerteneceA(int idCliente)
        {
            return IdCliente == idCliente;
        }

        public override void Validar()
        {
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                throw new NegocioException("El nombre de la mascota es obligatorio.");
            }
            if (Edad < 0)
            {
                throw new NegocioException("La edad no puede ser negativa.");
            }
            if (IdTipoMascota <= 0 || IdCliente <= 0)
            {
                throw new NegocioException("Tiene que elegir el tipo y el cliente.");
            }
        }

        public override string ToString()
        {
            return Nombre + " (" + Edad + " años)";
        }
    }
}
