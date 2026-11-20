using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veterinaria.Logica.Services.Interfaces;

namespace Veterinaria.API.Controllers
{
    [Authorize]
    [Route("api/dashboard")]
    public class DashboardController : BaseApiController
    {
        private readonly IDashboardService service;

        public DashboardController(IDashboardService service)
        {
            this.service = service;
        }

        // Facturacion mensual: ultimos 12 meses, o los 12 meses del año indicado
        [HttpGet("facturacion-mensual")]
        public IActionResult FacturacionMensual([FromQuery] int? anio)
        {
            return Ejecutar(() => Ok(service.GetFacturacionMensual(anio)));
        }

        // Cantidad e importe por tipo de atencion, con rango de fechas opcional
        [HttpGet("por-tipo")]
        public IActionResult PorTipo([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
        {
            return Ejecutar(() => Ok(service.GetCantidadPorTipo(desde, hasta)));
        }

        // Top 5 de clientes con mas atenciones, con rango de fechas opcional
        [HttpGet("top-clientes")]
        public IActionResult TopClientes([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
        {
            return Ejecutar(() => Ok(service.GetTopClientes(desde, hasta)));
        }
    }
}
