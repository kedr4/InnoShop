using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Controllers;

namespace UserService.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly UsersController _controller;
        private readonly UserContext _context;

        public UsersControllerTests()
        {
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new UserContext(options);
            // _controller = new UsersController(_context); //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }

        [Fact]
        public async Task PutUser_ExistingId_UpdatesUser()
        {
            var userId = "1";
            var existingUser = new User { Id = userId, Name = "John", Lastname = "Doe", Email = "john@example.com", Role = "User" };
            await _context.Users.AddAsync(existingUser);
            await _context.SaveChangesAsync();

            var updateUser = new User { Id = userId, Name = "Johnny", Lastname = "Doe", Email = "johnny@example.com", Role = "User" };

            var result = await _controller.PutUser(userId, updateUser);

            Assert.IsType<NoContentResult>(result);
            var updatedUser = await _context.Users.FindAsync(userId);
            Assert.NotNull(updatedUser);
            Assert.Equal("Johnny", updatedUser.Name);
        }

        [Fact]
        public async Task DeleteUser_ExistingId_RemovesUser()
        {
            var userId = "1";
            var user = new User { Id = userId, Name = "John", Lastname = "Doe", Email = "john@example.com", Role = "User" };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteUser(userId);

            Assert.IsType<NoContentResult>(result);
            var deletedUser = await _context.Users.FindAsync(userId);
            Assert.Null(deletedUser);
        }

        [Fact]
        public async Task GetUsers_ReturnsAllUsers()
        {
            var users = new List<User>
            {
                new User { Id = "1", Name = "John", Lastname = "Doe", Email = "john@example.com", Role = "User" },
                new User { Id = "2", Name = "Jane", Lastname = "Doe", Email = "jane@example.com", Role = "Admin" }
            };
            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            var result = await _controller.GetUsers();

            var actionResult = Assert.IsType<ActionResult<IEnumerable<User>>>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<User>>(actionResult.Value);
            Assert.Equal(2, returnValue.Count());
        }
    }
}
