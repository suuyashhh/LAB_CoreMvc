using Dapper;
using Lab_Mvc.Constants;
using Lab_Mvc.Interfaces;
using Lab_Mvc.Repositries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models;
using StackExchange.Redis;
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
        private readonly IConfiguration _config;
        private readonly IConnectionMultiplexer _redis;

        public LoginController(ILogin loginrepository, IConfiguration config, IConnectionMultiplexer redis)
        {
            this.loginRepository = loginrepository;
            _config = config;
            _redis = redis;
        }

        
        [AllowAnonymous]
        //    [HttpPost]
        //    [Route("Login")]
        //    public async Task<IActionResult> Login([FromBody] DTOLogin login)
        //    {
        //        var user = await loginRepository.Login(login);
        //        if (user == null)
        //        {
        //            return Unauthorized("Invalid credentials");
        //        }

        //        var jwtSettings = this.HttpContext.RequestServices.GetRequiredService<IConfiguration>().GetSection("Jwt");

        //        var authClaims = new[]
        //        {
        //    new Claim(user.CONTACT, user.PASSWORD),
        //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //};

        //        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

        //        var token = new JwtSecurityToken(
        //            issuer: jwtSettings["Issuer"],
        //            audience: jwtSettings["Audience"],
        //            expires: DateTime.UtcNow.AddDays(Convert.ToDouble(jwtSettings["AccessTokenExpirationDays"])), // 🔥 Expiration in days
        //            claims: authClaims,
        //            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        //        );

        //        return Ok(new
        //        {
        //            token = new JwtSecurityTokenHandler().WriteToken(token),
        //            expiration = token.ValidTo,
        //            userDetails = new
        //            {
        //                user.USER_ID,
        //                user.NAME,
        //                user.CONTACT,
        //                user.COM_ID,
        //                user.USER_LOGIN
        //                // 🔒 Don’t include password in response
        //            }
        //        });
        //    }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] DTOLogin login)
        {
            var user = await loginRepository.Login(login);
            if (user == null) return Unauthorized("Invalid credentials");

            var jwtSettings = this.HttpContext.RequestServices.GetRequiredService<IConfiguration>().GetSection("Jwt");
            var tokenId = Guid.NewGuid().ToString(); // Unique ID per token

            var authClaims = new List<Claim>
    {
                new Claim(user.CONTACT, user.PASSWORD),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //new Claim(JwtRegisteredClaimNames.Jti, tokenId),
        //new Claim(ClaimTypes.NameIdentifier, user.useR_ID.ToString()),
        //new Claim(ClaimTypes.Name, user.name),
        //new Claim("COM_ID", user.coM_ID)
    };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["key"]));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["AccessTokenExpirationMinutes"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Store token in Redis by user ID
            var redis = _redis.GetDatabase();
            await redis.StringSetAsync($"user:{user.USER_ID}", tokenId, TimeSpan.FromDays(15));


            return Ok(new
            {
                token = tokenString,
                userDetails = new
                {
                    user.USER_ID,
                    user.NAME,
                    user.COM_ID,
                    user.USER_LOGIN
                }
            });
        }


        public class TokenValidationMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly IConnectionMultiplexer _redis;

            public TokenValidationMiddleware(RequestDelegate next, IConnectionMultiplexer redis)
            {
                _next = next;
                _redis = redis;
            }

            public async Task Invoke(HttpContext context)
            {
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var jti = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                    if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(jti))
                    {
                        var redisJti = await _redis.GetDatabase().StringGetAsync($"user:{userId}");
                        if (redisJti != jti)
                        {
                            context.Response.StatusCode = 401;
                            await context.Response.WriteAsync("Logged in from another device. Session expired.");
                            return;
                        }
                    }
                }

                await _next(context);
            }
        }


    }
}
