using System.Security.Claims;

using Microsoft.AspNetCore.Http;

using Shared.Common.Interfaces;

namespace Booking.Booking.Application.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)!;
}