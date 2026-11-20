using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Exceptions;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Services.Interfaces;

namespace Veterinaria.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/clientes")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService service;
        private readonly IMascotaService mascotaService;

        public ClientesController(IClienteService service, IMascotaService mascotaService)
        {
            this.service = service;
            this.mascotaService = mascotaService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string? texto, [FromQuery] bool incluirInactivos = false)
        {
            try
            {
                return Ok(service.GetAll(texto, incluirInactivos));
            }
            catch (Exception)
            {
                return StatusCode(500, new { mensaje = "Error en el servidor." });
            }
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            try
            {
                Cliente? cliente = service.GetById(id);
                if (cliente == null)
                {
                    return NotFound(new { mensaje = "No existe el cliente." });
                }
                return Ok(cliente);
            }
            catch (Exception)
            {
                return StatusCode(500, new { mensaje = "Error en el servidor." });
            }
        }

        // mascotas activas del cliente (para el combo de atenciones)
        [HttpGet]
        [Route("{id}/mascotas")]
        public IActionResult GetMascotas([FromRoute] int id)
        {
            try
            {
                if (service.GetById(id) == null)
                {
                    return NotFound(new { mensaje = "No existe el cliente." });
                }
                return Ok(mascotaService.GetAll(id, null, null, false));
            }
            catch (Exception)
            {
                return StatusCode(500, new { mensaje = "Error en el servidor." });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] ClienteDto dto)
        {
            try
            {
                Cliente nuevo = service.Crear(dto);
                return Created("api/clientes/" + nuevo.Id, nuevo);
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

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] ClienteDto dto)
        {
            try
            {
                Cliente? cliente = service.Modificar(id, dto);
                if (cliente == null)
                {
                    return NotFound(new { mensaje = "No existe el cliente." });
                }
                return Ok(cliente);
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

        // baja logica, solo el administrador
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                if (!service.DarDeBaja(id))
                {
                    return NotFound(new { mensaje = "No existe el cliente." });
                }
                return NoContent();
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

        [Authorize(Roles = "Admin")]
        [HttpPatch]
        [Route("{id}/reactivar")]
        public IActionResult Reactivar([FromRoute] int id)
        {
            try
            {
                if (!service.Reactivar(id))
                {
                    return NotFound(new { mensaje = "No existe el cliente." });
                }
                return NoContent();
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
