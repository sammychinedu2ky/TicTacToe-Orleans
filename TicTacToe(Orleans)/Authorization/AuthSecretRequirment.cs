using Microsoft.AspNetCore.Authorization;

namespace TicTacToe_Orleans_.Authorization
{
    public class AuthSecretRequirement : IAuthorizationRequirement
    {
        public const string Policy = nameof(AuthSecretRequirement);
        public string Secret { get; set; }

        public AuthSecretRequirement(string secret)
        {
            Secret = secret;
        }
    }
}
