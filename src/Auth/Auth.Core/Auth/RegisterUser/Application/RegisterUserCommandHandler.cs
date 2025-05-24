using System.Threading;
using System.Threading.Tasks;

using Auth.Core.Common.Domain.Entities;
using Auth.Core.Common.Interfaces;
using ErrorOr;
using MediatR;

namespace Auth.Core.Auth.RegisterUser.Application
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ErrorOr<RegisterUserResult>>
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserCommandHandler(IAuthRepository authRepository, IPasswordHasher passwordHasher)
        {
            _authRepository = authRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<ErrorOr<RegisterUserResult>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _authRepository.GetUserByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null)
            {
                return Error.Conflict(code: "User.AlreadyExists", description: "User with this email already exists.");
            }

            var hashedPassword = _passwordHasher.HashPassword(request.Password);

            var user = new User
            {
                FirstName = request.FirstName ?? string.Empty,
                LastName = request.LastName ?? string.Empty,
                Email = request.Email,
                PasswordHash = hashedPassword,
                Role = UserRole.Buyer,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                IsEmailConfirmed = false,
            };

            await _authRepository.AddUserAsync(user, cancellationToken);

            return new RegisterUserResult { UserId = user.Id };
        }
    }
}
