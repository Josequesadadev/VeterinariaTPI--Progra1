using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Services.Interfaces;

namespace Veterinaria.API.Controllers
{
    [Authorize]
    [Route("api/mascotas")]
    public class MascotasController : BaseApiController
    {
        private readonly IMascotaService service;

        public MascotasController(IMascotaService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] int? idCliente, [FromQuery] int? idTipoMascota,
                                    [FromQuery] string? nombre, [FromQuery] bool incluirInactivos = false)
        {
            return Ejecutar(() => Ok(service.GetAll(idCliente, idTipoMascota, nombre, incluirInactivos)));
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            return Ejecutar(() => Ok(service.GetById(id)));
        }

        [HttpPost]
        public IActionResult Create([FromBody] MascotaDto dto)
        {
            return Ejecutar(() =>
            {
                Mascota nueva = service.Crear(dto);
                return CreatedAtAction(nameof(GetById), new { id = nueva.Id }, nueva);
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] MascotaDto dto)
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
