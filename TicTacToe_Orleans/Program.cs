using Microsoft.EntityFrameworkCore;
using TicTacToe_Orleans.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TicTacToe_Orleans.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using TicTacToe_Orleans.Hubs;
using Orleans.Runtime;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
var builder = WebApplication.CreateBuilder(args);

IdentityModelEventSource.ShowPII = true;
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ApplicationDbContext") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found."),
    npgsqlOptionsAction: npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), null);
    }
    ));
Console.WriteLine(builder.Configuration["AUTH_SECRET"]!);
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AUTH_SECRET"]!))
        };

    })
    .AddCookie(options =>
    {
        options.ForwardChallenge = JwtBearerDefaults.AuthenticationScheme;
        options.Events.OnSigningIn = async context =>
        {
            var jwtToken = context.Request.Cookies["authToken"];
            if (jwtToken != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AUTH_SECRET"]!))
                };
                try
                {
                    var principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out _);
                    context.Principal = principal;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }
            };
        };
     
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthSecretRequirement.Policy, r =>
    {
        r.AddRequirements(new AuthSecretRequirement(builder.Configuration["AUTH_SECRET"]!));
    });
    options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, r =>
    {
        r.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
        r.RequireAuthenticatedUser();
    });
    options.AddPolicy(CookieAuthenticationDefaults.AuthenticationScheme, r =>
    {
        r.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
        r.RequireAuthenticatedUser();
    });
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.Services.AddSingleton<IAuthorizationHandler, AuthSecretHandler>();
builder.Services.AddHttpContextAccessor();
builder.Host.UseOrleans(static siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
    siloBuilder.AddMemoryGrainStorage("urls");

});
builder.Services.AddSignalR();
var app = builder.Build();

app.UseCors();
app.MapUserEndpoints();

app.MapInviteEndpoints();

app.MapGameRoomEndpoints();
app.MapHub<GameRoomHub>("/gameRoomHub");
app.Run();


