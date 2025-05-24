using ErrorOr;
using MediatR;

namespace Auth.Core.Auth.RegisterUser.Application
{
    public record RegisterUserCommand : IRequest<ErrorOr<RegisterUserResult>>
    {
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string? FirstName { get; init; } = string.Empty;
        public string? LastName { get; init; }
        public DateTime? DateOfBirth { get; init; }
        public string? PhoneNumber { get; init; }
        public string? Address { get; init; }
    }
}
