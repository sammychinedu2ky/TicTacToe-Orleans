using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Hosting;
using Polly;
using System.Threading;

namespace DataBaseMigrator
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public Worker(ILogger<Worker> logger, ApplicationDbContext applicationDbContext, IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await EnsureDatabaseAsync(stoppingToken);
                await RunMigrationAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            _hostApplicationLifetime.StopApplication();
        }
        private async Task EnsureDatabaseAsync(CancellationToken stoppingToken)
        {
            var dbCreator = _applicationDbContext.GetService<IRelationalDatabaseCreator>();
            var strategy = _applicationDbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                if (!await dbCreator.ExistsAsync(stoppingToken))
                {
                    await dbCreator.CreateAsync(stoppingToken);
                }
            });
        }
        private async Task RunMigrationAsync(CancellationToken stoppingToken)
        {
            var strategy = _applicationDbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _applicationDbContext.Database.BeginTransactionAsync(stoppingToken);
                await _applicationDbContext.Database.MigrateAsync(stoppingToken);
                await transaction.CommitAsync(stoppingToken);
            });
        }

    }
}
