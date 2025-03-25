using Booking.Application.Common.Behaviours;
using Booking.Application.Common.Interfaces;
using Booking.Application.Features.TodoItems;

using Microsoft.Extensions.Logging;

using Moq;

namespace Booking.Application.UnitTests.Common.Behaviours;

public class RequestLoggerTests
{
    private readonly Mock<ILogger<CreateTodoItemCommand>> _logger;
    private readonly Mock<ICurrentUserService> _currentUserService;

    public RequestLoggerTests()
    {
        _logger = new Mock<ILogger<CreateTodoItemCommand>>();
        _currentUserService = new Mock<ICurrentUserService>();
    }

    [Fact]
    public async Task ShouldCallGetUserNameAsyncOnceIfAuthenticated()
    {
        _currentUserService.Setup(x => x.UserId).Returns(Guid.NewGuid().ToString());

        var requestLogger = new LoggingBehaviour<CreateTodoItemCommand>(_logger.Object, _currentUserService.Object);

        await requestLogger.Process(new CreateTodoItemCommand(1, "title"), CancellationToken.None);
    }

    [Fact]
    public async Task ShouldNotCallGetUserNameAsyncOnceIfUnauthenticated()
    {
        var requestLogger = new LoggingBehaviour<CreateTodoItemCommand>(_logger.Object, _currentUserService.Object);

        await requestLogger.Process(new CreateTodoItemCommand(1, "title"), CancellationToken.None);
    }
}