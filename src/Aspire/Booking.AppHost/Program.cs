using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddPostgres("sql")
    .WithLifetime(ContainerLifetime.Persistent)
     .WithPgAdmin();

var bookingDb = sqlServer.AddDatabase("booking-db");

var storage = builder.AddAzureStorage("storage")
                   .RunAsEmulator();
var blobs = storage.AddBlobs("bookings-blobs");

var bookingMigration = builder.AddProject<Projects.Booking_MigrationService>("booking-migrationservice")
    .WithReference(bookingDb)
    .WaitFor(sqlServer)
    .WaitFor(bookingDb)
    .WithReference(blobs)
    .WaitFor(blobs);

var bookingApi = builder.AddProject<Projects.Booking_Api>("booking-api")
    .WithReference(bookingDb)
    .WaitFor(bookingDb)
    .WaitForCompletion(bookingMigration);

var messaging = builder
    .AddAzureServiceBus("servicebus")
    .RunAsEmulator(c =>
        c.WithLifetime(ContainerLifetime.Persistent))
    .AddServiceBusTopic("booking")
    .AddServiceBusSubscription("hotel");

bookingApi.WithReference(messaging)
    .WaitFor(messaging);

builder.AddAzureFunctionsProject<Projects.Hotel_EventConsumer>("hotel-eventconsumer")
    .WithHostStorage(storage)
    .WithReference(messaging)
    .WaitFor(messaging)
    .WithReference(blobs)
    .WaitFor(blobs);

builder.Build().Run();