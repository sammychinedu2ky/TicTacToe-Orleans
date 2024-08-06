using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Polly;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace TicTacToe_Orleans.Authorization
{
    public class CookieHandlerAuth : AuthenticationHandler<CookieHandlerAuthOptions>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptionsMonitor<CookieHandlerAuthOptions> _options;

        public CookieHandlerAuth(IHttpContextAccessor httpContextAccessor, IOptionsMonitor<CookieHandlerAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _httpContextAccessor = httpContextAccessor;
            _options = options;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.CurrentValue.Secret))
                };
                try
                {
                    var principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out _);
                    var ticket = new AuthenticationTicket(principal, CookieHandlerAuthOptions.Scheme);
                    var d = AuthenticateResult.Success(ticket);


                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }
                catch
                {
                    return Task.FromResult(AuthenticateResult.Fail("Invalid token"));

                }
            }
            return Task.FromResult(AuthenticateResult.Fail("No token"));
           
        }
    }

    public class CookieHandlerAuthOptions : AuthenticationSchemeOptions
    {
        public const string Scheme = nameof(CookieHandlerAuthOptions);
        public string Secret { get; set; }
       
    }
}


