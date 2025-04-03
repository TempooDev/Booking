using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddPostgres("sql")
    .WithLifetime(ContainerLifetime.Persistent)
     .WithPgAdmin();

var bookingDb = sqlServer.AddDatabase("booking-db");

var bookingMigration = builder.AddProject<Projects.Booking_MigrationService>("booking-migrationservice")
    .WithReference(bookingDb)
    .WaitFor(sqlServer)
    .WaitFor(bookingDb);

var eventHub = builder.AddAzureEventHubs("eventhubns")
.RunAsEmulator();

eventHub.AddHub("booking");

var blobs = builder.AddAzureStorage("storage")
                   .RunAsEmulator()
                   .AddBlobs("blobs");

builder.AddProject<Projects.Booking_Api>("booking-api")
    .WithReference(bookingDb)
    .WithReference(eventHub)
    .WaitFor(bookingDb)
    .WaitForCompletion(bookingMigration);

var bookingHandler = builder.AddProject<Projects.BookingHandler>("booking-handler")
    .WithReference(eventHub)
    .WithReference(blobs);

builder.Build().Run();