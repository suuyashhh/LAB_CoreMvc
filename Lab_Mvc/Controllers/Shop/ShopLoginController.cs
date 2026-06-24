using Lab_Mvc.Interfaces.Shop;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Shop;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Lab_Mvc.Controllers.Shop
{
    [ApiController]
    [Route("api/LoginShop")]
    public class ShopLoginController : Controller
    {
        private readonly IShopLogin _iShopLogin;
        private readonly IConfiguration _config;

        public static readonly ConcurrentDictionary<string, string> ShopUserTokenStore = new();

        public ShopLoginController(IShopLogin iShopLogin, IConfiguration config)
        {
            _iShopLogin = iShopLogin;
            _config = config;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] DTOShopLogin loginShop)
        {
            try
            {
                var result = await _iShopLogin.Login(loginShop);

                if (result == null)
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                var jwtSettings = _config.GetSection("Jwt");
                var tokenId = Guid.NewGuid().ToString(); // Unique token ID (jti)

                var authClaims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, tokenId),
                    new Claim(ClaimTypes.NameIdentifier, result.USER_ID.ToString()),
                    new Claim(ClaimTypes.Name, result.USER_NAME ?? string.Empty),
                    new Claim("module", "Shop"),
                    new Claim("shop_user_id", result.USER_ID.ToString()),
                    new Claim("shop_contact", result.CONTACT ?? string.Empty)
                };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

                var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                // Store/overwrite the latest valid token ID (Jti) for this user to enforce single session
                ShopUserTokenStore[result.USER_ID.ToString()] = tokenId;

                return Ok(new
                {
                    token = tokenString,
                    userDetails = new
                    {
                        useR_ID = result.USER_ID,
                        useR_NAME = result.USER_NAME,
                        contact = result.CONTACT,
                        useR_IMG = result.USER_IMG
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Something went wrong: " + ex.Message });
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("validate")]
        public IActionResult Validate()
        {
            var shopUserId = GetAuthenticatedShopUserId();
            if (shopUserId == null)
            {
                return Unauthorized(new { message = "Shop login required." });
            }

            return Ok(new
            {
                user = new
                {
                    userId = shopUserId,
                    name = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
                    contact = User.FindFirst("shop_contact")?.Value ?? string.Empty
                }
            });
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var shopUserId = GetAuthenticatedShopUserId();
            if (shopUserId == null)
            {
                return Unauthorized(new { message = "Shop login required." });
            }

            ShopUserTokenStore.TryRemove(shopUserId.Value.ToString(), out _);
            return Ok(new { message = "Logged out successfully" });
        }

        private int? GetAuthenticatedShopUserId()
        {
            var module = User.FindFirst("module")?.Value;
            if (!string.Equals(module, "Shop", StringComparison.Ordinal))
            {
                return null;
            }

            var claimValue =
                User.FindFirst("shop_user_id")?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(claimValue, out var userId) ? userId : null;
        }

        // Token check middleware
        public class ShopTokenValidationMiddleware
        {
            private readonly RequestDelegate _next;

            public ShopTokenValidationMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task Invoke(HttpContext context)
            {
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    var module = context.User.FindFirst("module")?.Value;
                    if (string.Equals(module, "Shop", StringComparison.Ordinal))
                    {
                        var userId =
                            context.User.FindFirst("shop_user_id")?.Value ??
                            context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                        var jti = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(jti))
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync("{\"message\":\"Invalid Shop session.\"}");
                            return;
                        }

                        if (!ShopUserTokenStore.TryGetValue(userId, out var currentJti) || currentJti != jti)
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
