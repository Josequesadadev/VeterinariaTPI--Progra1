namespace Veterinaria.Logica.Dtos
{
    public class AtencionDto
    {
        public int IdCliente { get; set; }
        public int IdMascota { get; set; }
        public int IdTipoAtencion { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Importe { get; set; }
    }
}
