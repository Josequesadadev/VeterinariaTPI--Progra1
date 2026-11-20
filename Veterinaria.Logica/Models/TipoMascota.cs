using System.Text.Json.Serialization;

namespace Veterinaria.Logica.Models
{
    public class TipoMascota : Catalogo
    {
        [JsonIgnore]
        public virtual ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();

        public TipoMascota() { }

        public TipoMascota(int id, string nombre) : base(id, nombre) { }
    }
}
