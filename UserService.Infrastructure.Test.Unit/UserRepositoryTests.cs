using Microsoft.EntityFrameworkCore;
using Moq;
using UserService.Domain.Users;
using UserService.Infrastructure.Database.Repositories;
using UserService.Infrastructure.Database;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace UserService.Tests.Infrastructure.Database.Repositories
{
    public class UserRepositoryTests
    {
        private DbContextOptions<DatabaseContext> _options;

        public UserRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }

        private DatabaseContext GetContext()
        {
            return new DatabaseContext(_options);
        }

        private UserRepository GetRepository()
        {
            return new UserRepository(GetContext());
        }

        [Fact]
        public async Task CreateUserAsync_ShouldAddUserToDatabase()
        {
            // Arrange
            var context = GetContext();
            var userRepository = GetRepository();
            var user = new User { Id = Guid.NewGuid().ToString(), Name = "John", Lastname = "Doe", Email = "john.doe@example.com", Role = "Admin" };

            // Act
            var userId = await userRepository.CreateUserAsync(user);

            // Assert
            var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            Assert.NotNull(savedUser);
            Assert.Equal("John", savedUser.Name);
            Assert.Equal("Doe", savedUser.Lastname);
            Assert.Equal("john.doe@example.com", savedUser.Email);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUserIfFound()
        {
            // Arrange
            var context = GetContext();
            var userRepository = GetRepository();
            var user = new User { Id = Guid.NewGuid().ToString(), Name = "Jane", Lastname = "Doe", Email = "jane.doe@example.com", Role = "User" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            var result = await userRepository.GetUserByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Jane", result.Name);
            Assert.Equal("Doe", result.Lastname);
            Assert.Equal("jane.doe@example.com", result.Email);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnListOfUsers()
        {
            // Arrange
            var context = GetContext();
            var userRepository = GetRepository();
            var user1 = new User { Id = Guid.NewGuid().ToString(), Name = "Alice", Lastname = "Smith", Email = "alice.smith@example.com", Role = "User" };
            var user2 = new User { Id = Guid.NewGuid().ToString(), Name = "Bob", Lastname = "Johnson", Email = "bob.johnson@example.com", Role = "Admin" };
            await context.Users.AddAsync(user1);
            await context.Users.AddAsync(user2);
            await context.SaveChangesAsync();

            // Act
            var users = await userRepository.GetAllUsersAsync();

            // Assert
            Assert.Equal(3, users.Count);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUserDetails()
        {
            // Arrange
            var context = GetContext();
            var userRepository = GetRepository();
            var user = new User { Id = Guid.NewGuid().ToString(), Name = "Charlie", Lastname = "Brown", Email = "charlie.brown@example.com", Role = "User" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            user.Name = "Charles";

            // Act
            var updatedUserId = await userRepository.UpdateUserAsync(user);

            // Assert
            var updatedUser = await context.Users.FirstOrDefaultAsync(u => u.Id == updatedUserId);
            Assert.NotNull(updatedUser);
            Assert.Equal("Charles", updatedUser.Name);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldRemoveUserFromDatabase()
        {
            // Arrange
            var context = GetContext();
            var userRepository = GetRepository();
            var user = new User { Id = Guid.NewGuid().ToString(), Name = "David", Lastname = "Wilson", Email = "david.wilson@example.com", Role = "User" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            var result = await userRepository.DeleteUserAsync(user);

            // Assert
            Assert.True(result);
            var deletedUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            Assert.Null(deletedUser);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldThrowArgumentNullException_WhenUserIsNull()
        {
            // Arrange
            var userRepository = GetRepository();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => userRepository.CreateUserAsync(null));
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrowArgumentNullException_WhenUserIsNull()
        {
            // Arrange
            var userRepository = GetRepository();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => userRepository.UpdateUserAsync(null));
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldThrowArgumentNullException_WhenUserIsNull()
        {
            // Arrange
            var userRepository = GetRepository();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => userRepository.DeleteUserAsync(null));
        }
    }
}
