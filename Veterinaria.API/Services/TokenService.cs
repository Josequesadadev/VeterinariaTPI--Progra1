using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Veterinaria.Logica.Dtos;
using Veterinaria.Logica.Models;

namespace Veterinaria.API.Services
{
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

            // datos del usuario que van dentro del token
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, usuario.NombreUsuario));
            claims.Add(new Claim(ClaimTypes.Role, usuario.Rol.ToString()));

            SymmetricSecurityKey clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            SigningCredentials firma = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: expira,
                signingCredentials: firma);

            LoginResponseDto respuesta = new LoginResponseDto();
            respuesta.Token = new JwtSecurityTokenHandler().WriteToken(token);
            respuesta.NombreUsuario = usuario.NombreUsuario;
            respuesta.Rol = usuario.Rol.ToString();
            respuesta.Expira = expira;
            return respuesta;
        }
    }
}
