using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veterinaria.Logica.Exceptions;
using Veterinaria.Logica.Services.Interfaces;

namespace Veterinaria.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService service;

        public DashboardController(IDashboardService service)
        {
            this.service = service;
        }

        [HttpGet]
        [Route("facturacion-mensual")]
        public IActionResult FacturacionMensual([FromQuery] int? anio)
        {
            try
            {
                return Ok(service.GetFacturacionMensual(anio));
            }
            catch (Exception)
            {
                return StatusCode(500, new { mensaje = "Error en el servidor." });
            }
        }

        [HttpGet]
        [Route("por-tipo")]
        public IActionResult PorTipo([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
        {
            try
            {
                return Ok(service.GetCantidadPorTipo(desde, hasta));
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { mensaje = "Error en el servidor." });
            }
        }

        [HttpGet]
        [Route("top-clientes")]
        public IActionResult TopClientes([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
        {
            try
            {
                return Ok(service.GetTopClientes(desde, hasta));
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { mensaje = "Error en el servidor." });
            }
        }
    }
}
