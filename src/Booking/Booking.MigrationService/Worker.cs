using System.Diagnostics;

using Booking.Core.Common.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

using OpenTelemetry.Trace;

namespace Booking.MigrationService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime hostApplicationLifetime;

        public Worker(
            ILogger<Worker> logger,
            IServiceProvider serviceProvider,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            this.hostApplicationLifetime = hostApplicationLifetime;
        }

        public const string ActivitySourceName = "Migrations";
        private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var activity = ActivitySource.StartActivity("Migrating database", ActivityKind.Client);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await RunMigrationAsync(dbContext, cancellationToken);
            }
            catch (Exception ex)
            {
                activity?.AddEvent(new ActivityEvent("Exception", default, new ActivityTagsCollection
                {
                    { "exception.type", ex.GetType().FullName },
                    { "exception.message", ex.Message },
                    { "exception.stacktrace", ex.StackTrace },
                }));
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