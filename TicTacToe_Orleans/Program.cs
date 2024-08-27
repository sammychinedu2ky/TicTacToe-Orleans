using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using StackExchange.Redis;
using System.Text.Json;
using TicTacToe_Orleans.Authorization;
using TicTacToe_Orleans.Endpoints;
using TicTacToe_Orleans.Hubs;
var builder = WebApplication.CreateBuilder(args);
var secret = builder.Configuration["AUTH_SECRET"]!;
var postgresConnectionString = builder.Configuration.GetConnectionString("tictactoedb")!;
var client = builder.Configuration["CLIENT"]!;
var dashBoardPort = builder.Configuration["ORLEANS-SILO-DASHBOARD"]!;
var redisConnectionString = builder.Configuration.GetConnectionString("redis")!;

ArgumentNullException.ThrowIfNullOrEmpty(postgresConnectionString);
ArgumentNullException.ThrowIfNullOrEmpty(secret);
ArgumentNullException.ThrowIfNullOrEmpty(client);
ArgumentNullException.ThrowIfNullOrEmpty(dashBoardPort);
ArgumentNullException.ThrowIfNullOrEmpty(redisConnectionString);

int.TryParse(dashBoardPort, out var dashBoardPortInt);
builder.AddServiceDefaults();
builder.AddKeyedRedisClient("redis");
builder.UseOrleans(siloBuilder =>
{


    siloBuilder.Services.AddDbContextFactory<ApplicationDbContext>((Action<DbContextOptionsBuilder>?)(options =>
    options.UseNpgsql(postgresConnectionString,
    npgsqlOptionsAction: handleDbRetry()
    )));

    siloBuilder.UseDashboard(option =>
    {
        option.Port = dashBoardPortInt;
    });
});


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
builder.Services.AddSignalR().AddStackExchangeRedis(redisConnectionString, options => options.Configuration.ChannelPrefix = RedisChannel.Literal("MyApp"));


var app = builder.Build();

app.MapDefaultEndpoints();

app.UseCors();

app.MapUserEndpoints();

app.MapInviteEndpoints();

app.MapGameRoomEndpoints();
app.MapHub<GameRoomHub>("api/orleans/gameRoomHub");
app.Run();

static Action<NpgsqlDbContextOptionsBuilder> handleDbRetry()
{
    return npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), null);
    };
}