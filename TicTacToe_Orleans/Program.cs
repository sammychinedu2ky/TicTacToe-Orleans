using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using System.Text.Json;
using TicTacToe_Orleans.Authorization;
using TicTacToe_Orleans.Endpoints;
using TicTacToe_Orleans.Hubs;
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
var secret = builder.Configuration["AUTH_SECRET"];
var dbConnectionString = builder.Configuration.GetConnectionString("ApplicationDbContext");
var client = builder.Configuration["CLIENT"];
ArgumentNullException.ThrowIfNullOrEmpty(secret);
ArgumentNullException.ThrowIfNullOrEmpty(dbConnectionString);
ArgumentNullException.ThrowIfNullOrEmpty(client);

builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
    siloBuilder.AddMemoryGrainStorage("urls");
    siloBuilder.Services.AddDbContextFactory<ApplicationDbContext>((Action<DbContextOptionsBuilder>?)(options =>
    options.UseNpgsql(dbConnectionString,
    npgsqlOptionsAction: handleDbRetry()
    )));
});
//IdentityModelEventSource.ShowPII = true;
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseNpgsql(dbConnectionString,
//    npgsqlOptionsAction: handleDbRetry()
//    ));

builder.Services.Configure<CookieHandlerAuthOptions>(options =>
{

    options.Secret = secret;
});
builder.Services.AddAuthentication()
    .AddScheme<CookieHandlerAuthOptions, CookieHandlerAuthScheme>(CookieHandlerAuthOptions.Scheme, null);

builder.Services.AddSingleton<IAuthorizationHandler, CookieHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthSecretRequirement.Policy, r =>
    {
        r.AddRequirements(new AuthSecretRequirement(secret));
    });

    options.AddPolicy(CookieHandlerRequirement.Policy, r =>
    {
        r.AddRequirements(new CookieHandlerRequirement(secret));
        // r.RequireAuthenticatedUser();
        // r.AddAuthenticationSchemes(CookieHandlerAuthOptions.Scheme);


    });
});
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.WithOrigins(client)
            .AllowAnyMethod()
            .AllowCredentials()
            .AllowAnyHeader();
    });
});
builder.Services.AddSingleton<IAuthorizationHandler, AuthSecretHandler>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();
var app = builder.Build();

app.MapDefaultEndpoints();

app.UseCors();

app.MapUserEndpoints();

app.MapInviteEndpoints();

app.MapGameRoomEndpoints();
app.MapHub<GameRoomHub>("/gameRoomHub");
app.Run();

static Action<NpgsqlDbContextOptionsBuilder> handleDbRetry()
{
    return npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), null);
    };
}