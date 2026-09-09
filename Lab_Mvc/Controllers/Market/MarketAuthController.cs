using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Lab_Mvc.Interfaces.Market;
using Models.Market;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Lab_Mvc.Controllers.Market
{
    [ApiController]
    [Route("api/auth")]
    public class MarketAuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public MarketAuthController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { Message = "Username and password are required." });
            }

            bool isValid = await _userRepository.ValidateUserAsync(request.Username, request.Password);
            if (!isValid)
            {
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null)
            {
                return Unauthorized();
            }

            var token = GenerateJwtToken(user);
            return Ok(new LoginResponse
            {
                Token = token,
                Username = user.Username,
                Role = user.Role
            });
        }

        private string GenerateJwtToken(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSettings = _configuration.GetSection("Jwt");
            var keyStr = jwtSettings["Key"] ?? "SuperSecretKeyForVegBookApp123456789!";
            var key = Encoding.ASCII.GetBytes(keyStr);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("module", "Market")
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryInMinutes"] ?? "1440")),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
