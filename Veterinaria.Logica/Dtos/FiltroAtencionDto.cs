namespace Veterinaria.Logica.Dtos
{
    public class FiltroAtencionDto
    {
        public int? IdCliente { get; set; }
        public int? IdMascota { get; set; }
        public int? IdTipoAtencion { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public bool IncluirInactivas { get; set; }
    }
}
