using System.Net;
using System.Text.Json;

using Booking.Web.Models;

namespace Booking.Web.Services;

public class BookingApiService : IBookingApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BookingApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public BookingApiService(HttpClient httpClient, ILogger<BookingApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };
    }

    public async Task<BookingDto?> GetBookingByIdAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"booking/{bookingId}", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<BookingDto>(_jsonOptions, cancellationToken);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError(
                "Error getting booking {BookingId}. Status: {StatusCode}, Error: {Error}",
                bookingId,
                response.StatusCode,
                error);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while getting booking {BookingId}", bookingId);
            return null;
        }
    }

    public async Task<PaginatedResponse<BookingDto>?> GetBookingsAsync(
        Guid? bookingId = null,
        Guid? customerId = null,
        Guid? sellerId = null,
        Guid? productId = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = new List<string>();

            if (bookingId.HasValue)
            {
                queryParams.Add($"bookingId={bookingId}");
            }

            if (customerId.HasValue)
            {
                queryParams.Add($"customerId={customerId}");
            }

            if (sellerId.HasValue)
            {
                queryParams.Add($"sellerId={sellerId}");
            }

            if (productId.HasValue)
            {
                queryParams.Add($"productId={productId}");
            }

            queryParams.Add($"pageNumber={pageNumber}");
            queryParams.Add($"pageSize={pageSize}");

            var queryString = string.Join("&", queryParams);
            var url = $"booking?{queryString}";

            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PaginatedResponse<BookingDto>>(_jsonOptions, cancellationToken);
            }

            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError(
                "Error getting bookings. Status: {StatusCode}, Error: {Error}",
                response.StatusCode,
                error);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while getting bookings");
            return null;
        }
    }

    public async Task<Guid?> CreateBookingAsync(CreateBookingDto booking, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("booking", booking, _jsonOptions, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Guid>(_jsonOptions, cancellationToken);
                return result;
            }

            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError(
                "Error creating booking. Status: {StatusCode}, Error: {Error}",
                response.StatusCode,
                error);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while creating booking");
            return null;
        }
    }

    public async Task<Guid?> UpdateBookingAsync(UpdateBookingDto booking, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"booking/{booking.BookingId}", booking, _jsonOptions, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Guid>(_jsonOptions, cancellationToken);
                return result;
            }

            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError(
                "Error updating booking {BookingId}. Status: {StatusCode}, Error: {Error}",
                booking.BookingId,
                response.StatusCode,
                error);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while updating booking {BookingId}", booking.BookingId);
            return null;
        }
    }

    public async Task<bool> ChangeBookingStatusAsync(Guid bookingId, BookingStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PatchAsJsonAsync($"booking/{bookingId}/status", status, _jsonOptions, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError(
                "Error changing status for booking {BookingId}. Status: {StatusCode}, Error: {Error}",
                bookingId,
                response.StatusCode,
                error);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while changing status for booking {BookingId}", bookingId);
            return false;
        }
    }
}
