using Microsoft.AspNetCore.Mvc;
using Veterinaria.API.Services;
using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Services.Interfaces;

namespace Veterinaria.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService service;
        private readonly TokenService tokenService;

        public AuthController(IAuthService service, TokenService tokenService)
        {
            this.service = service;
            this.tokenService = tokenService;
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            try
            {
                Usuario? usuario = service.Login(dto);

                if (usuario == null)
                {
                    return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos." });
                }

                return Ok(tokenService.GenerarToken(usuario));
            }
            catch (Exception)
            {
                return StatusCode(500, new { mensaje = "Error en el servidor." });
            }
        }
    }
}
