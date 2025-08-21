using Apibackend.Models;
using Apibackend.Services;
using JwtAuthDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Apibackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserService _userservice;

        public AuthController(IConfiguration config, UserService userService)
        {
            _config = config;
            _userservice = userService;
        }

        // -------------------- REGISTER --------------------
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            try
            {
                if (user == null)
                    return BadRequest("Utilisateur invalide");

                var existingUser = _userservice.GetUserByEmail(user.Email);
                if (existingUser != null)
                    return BadRequest("Un utilisateur avec cet email existe déjà");

                _userservice.AddUser(user);
                return Ok("Utilisateur enregistré avec succès");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }

        // -------------------- LOGIN --------------------
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            if (login == null || string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
            {
                return BadRequest("Requête de login invalide");
            }

            try
            {
                var user = _userservice.GetUserByEmail(login.Email);
                if (user == null)
                {
                    return Unauthorized("Utilisateur non trouvé");
                }

                // Vérifier le mot de passe hashé
                bool isPasswordValid = _userservice.VerifyPassword(user, login.Password);
                if (!isPasswordValid)
                {
                    return Unauthorized("Mot de passe incorrect");
                }

                // Générer le JWT
                var token = GenerateJwtToken(user.Email, user.Role);

                return Ok(new
                {
                    token,
                    user
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de la récupération de l'utilisateur : {ex.Message}");
            }
        }

        // -------------------- ENDPOINT PROTÉGÉ --------------------
        [HttpGet("protected")]
        [Authorize]
        public IActionResult Protected()
        {
            return Ok("Vous êtes authentifié !");
        }

        // -------------------- GENERATE JWT --------------------
        private string GenerateJwtToken(string email, string role)
        {
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
