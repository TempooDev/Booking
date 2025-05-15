using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Hotel.Api;

public class GetCurrentBookings
{
    private readonly ILogger<GetCurrentBookings> _logger;

    public GetCurrentBookings(ILogger<GetCurrentBookings> logger)
    {
        _logger = logger;
    }

    [Function("GetCurrentBookings")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}