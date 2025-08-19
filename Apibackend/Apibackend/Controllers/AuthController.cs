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
        public readonly UserService _userservice;

        public AuthController(IConfiguration config, UserService userService)
        {
            _config = config;
            _userservice = userService;
        }

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
                // Log l'exception ici si nécessaire
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            if (login == null || string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
            {
                return BadRequest("Invalid login request");
            }
            try
            {
                User user = _userservice.GetUserByEmail(login.Email);
                if (user == null)
                {
                    return Unauthorized("User not found");
                }
                if (login.Email == user.Email && login.Password == user.Password)
                {
                    var currentUser = _userservice.GetUserById(user.Id);
                    var token = GenerateJwtToken(login.Email);
                    return Ok(new
                    {
                        token,
                        user = currentUser
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving user: {ex.Message}");
            }

            return Unauthorized();
        }

        [HttpGet("protected")]
        [Authorize]
        public IActionResult Protected()
        {
            try
            {
                return Ok("Vous êtes authentifié !");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur interne : {ex.Message}");
            }
        }

        private string GenerateJwtToken(string username)
        {
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin")
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
