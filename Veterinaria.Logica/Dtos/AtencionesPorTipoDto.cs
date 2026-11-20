namespace Veterinaria.Logica.Dtos
{
    public class AtencionesPorTipoDto
    {
        public string Tipo { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Importe { get; set; }
    }
}
