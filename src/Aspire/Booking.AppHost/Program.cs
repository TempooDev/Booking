var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Booking_Api>("api");

builder.Build().Run();
