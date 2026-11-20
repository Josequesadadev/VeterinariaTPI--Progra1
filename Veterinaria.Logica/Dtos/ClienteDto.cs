namespace Veterinaria.Logica.Dtos
{
    public class ClienteDto
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty;
        public string? Telefono { get; set; }
    }
}
