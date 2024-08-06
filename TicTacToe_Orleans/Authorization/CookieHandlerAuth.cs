using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        private readonly IOptionsMonitor<CookieHandlerAuthOptions> _options;

        public CookieHandlerAuth( IOptionsMonitor<CookieHandlerAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _options = options;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            
            if (Context!.Request.Cookies.TryGetValue("authToken", out var jwtToken))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    AuthenticationType = "JWT",
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.CurrentValue.Secret))
                };
                try
                {
                    
                    var principal =  await tokenHandler.ValidateTokenAsync(jwtToken, validationParameters);
                    
                  
                    // Modify claims if necessary
                    var identity = (ClaimsIdentity)principal.ClaimsIdentity!;
                    var claims = new List<Claim>(identity.Claims);

                    // Example: Map a specific claim
                    var email = identity?.FindFirst(ClaimTypes.Email)?.Value;
                    var name = identity?.FindFirst(ClaimTypes.Name)?.Value;
                    if (name == null)
                    {
                        claims.Add(new Claim(ClaimTypes.Name, "user"));
                    }
                    var newIdentity = new ClaimsIdentity(claims,"JWT");
                   
                    var newPrincipal = new ClaimsPrincipal(newIdentity);
                    var ticket = new AuthenticationTicket(newPrincipal, CookieHandlerAuthOptions.Scheme);
                    var d = AuthenticateResult.Success(ticket);


                    return AuthenticateResult.Success(ticket);
                }
                catch
                {
                    return AuthenticateResult.Fail("Invalid token");

                }
            }
            return AuthenticateResult.Fail("No token");

        }
    }

    public class CookieHandlerAuthOptions : AuthenticationSchemeOptions
    {
        public const string Scheme = "CustomCookie";
        public string Secret { get; set; }

    }
}


