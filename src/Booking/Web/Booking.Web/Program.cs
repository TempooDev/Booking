using Booking.Web.Components;
using Booking.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<IBookingApiService, BookingApiService>();

// Register the unified BookingApiClient
builder.Services.AddHttpClient<BookingApiService>(client => client.BaseAddress = new Uri("https+http://booking-api"));

builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddConsole();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseStaticFiles();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
