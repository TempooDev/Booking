using System.Net;
using System.Text.Json;

using Booking.Core.Users.Application.Commands;
using Booking.Core.Users.Domain.Entities;
using Booking.Web.Models;

using static Booking.Common.Errors.Errors;

namespace Booking.Web.Services;

public class BookingApiService : IBookingApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BookingApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public BookingApiService(HttpClient httpClient, ILogger<BookingApiService> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://localhost:7098/api/v1");
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
            var url = _httpClient.BaseAddress + $"/booking/{bookingId}"; // Add missing semicolon
            var response = await _httpClient.GetAsync(url, cancellationToken);

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
            var url = _httpClient.BaseAddress + $"/booking?{queryString}";

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
            var url = _httpClient.BaseAddress + "/booking";
            var response = await _httpClient.PostAsJsonAsync(url, booking, _jsonOptions, cancellationToken);

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
            var url = _httpClient.BaseAddress + $"/booking/{booking.BookingId}";
            var response = await _httpClient.PutAsJsonAsync(url, booking, _jsonOptions, cancellationToken);

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
            var url = _httpClient.BaseAddress + $"/booking/{bookingId}/status";

            var response = await _httpClient.PatchAsJsonAsync(url, status, _jsonOptions, cancellationToken);

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

    public async Task<bool> DeleteBookingAsync(Guid bookingId, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = _httpClient.BaseAddress + $"/booking/{bookingId}";
            var response = await _httpClient.DeleteAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError(
                "Error deleting booking {BookingId}. Status: {StatusCode}, Error: {Error}",
                bookingId,
                response.StatusCode,
                error);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while deleting booking {BookingId}", bookingId);
            return false;
        }
    }

    public async Task<IEnumerable<BookingDto>> GetBookingsByStatusAsync(BookingStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = _httpClient.BaseAddress + $"/booking/status/{status}";
            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<BookingDto>>(_jsonOptions, cancellationToken) ?? new List<BookingDto>();
            }

            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError(
                "Error getting bookings by status {Status}. Status: {StatusCode}, Error: {Error}",
                status,
                response.StatusCode,
                error);
            return new List<BookingDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while getting bookings by status {Status}", status);
            return new List<BookingDto>();
        }
    }

    public async Task<Guid?> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken = default)
    {
        CreateUserCommand user = new CreateUserCommand(
            Name: $"{createUserDto.FirstName} {createUserDto.LastName}",
            FirstName: createUserDto.FirstName,
            LastName: createUserDto.LastName,
            Email: createUserDto.Email,
            Role: createUserDto.Role,
            PreferredPaymentMethod: createUserDto.PreferredPaymentMethod,
            Rating: createUserDto.Role == UserRole.Seller ? 0 : null, // Default rating for sellers
            StoreName: createUserDto.Role == UserRole.Seller ? "Default Store" : null); // Default store name for sellers

        try
        {
            var url = _httpClient.BaseAddress + "/users";
            var response = await _httpClient.PostAsJsonAsync(url, user, _jsonOptions, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Guid>(_jsonOptions, cancellationToken);
                return result;
            }

            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError(
                "Error creating user. Status: {StatusCode}, Error: {Error}",
                response.StatusCode,
                error);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while creating user");
            return null;
        }
    }
}
