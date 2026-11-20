using Microsoft.AspNetCore.Mvc;
using Veterinaria.API.Services;
using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Models;
using Veterinaria.Logica.Services.Interfaces;

namespace Veterinaria.API.Controllers
{
    [Route("api/auth")]
    public class AuthController : BaseApiController
    {
        private readonly IAuthService authService;
        private readonly TokenService tokenService;

        public AuthController(IAuthService authService, TokenService tokenService)
        {
            this.authService = authService;
            this.tokenService = tokenService;
        }

        // POST api/auth/login  -> si las credenciales son correctas devuelve el token JWT
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto dto)
        {
            return Ejecutar(() =>
            {
                Usuario? usuario = authService.ValidarCredenciales(dto);
                if (usuario == null)
                {
                    return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos." });
                }
                return Ok(tokenService.GenerarToken(usuario));
            });
        }
    }
}
