using Veterinaria.Logica.Exceptions;

namespace Veterinaria.Logica.Models
{
    public class Atencion : Entidad
    {
        public int IdMascota { get; set; }
        public int IdTipoAtencion { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Importe { get; set; }

        public virtual Mascota? Mascota { get; set; }
        public virtual TipoAtencion? TipoAtencion { get; set; }

        public Atencion() { }

        public Atencion(int id, int idMascota, int idTipoAtencion, DateTime fecha, string descripcion, decimal importe)
        {
            Id = id;
            IdMascota = idMascota;
            IdTipoAtencion = idTipoAtencion;
            Fecha = fecha;
            Descripcion = descripcion;
            Importe = importe;
        }

        public override void Validar()
        {
            if (IdMascota <= 0)
            {
                throw new NegocioException("Debe seleccionar la mascota.");
            }
            if (IdTipoAtencion <= 0)
            {
                throw new NegocioException("Debe seleccionar el tipo de atención.");
            }
            if (string.IsNullOrWhiteSpace(Descripcion))
            {
                throw new NegocioException("La descripción es obligatoria.");
            }
            if (Descripcion.Trim().Length > 255)
            {
                throw new NegocioException("La descripción no puede superar los 255 caracteres.");
            }
            if (Importe <= 0)
            {
                throw new NegocioException("El importe debe ser mayor a 0.");
            }
            if (Fecha.Date > DateTime.Today)
            {
                throw new NegocioException("La fecha de la atención no puede ser futura.");
            }
        }

        public override string ToString()
        {
            return $"{Fecha:dd/MM/yyyy} - {Descripcion} - ${Importe:N2}";
        }
    }
}
