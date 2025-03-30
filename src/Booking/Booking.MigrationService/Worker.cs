using System.Diagnostics;

using Booking.Booking.Application.Common.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

using OpenTelemetry.Trace;

namespace Booking.Booking.MigrationService
{
    public class Worker(
        IServiceProvider serviceProvider,
        IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
    {
        public const string ActivitySourceName = "Migrations";
        private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var activity = ActivitySource.StartActivity("Migrating database", ActivityKind.Client);

            try
            {
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await RunMigrationAsync(dbContext, cancellationToken);
            }
            catch (Exception ex)
            {
                activity?.RecordException(ex);
                throw;
            }

            hostApplicationLifetime.StopApplication();
        }

        private static async Task RunMigrationAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
        {
            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
                await dbContext.Database.MigrateAsync(cancellationToken));
        }
    }
}