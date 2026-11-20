using Microsoft.AspNetCore.Mvc;
using Veterinaria.Logica.Exceptions;

namespace Veterinaria.API.Controllers
{
    // Controller base: centraliza el manejo de errores para no repetir el try/catch en cada accion
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected IActionResult Ejecutar(Func<IActionResult> accion)
        {
            try
            {
                return accion();
            }
            catch (NoEncontradoException ex)
            {
                return NotFound(new { mensaje = ex.Message });          // 404
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { mensaje = ex.Message });        // 400
            }
            catch (Exception)
            {
                return StatusCode(500, new { mensaje = "Ocurrió un error inesperado en el servidor." });
            }
        }
    }
}
