using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserService.Domain.Users;
using Xunit;

namespace UserService.Tests.Repositories
{
    public class UserDomainTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;

        public UserDomainTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
        }

        [Fact]
        public async Task CreateUserAsync_ReturnsUserId_WhenUserIsCreated()
        {
            var user = new User
            {
                Id = "123",
                UserName = "testuser@example.com",
                Name = "Test",
                Lastname = "User",
                Role = "Admin"
            };

            _userRepositoryMock.Setup(repo => repo.CreateUserAsync(It.IsAny<User>()))
                .ReturnsAsync(user.Id);

            // Act
            var result = await _userRepositoryMock.Object.CreateUserAsync(user);

            // Assert
            Assert.Equal(user.Id, result);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var userId = "123";
            var user = new User
            {
                Id = userId,
                UserName = "testuser@example.com",
                Name = "Test",
                Lastname = "User",
                Role = "Admin"
            };

            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userRepositoryMock.Object.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsListOfUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = "123", UserName = "user1@example.com", Name = "User1", Lastname = "One", Role = "Admin" },
                new User { Id = "124", UserName = "user2@example.com", Name = "User2", Lastname = "Two", Role = "User" }
            };

            _userRepositoryMock.Setup(repo => repo.GetAllUsersAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _userRepositoryMock.Object.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task UpdateUserAsync_ReturnsUserId_WhenUserIsUpdated()
        {
            // Arrange
            var user = new User
            {
                Id = "123",
                UserName = "testuser@example.com",
                Name = "Test",
                Lastname = "User",
                Role = "Admin"
            };

            _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(It.IsAny<User>()))
                .ReturnsAsync(user.Id);

            // Act
            var result = await _userRepositoryMock.Object.UpdateUserAsync(user);

            // Assert
            Assert.Equal(user.Id, result);
        }

        [Fact]
        public async Task DeleteUserAsync_ReturnsTrue_WhenUserIsDeleted()
        {
            // Arrange
            var user = new User
            {
                Id = "123",
                UserName = "testuser@example.com",
                Name = "Test",
                Lastname = "User",
                Role = "Admin"
            };

            _userRepositoryMock.Setup(repo => repo.DeleteUserAsync(It.IsAny<User>()))
                .ReturnsAsync(true);

            // Act
            var result = await _userRepositoryMock.Object.DeleteUserAsync(user);

            // Assert
            Assert.True(result);
        }
    }
}
