using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PruebaTecnica.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class AuthController : ControllerBase
    {
        private const string UsuarioValido = "loreanny";
        private const string ContrasenaValida = "12345678";

        [HttpPost("login")] 
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Validaciones de entrada
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "El usuario y contraseña son requeridos." });
            }

            // Validación de credenciales 
            if (request.Username.ToLower() == UsuarioValido && request.Password == ContrasenaValida)
            {
                // Generar el token de acceso JWT
                var token = GenerarTokenJwt(request.Username);

                // Retornar la respuesta que el frontend en Vue.js está esperando
                return Ok(new LoginResponse
                {
                    Token = token,
                    Username = request.Username
                });
            }

            // Credenciales incorrectas
            return Unauthorized(new { message = "Nombre de usuario o contraseña incorrectos." });
        }

        // Método auxiliar para generar la firma del JWT
        private string GenerarTokenJwt(string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Administrador")
            };

            var claveSecretaBytes = Encoding.UTF8.GetBytes("clave_super_secreta_12345");
            var key = new SymmetricSecurityKey(claveSecretaBytes);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "SistemaVentas",
                audience: "SistemaVentasClientes",
                claims: claims,
                expires: DateTime.Now.AddHours(8), // El token durará 8 horas activo
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // --- Clases DTO para recibir y enviar los datos en formato JSON ---

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}