using Microsoft.EntityFrameworkCore;
using UserService.Application.Users.Commands.Delete;
using UserService.Domain.Users;
using UserService.Infrastructure.Database;

public class DeleteUserHandlerTests
{
    private readonly DatabaseContext _context;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _context = new DatabaseContext(options);
        _handler = new DeleteUserCommandHandler(_context);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserNotFound()
    {
        var command = new DeleteUserCommand("nonexistent");

        var response = await _handler.Handle(command, CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal("User not found", response.ErrorMessage);
    }
}
