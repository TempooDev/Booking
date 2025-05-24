using Booking.Core.Users.Domain.Entities;

namespace Booking.Web.Models;

public class CreateUserDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PreferredPaymentMethod { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Buyer;
}