using Booking.Web.Components;
using Booking.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<IBookingApiService, BookingApiService>();

// Register the unified BookingApiClient
builder.Services.AddHttpClient<BookingApiService>(client =>
{
    var bookingApiBaseUrl = Environment.GetEnvironmentVariable("BOOKING_API_URL")
                            ?? builder.Configuration["BookingApi:BaseUrl"]
                            ?? "https://localhost:7098/api/v1";
    client.BaseAddress = new Uri(bookingApiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

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
