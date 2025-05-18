using Booking.Web.Models;

namespace Booking.Web.Services;

public interface IBookingApiService
{
    Task<BookingDto?> GetBookingByIdAsync(Guid bookingId, CancellationToken cancellationToken = default);

    Task<PaginatedResponse<BookingDto>?> GetBookingsAsync(
        Guid? bookingId = null,
        Guid? customerId = null,
        Guid? sellerId = null,
        Guid? productId = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<Guid?> CreateBookingAsync(CreateBookingDto booking, CancellationToken cancellationToken = default);

    Task<Guid?> UpdateBookingAsync(UpdateBookingDto booking, CancellationToken cancellationToken = default);

    Task<bool> ChangeBookingStatusAsync(Guid bookingId, BookingStatus status, CancellationToken cancellationToken = default);
}
