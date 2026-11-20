namespace Veterinaria.Logica.Dtos
{
    public class MascotaDto
    {
        public string Nombre { get; set; } = string.Empty;
        public int Edad { get; set; }
        public int IdTipoMascota { get; set; }
        public int IdCliente { get; set; }
    }
}
