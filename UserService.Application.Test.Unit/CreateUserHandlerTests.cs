namespace UserService.Application.Test.Unit
{
    using Moq;
    using Xunit;
    using Microsoft.EntityFrameworkCore;
    using UserService.Application.Users.Commands.Create;
    using UserService.Infrastructure.Database;
    using UserService.Application.Users.Commands.Create.UserService.Infrastructure.Email;
    using UserService.Domain.Users;
    using Microsoft.AspNetCore.Identity;

    public class CreateUserHandlerTests
    {
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
        private readonly DatabaseContext _context;
        private readonly CreateUserHandler _handler;

        public CreateUserHandlerTests()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new DatabaseContext(options);
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _passwordHasherMock = new Mock<IPasswordHasher<User>>();
            _emailServiceMock = new Mock<IEmailService>();

            _handler = new CreateUserHandler(_context, _userManagerMock.Object, _passwordHasherMock.Object, _emailServiceMock.Object);
        }

       [Fact]
        public async Task Handle_ShouldThrowException_WhenUserAlreadyExists()
        {
            var existingUser = new User
            {
                UserName = "test@example.com",
                Email = "test@example.com",
                Name = "John",
                Lastname = "Doe",
                Role = "Admin"
            };
            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            var command = new CreateUserCommand
            {
                Email = "test@example.com",
                Name = "John",
                Lastname = "Doe",
                Role = "Admin",
                Password = "Password123",
                Scheme = "http",
                Host = "localhost"
            };

            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        }
    }

}