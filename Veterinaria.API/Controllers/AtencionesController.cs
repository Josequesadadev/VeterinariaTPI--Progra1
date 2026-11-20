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
    [Route("api/atenciones")]
    public class AtencionesController : ControllerBase
    {
        private readonly IAtencionService service;

        public AtencionesController(IAtencionService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] FiltroAtencionDto filtro)
        {
            try
            {
                return Ok(service.GetAll(filtro));
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
        [Route("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            try
            {
                Atencion? atencion = service.GetById(id);
                if (atencion == null)
                {
                    return NotFound(new { mensaje = "No existe la atencion." });
                }
                return Ok(atencion);
            }
            catch (Exception)
            {
                return StatusCode(500, new { mensaje = "Error en el servidor." });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] AtencionDto dto)
        {
            try
            {
                Atencion nueva = service.Crear(dto);
                return Created("api/atenciones/" + nueva.Id, nueva);
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
        public IActionResult Update([FromRoute] int id, [FromBody] AtencionDto dto)
        {
            try
            {
                Atencion? atencion = service.Modificar(id, dto);
                if (atencion == null)
                {
                    return NotFound(new { mensaje = "No existe la atencion." });
                }
                return Ok(atencion);
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
        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                if (!service.DarDeBaja(id))
                {
                    return NotFound(new { mensaje = "No existe la atencion." });
                }
                return NoContent();
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
                    return NotFound(new { mensaje = "No existe la atencion." });
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
