using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe_Orleans_.Model;
using TicTacToe_Orleans_.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TicTacToe_Orleans_.Authorization;
using Microsoft.AspNetCore.Authorization;
using Npgsql;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ApplicationDbContext") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found.")));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Authority"];
        options.Audience = builder.Configuration["Audience"];
        
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthSecretRequirement.Policy, r =>
    {
        r.AddRequirements(new AuthSecretRequirement(builder.Configuration["AUTH_SECRET"]!));
    });
});
builder.Services.AddSingleton<IAuthorizationHandler, AuthSecretHandler>();
builder.Services.AddHttpContextAccessor();
// Add services to the container.

var app = builder.Build();




app.UseHttpsRedirection();



app.MapUserEndpoints();

app.MapInviteEndpoints();

app.MapGamePlayEndpoints();

app.Run();


