var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddPostgres("sql")
    .WithLifetime(ContainerLifetime.Persistent)
     .WithPgAdmin();

var bookingDb = sqlServer.AddDatabase("booking-db");

var bookingMigration = builder.AddProject<Projects.Booking_MigrationService>("booking-migrationservice")
    .WithReference(bookingDb)
    .WaitFor(sqlServer)
    .WaitFor(bookingDb);

builder.AddProject<Projects.Booking_Api>("booking-api")
    .WithReference(bookingDb)
    .WaitFor(bookingDb)
    .WaitForCompletion(bookingMigration);

builder.Build().Run();