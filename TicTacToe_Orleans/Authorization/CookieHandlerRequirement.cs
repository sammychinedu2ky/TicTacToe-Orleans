using Microsoft.AspNetCore.Authorization;

namespace TicTacToe_Orleans.Authorization
{
    public class CookieHandlerRequirement : IAuthorizationRequirement
    {
        public const string Policy = nameof(CookieHandlerRequirement);
        public string Secret { get; set; }

        public CookieHandlerRequirement(string secret)
        {
            Secret = secret;
        }
    }
}