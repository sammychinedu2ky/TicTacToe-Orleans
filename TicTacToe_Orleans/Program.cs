using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TicTacToe_Orleans.Authorization;
using TicTacToe_Orleans.Endpoints;
using TicTacToe_Orleans.Hubs;
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

//IdentityModelEventSource.ShowPII = true;
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ApplicationDbContext") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found."),
    npgsqlOptionsAction: npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), null);
    }
    ));
Console.WriteLine(builder.Configuration["AUTH_SECRET"]!);
builder.Services.Configure<CookieHandlerAuthOptions>(options =>
{

    options.Secret = builder.Configuration["AUTH_SECRET"]!;
});
builder.Services.AddAuthentication()
    .AddScheme<CookieHandlerAuthOptions, CookieHandlerAuthScheme>(CookieHandlerAuthOptions.Scheme, null)
    ;

builder.Services.AddSingleton<IAuthorizationHandler, CookieHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthSecretRequirement.Policy, r =>
    {
        r.AddRequirements(new AuthSecretRequirement(builder.Configuration["AUTH_SECRET"]!));
    });

    options.AddPolicy(CookieHandlerRequirement.Policy, r =>
    {
        // r.RequireAuthenticatedUser();
        r.AddRequirements(new CookieHandlerRequirement(builder.Configuration["AUTH_SECRET"]!));
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
        policyBuilder.WithOrigins(builder.Configuration["CLIENT"]!)
            .AllowAnyMethod()
            .AllowCredentials()
            .AllowAnyHeader();
    });
});
builder.Services.AddSingleton<IAuthorizationHandler, AuthSecretHandler>();
builder.Services.AddHttpContextAccessor();
builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
    siloBuilder.UseLocalhostClustering();
    siloBuilder.AddMemoryGrainStorage("urls");
    //siloBuilder.Services.AddDbContextFactory<ApplicationDbContext>();
    siloBuilder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ApplicationDbContext") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found."),
    npgsqlOptionsAction: npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), null);
    }
    ), ServiceLifetime.Scoped);

});
builder.Services.AddSignalR();
var app = builder.Build();

app.MapDefaultEndpoints();

app.UseCors();

app.MapUserEndpoints();

app.MapInviteEndpoints();

app.MapGameRoomEndpoints();
app.MapHub<GameRoomHub>("/gameRoomHub");
app.Run();


