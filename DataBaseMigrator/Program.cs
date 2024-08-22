using DataBaseMigrator;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Orleans.Hosting;

var builder = Host.CreateApplicationBuilder(args);
var postgresConnectionString = builder.Configuration.GetConnectionString("tictactoedb");
Console.WriteLine(postgresConnectionString);
ArgumentNullException.ThrowIfNullOrEmpty(postgresConnectionString);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();
builder.Services.AddDbContext<ApplicationDbContext>((Action<DbContextOptionsBuilder>?)(options =>
options.UseNpgsql(postgresConnectionString,
npgsqlOptionsAction: handleDbRetry()
)),ServiceLifetime.Singleton);
var host = builder.Build();
host.Run();

static Action<NpgsqlDbContextOptionsBuilder> handleDbRetry()
{
    return npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), null);
    };
}