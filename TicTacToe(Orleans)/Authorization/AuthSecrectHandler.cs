using Microsoft.AspNetCore.Authorization;

namespace TicTacToe_Orleans.Authorization
{
    public class AuthSecretHandler : AuthorizationHandler<AuthSecretRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthSecretHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthSecretRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            
            if (httpContext != null &&
                httpContext.Request.Headers.TryGetValue("Auth-Secret", out var authHeader))
            {
                if (authHeader.ToString().Contains(requirement.Secret))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
