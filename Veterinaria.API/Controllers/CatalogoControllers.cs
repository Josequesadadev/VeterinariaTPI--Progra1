using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Services.Interfaces;

namespace Veterinaria.API.Controllers
{
    // Controller generico para las tablas de soporte con solo nombre.
    // Las clases de abajo solo le ponen la ruta.
    [Authorize]
    public abstract class CatalogoController<T> : BaseApiController where T : Catalogo
    {
        private readonly ICatalogoService<T> service;

        protected CatalogoController(ICatalogoService<T> service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] bool incluirInactivos = false)
        {
            return Ejecutar(() => Ok(service.GetAll(incluirInactivos)));
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            return Ejecutar(() => Ok(service.GetById(id)));
        }

        [HttpPost]
        public IActionResult Create([FromBody] CatalogoDto dto)
        {
            return Ejecutar(() =>
            {
                T nuevo = service.Crear(dto);
                return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
            });
        }

        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] CatalogoDto dto)
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

    [Route("api/tipos-mascota")]
    public class TiposMascotaController : CatalogoController<TipoMascota>
    {
        public TiposMascotaController(ICatalogoService<TipoMascota> service) : base(service) { }
    }

    [Route("api/tipos-atencion")]
    public class TiposAtencionController : CatalogoController<TipoAtencion>
    {
        public TiposAtencionController(ICatalogoService<TipoAtencion> service) : base(service) { }
    }
}
