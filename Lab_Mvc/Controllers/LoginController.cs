using Dapper;
using Lab_Mvc.Constants;
using Lab_Mvc.Interfaces;
using Lab_Mvc.Repositries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Lab_Mvc.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly ILogin loginRepository;

        public LoginController(ILogin loginrepository)
        {
            this.loginRepository = loginrepository;
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] DTOLogin login)
        {
            var user = await loginRepository.Login(login);
            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            var jwtSettings = this.HttpContext.RequestServices.GetRequiredService<IConfiguration>().GetSection("Jwt");

            var authClaims = new[]
            {
        new Claim(user.CONTACT, user.PASSWORD),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

    }
}
