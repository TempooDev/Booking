var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddPostgres("sql")
    .WithLifetime(ContainerLifetime.Persistent)
     .WithPgAdmin();

var bookingDb = sqlServer.AddDatabase("booking-db");

var storage = builder.AddAzureStorage("storage")
                   .RunAsEmulator();
var blobs = storage.AddBlobs("bookings-blobs");

var messaging = builder
    .AddAzureServiceBus("servicebus")
    .RunAsEmulator()
    .AddServiceBusTopic("booking")
    .AddServiceBusSubscription("hotel");

var bookingMigration = builder.AddProject<Projects.Booking_MigrationService>("booking-migrationservice")
    .WithReference(bookingDb)
    .WaitFor(sqlServer)
    .WaitFor(bookingDb);

var bookingApi = builder.AddProject<Projects.Booking_Api>("booking-api")
    .WithReference(bookingDb)
    .WaitFor(bookingDb)
    .WaitForCompletion(bookingMigration);

bookingApi.WithReference(messaging)
    .WaitFor(messaging);

builder.AddAzureFunctionsProject<Projects.Hotel_EventConsumer>("hotel-eventconsumer")
    .WithHostStorage(storage)
    .WithReference(messaging)
    .WaitFor(messaging)
    .WithReference(blobs)
    .WaitFor(blobs);

builder.AddAzureFunctionsProject<Projects.Hotel_Api>("hotel-api");

builder.Build().Run();