using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Services.Interfaces;

namespace Veterinaria.API.Controllers
{
    [Authorize]
    [Route("api/clientes")]
    public class ClientesController : BaseApiController
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
            return Ejecutar(() => Ok(service.GetAll(texto, incluirInactivos)));
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            return Ejecutar(() => Ok(service.GetById(id)));
        }

        // Mascotas activas de un cliente: lo usa el formulario de atenciones (seleccion dependiente)
        [HttpGet("{id}/mascotas")]
        public IActionResult GetMascotas([FromRoute] int id)
        {
            return Ejecutar(() =>
            {
                service.GetById(id);
                return Ok(mascotaService.GetAll(id, null, null, false));
            });
        }

        [HttpPost]
        public IActionResult Create([FromBody] ClienteDto dto)
        {
            return Ejecutar(() =>
            {
                Cliente nuevo = service.Crear(dto);
                return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] ClienteDto dto)
        {
            return Ejecutar(() => Ok(service.Modificar(id, dto)));
        }

        // Baja logica: solo el administrador
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
