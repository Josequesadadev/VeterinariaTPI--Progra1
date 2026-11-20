namespace Veterinaria.Logica.Dtos
{
    public class TopClienteDto
    {
        public int IdCliente { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Importe { get; set; }
    }
}
