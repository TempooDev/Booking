using System; // Required for DateTime

using Shared.Common;

namespace Auth.Core.Common.Domain.Entities
{
    public class User : AuditableEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Anonymous;
        public string? PhoneNumber { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }

    public enum UserRole
    {
        Anonymous,
        Buyer,
        Seller,
        Admin,
    }
}