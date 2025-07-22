using Lab_Mvc.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.Collections.Concurrent;
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
        private readonly IConfiguration _config;

        // 🔐 In-memory store to track the latest valid token per user
        private static readonly ConcurrentDictionary<string, string> UserTokenStore = new();

        public LoginController(ILogin loginrepository, IConfiguration config)
        {
            loginRepository = loginrepository;
            _config = config;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] DTOLogin login)
        {
            var user = await loginRepository.Login(login);
            if (user == null) return Unauthorized("Invalid credentials");

            var jwtSettings = _config.GetSection("Jwt");
            var tokenId = Guid.NewGuid().ToString(); // Unique token ID (jti)

            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, tokenId),
                new Claim(ClaimTypes.NameIdentifier, user.USER_ID.ToString()),
                new Claim(ClaimTypes.Name, user.NAME),
                new Claim("COM_ID", user.COM_ID.ToString()),
                new Claim("USER_LOGIN", user.USER_LOGIN)
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            // ⚠️ Token without expiry
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            // No expiry
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // 🔄 Store the new token ID and overwrite any old session
            UserTokenStore[user.USER_ID.ToString()] = tokenId;

            return Ok(new
            {
                token = tokenString,
                userDetails = new
                {
                    user.USER_ID,
                    user.NAME,
                    user.CONTACT,
                    user.COM_ID,
                    user.USER_LOGIN
                }
            });
        }

        [HttpPost("Logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
            {
                UserTokenStore.TryRemove(userId, out _); // Invalidate the token
                return Ok("Logged out successfully");
            }

            return Unauthorized();
        }

        // 🔍 Token check middleware (copy to Program.cs or Middleware folder if needed)
        public class TokenValidationMiddleware
        {
            private readonly RequestDelegate _next;

            public TokenValidationMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task Invoke(HttpContext context)
            {
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var jti = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                    if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(jti))
                    {
                        if (UserTokenStore.TryGetValue(userId, out var currentJti))
                        {
                            if (currentJti != jti)
                            {
                                context.Response.StatusCode = 401;
                                await context.Response.WriteAsync("Session expired. You logged in from another device.");
                                return;
                            }
                        }
                    }
                }

                await _next(context);
            }
        }
    }
}
