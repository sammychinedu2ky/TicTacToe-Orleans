using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace TicTacToe_Orleans.Authorization
{
    public class CookieHandler : AuthorizationHandler<CookieHandlerRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CookieHandlerRequirement requirement)
        {
            
            if (_httpContextAccessor.HttpContext!.Request.Cookies.TryGetValue("authToken", out var jwtToken))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(requirement.Secret))
                };
                try
                {
                    var principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out _);
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
                catch
                {
                    context.Fail();
                    return Task.CompletedTask;

                }
            }
            context.Fail();
            return Task.CompletedTask;

        }
    }
}