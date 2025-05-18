using Booking.Web.Components;
using Booking.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register and configure the HttpClient for Booking API
builder.Services.AddHttpClient<IBookingApiService, BookingApiService>(client =>
{
    // Configure the base address from appsettings.json
    var bookingApiBaseUrl = builder.Configuration["BookingApi:BaseUrl"] ?? "http://localhost:5001/api";
    client.BaseAddress = new Uri(bookingApiBaseUrl);

    // Set default headers
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
