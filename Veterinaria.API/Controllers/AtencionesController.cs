using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Services.Interfaces;

namespace Veterinaria.API.Controllers
{
    // Transaccion principal: Registro de Atenciones
    [Authorize]
    [Route("api/atenciones")]
    public class AtencionesController : BaseApiController
    {
        private readonly IAtencionService service;

        public AtencionesController(IAtencionService service)
        {
            this.service = service;
        }

        // GET api/atenciones?idCliente=1&idTipoAtencion=3&desde=2026-01-01&hasta=2026-06-30
        [HttpGet]
        public IActionResult GetAll([FromQuery] AtencionFiltroDto filtro)
        {
            return Ejecutar(() => Ok(service.GetAll(filtro)));
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            return Ejecutar(() => Ok(service.GetById(id)));
        }

        [HttpPost]
        public IActionResult Create([FromBody] AtencionDto dto)
        {
            return Ejecutar(() =>
            {
                Atencion nueva = service.Crear(dto);
                return CreatedAtAction(nameof(GetById), new { id = nueva.Id }, nueva);
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] AtencionDto dto)
        {
            return Ejecutar(() => Ok(service.Modificar(id, dto)));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            return Ejecutar(() =>
            {
                service.DarDeBaja(id);
                return NoContent();
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/reactivar")]
        public IActionResult Reactivar([FromRoute] int id)
        {
            return Ejecutar(() =>
            {
                service.Reactivar(id);
                return NoContent();
            });
        }
    }
}
