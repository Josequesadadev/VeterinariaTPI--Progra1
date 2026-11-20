namespace Veterinaria.Logica.Dtos
{
    // DTO = objetos simples que viajan entre la web y la API (lo que manda o recibe el formulario)

    public class LoginRequestDto
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public DateTime Expira { get; set; }
    }

    public class CatalogoDto
    {
        public string Nombre { get; set; } = string.Empty;
    }

    public class ClienteDto
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty;
        public string? Telefono { get; set; }
    }

    public class MascotaDto
    {
        public string Nombre { get; set; } = string.Empty;
        public int Edad { get; set; }
        public int IdTipoMascota { get; set; }
        public int IdCliente { get; set; }
    }

    public class AtencionDto
    {
        public int IdCliente { get; set; }
        public int IdMascota { get; set; }
        public int IdTipoAtencion { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Importe { get; set; }
    }

    public class AtencionFiltroDto
    {
        public int? IdCliente { get; set; }
        public int? IdMascota { get; set; }
        public int? IdTipoAtencion { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public bool IncluirInactivas { get; set; }
    }

    // ===== Dashboards =====

    public class FacturacionMensualDto
    {
        public int Anio { get; set; }
        public int Mes { get; set; }
        public string Etiqueta { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
    }

    public class AtencionesPorTipoDto
    {
        public string Tipo { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Importe { get; set; }
    }

    public class TopClienteDto
    {
        public int IdCliente { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal Importe { get; set; }
    }
}
