using System.Text.Json.Serialization;

namespace Veterinaria.Logica.Models
{
    public class TipoAtencion : Catalogo
    {
        [JsonIgnore]
        public virtual ICollection<Atencion> Atenciones { get; set; } = new List<Atencion>();

        public TipoAtencion() { }

        public TipoAtencion(int id, string nombre) : base(id, nombre) { }
    }
}
