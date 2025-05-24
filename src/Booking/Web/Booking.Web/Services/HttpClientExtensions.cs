using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Booking.Web.Services;

// Extensiones para HttpClient para añadir soporte para PatchAsJsonAsync
public static class HttpClientExtensions
{
    public static Task<HttpResponseMessage> PatchAsJsonAsync<TValue>(
        this HttpClient client,
        string? requestUri,
        TValue value,
        JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        var content = JsonContent.Create(value, options: options);
        return client.PatchAsync(requestUri, content, cancellationToken);
    }
}