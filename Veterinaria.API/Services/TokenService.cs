using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Models;

namespace Veterinaria.API.Services
{
    // Genera el token JWT que la web guarda y manda en cada pedido (header Authorization: Bearer ...)
    public class TokenService
    {
        private readonly IConfiguration config;

        public TokenService(IConfiguration config)
        {
            this.config = config;
        }

        public LoginResponseDto GenerarToken(Usuario usuario)
        {
            DateTime expira = DateTime.UtcNow.AddMinutes(config.GetValue<int>("Jwt:ExpiraMinutos"));

            // Los "claims" son los datos del usuario que viajan dentro del token
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString())
            };

            SymmetricSecurityKey clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            SigningCredentials firma = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: expira,
                signingCredentials: firma);

            return new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol.ToString(),
                Expira = expira
            };
        }
    }
}
