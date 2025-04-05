using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddPostgres("sql")
    .WithLifetime(ContainerLifetime.Persistent)
     .WithPgAdmin();

var bookingDb = sqlServer.AddDatabase("booking-db");
var blobs = builder.AddAzureStorage("storage")
                   .RunAsEmulator()
                   .AddBlobs("bookings-blobs");

var bookingMigration = builder.AddProject<Projects.Booking_MigrationService>("booking-migrationservice")
    .WithReference(bookingDb)
    .WaitFor(sqlServer)
    .WaitFor(bookingDb)
    .WithReference(blobs)
    .WaitFor(blobs);

var servicesBus = builder.AddAzureServiceBus("servicebus")
.RunAsEmulator();

var bookingTopic = servicesBus.AddServiceBusTopic("booking");

builder.AddProject<Projects.Booking_Api>("booking-api")
    .WithReference(bookingDb)
    .WithReference(servicesBus)
    .WaitFor(bookingDb)
    .WaitForCompletion(bookingMigration);

var bookingHandler = builder.AddProject<Projects.BookingHandler>("booking-handler")
    .WithReference(servicesBus)
    .WithReference(blobs);

builder.Build().Run();