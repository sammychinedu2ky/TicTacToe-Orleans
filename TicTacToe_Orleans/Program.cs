using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Orleans.Configuration;
using System.Net;
using System.Text.Json;
using TicTacToe_Orleans.Authorization;
using TicTacToe_Orleans.Endpoints;
using TicTacToe_Orleans.Hubs;
var builder = WebApplication.CreateBuilder(args);
var secret = builder.Configuration["AUTH_SECRET"]!;
var dbConnectionString = builder.Configuration.GetConnectionString("ApplicationDbContext")!;
var postgresConnectionString = builder.Configuration.GetConnectionString("tictactoedb")!;
var client = builder.Configuration["CLIENT"]!;
var dashBoardPort = builder.Configuration["ORLEANS-SILO-DASHBOARD"]!;
var orleansPort = builder.Configuration["ORLEANS-SILO-PORT"]!;
var gatewayPort = builder.Configuration["ORLEANS-GATEWAY-PORT"];
var redisConnectionString = builder.Configuration.GetConnectionString("redis")!;
if (builder.Environment.IsEnvironment("Production"))
{
    ArgumentNullException.ThrowIfNullOrEmpty(postgresConnectionString);
    ArgumentNullException.ThrowIfNullOrEmpty(secret);
    //ArgumentNullException.ThrowIfNullOrEmpty(dbConnectionString);
    ArgumentNullException.ThrowIfNullOrEmpty(client);
    ArgumentNullException.ThrowIfNullOrEmpty(dashBoardPort);
    ArgumentNullException.ThrowIfNullOrEmpty(orleansPort);
    ArgumentNullException.ThrowIfNullOrEmpty(gatewayPort);
    ArgumentNullException.ThrowIfNullOrEmpty(redisConnectionString);
}
int.TryParse(dashBoardPort, out var dashBoardPortInt);
int.TryParse(orleansPort, out var orleansPortInt);
int.TryParse(gatewayPort, out var gatewayPortInt);
//builder.AddRedisClient("redis");
builder.AddServiceDefaults();
builder.AddKeyedRedisClient("redis");
builder.UseOrleans(siloBuilder =>
{

   
    siloBuilder.Services.AddDbContextFactory<ApplicationDbContext>((Action<DbContextOptionsBuilder>?)(options =>
    options.UseNpgsql(postgresConnectionString,
    npgsqlOptionsAction: handleDbRetry()
    )));
    //siloBuilder.Configure<EndpointOptions>(options =>
    //{
    //    // Port to use for silo-to-silo
    //   // options.AdvertisedIPAddress = IPAddress.Loopback;
    //    options.SiloPort = orleansPortInt;
    //   // options.GatewayPort = gatewayPortInt;
    //});
    //siloBuilder.UseRedisClustering( configuration: o =>
    //{
    //    o.ConnectionString = redisConnectionString;
    //    o.Database = 0;

    //});
    if (builder.Environment.IsDevelopment())
    {
        siloBuilder.ConfigureEndpoints(Random.Shared.Next(10_000, 50_000), Random.Shared.Next(10_000, 50_000));
    }
    siloBuilder.UseDashboard(option =>
    {
        option.Port = dashBoardPortInt;
    });
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
builder.Services.AddSignalR().AddStackExchangeRedis(redisConnectionString);
var app = builder.Build();

app.MapDefaultEndpoints();

app.UseCors();

app.MapUserEndpoints();

app.MapInviteEndpoints();

app.MapGameRoomEndpoints();
app.MapHub<GameRoomHub>("api/gameRoomHub");
app.Run();

static Action<NpgsqlDbContextOptionsBuilder> handleDbRetry()
{
    return npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), null);
    };
}