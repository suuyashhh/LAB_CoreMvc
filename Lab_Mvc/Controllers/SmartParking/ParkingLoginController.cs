using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models;
using SmartParking.Interfaces;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartParking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParkingLoginController : ControllerBase
    {
        private readonly IParkingLogin _parkingLogin;
        private readonly IConfiguration _config;

        public static readonly ConcurrentDictionary<string, string> ParkingUserTokenStore = new();

        public ParkingLoginController(IParkingLogin parkingLogin, IConfiguration config)
        {
            _parkingLogin = parkingLogin;
            _config = config;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] DTOParkingLogin loginRequest)
        {
            var user = await _parkingLogin.Login(loginRequest.PHONE ?? "", loginRequest.PASS ?? "");
            if (user == null || user.USERID == null)
            {
                return Unauthorized(new { message = "Invalid phone or password" });
            }

            var jwtSettings = _config.GetSection("Jwt");
            var tokenId = Guid.NewGuid().ToString();

            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, tokenId),
                new Claim(ClaimTypes.NameIdentifier, user.USERID.Value.ToString()),
                new Claim(ClaimTypes.Name, user.NAME ?? string.Empty),
                new Claim("module", "SmartParking"),
                new Claim("parking_user_id", user.USERID.Value.ToString()),
                new Claim("parking_phone", user.PHONE ?? string.Empty),
                new Claim("parking_email", user.EMAIL ?? string.Empty)
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            ParkingUserTokenStore[user.USERID.Value.ToString()] = tokenId;

            return Ok(new
            {
                token = tokenString,
                user = new
                {
                    userId = user.USERID,
                    name = user.NAME,
                    email = user.EMAIL,
                    phone = user.PHONE,
                    role = "Parking Provider"
                }
            });
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("validate")]
        public IActionResult Validate()
        {
            var parkingUserId = GetAuthenticatedParkingUserId();
            if (parkingUserId == null)
            {
                return Unauthorized(new { message = "SmartParking login required." });
            }

            return Ok(new
            {
                user = new
                {
                    userId = parkingUserId,
                    name = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
                    email = User.FindFirst("parking_email")?.Value ?? string.Empty,
                    phone = User.FindFirst("parking_phone")?.Value ?? string.Empty,
                    role = "Parking Provider"
                }
            });
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var parkingUserId = GetAuthenticatedParkingUserId();
            if (parkingUserId == null)
            {
                return Unauthorized(new { message = "SmartParking login required." });
            }

            ParkingUserTokenStore.TryRemove(parkingUserId.Value.ToString(), out _);
            return Ok(new { message = "Logged out successfully" });
        }

        private int? GetAuthenticatedParkingUserId()
        {
            var module = User.FindFirst("module")?.Value;
            if (!string.Equals(module, "SmartParking", StringComparison.Ordinal))
            {
                return null;
            }

            var claimValue =
                User.FindFirst("parking_user_id")?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(claimValue, out var userId) ? userId : null;
        }

        public class ParkingTokenValidationMiddleware
        {
            private readonly RequestDelegate _next;

            public ParkingTokenValidationMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task Invoke(HttpContext context)
            {
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    var module = context.User.FindFirst("module")?.Value;
                    if (string.Equals(module, "SmartParking", StringComparison.Ordinal))
                    {
                        var userId =
                            context.User.FindFirst("parking_user_id")?.Value ??
                            context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                        var jti = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(jti))
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync("{\"message\":\"Invalid SmartParking session.\"}");
                            return;
                        }

                        if (!ParkingUserTokenStore.TryGetValue(userId, out var currentJti) || currentJti != jti)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync("{\"message\":\"Session expired. You logged in from another device.\"}");
                            return;
                        }
                    }
                }

                await _next(context);
            }
        }
    }
}
