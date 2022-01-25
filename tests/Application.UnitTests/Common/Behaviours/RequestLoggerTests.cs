using CleanArchitecture.Application.Common.Behaviours;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CleanArchitecture.Application.UnitTests.Common.Behaviours;

public class RequestLoggerTests
{
    private readonly Mock<ILogger<CreateTodoItemCommand>> _logger;
    private readonly Mock<ICurrentUserService> _currentUserService;
    private readonly Mock<IIdentityService> _identityService;
    
    public RequestLoggerTests()
    {
        _logger = new Mock<ILogger<CreateTodoItemCommand>>();
        _currentUserService = new Mock<ICurrentUserService>();
        _identityService = new Mock<IIdentityService>();
    }

    [Fact]
    public async Task ShouldCallGetUserNameAsyncOnceIfAuthenticated()
    {
        _currentUserService.Setup(x => x.UserId).Returns(Guid.NewGuid());

        var requestLogger = new LoggingBehaviour<CreateTodoItemCommand>(_logger.Object, _currentUserService.Object, _identityService.Object);

        await requestLogger.Process(new CreateTodoItemCommand { ListId = Guid.NewGuid(), Title = "title" }, new CancellationToken());

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task ShouldNotCallGetUserNameAsyncOnceIfUnauthenticated()
    {
        var requestLogger = new LoggingBehaviour<CreateTodoItemCommand>(_logger.Object, _currentUserService.Object, _identityService.Object);

        await requestLogger.Process(new CreateTodoItemCommand { ListId = Guid.NewGuid(), Title = "title" }, new CancellationToken());

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<Guid>()), Times.Never);
    }
}
