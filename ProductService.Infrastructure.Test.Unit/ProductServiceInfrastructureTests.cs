using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using ProductService.Infrastructure.Database.Repositories;
using ProductService.Models;
using ProductService.Domain.Products;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductService.Infrastructure.Tests
{
    public class ProductServiceInfrastructureTests
    {
        private readonly DbContextOptions<DatabaseContext> _dbContextOptions;
        private readonly DatabaseContext _dbContext;

        public ProductServiceInfrastructureTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "ProductServiceTestDb")
                .Options;

            _dbContext = new DatabaseContext(_dbContextOptions);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldAddProductToDatabase()
        {
            // Arrange
            var productRepository = new ProductRepository(_dbContext);
            var product = new Product
            {
                Name = "Test Product",
                Description = "Description of test product",
                Price = 100.0m,
                IsAvailable = true,
                Quantity = 5,
                UserId = "user123"
            };

            // Act
            var productId = await productRepository.CreateProductAsync(product);

            // Assert
            var addedProduct = await _dbContext.Products.FindAsync(productId);
            Assert.NotNull(addedProduct);
            Assert.Equal(product.Name, addedProduct.Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var productRepository = new ProductRepository(_dbContext);
            var product = new Product
            {
                Name = "Test Product",
                Description = "Description of test product",
                Price = 100.0m,
                IsAvailable = true,
                Quantity = 5,
                UserId = "user123"
            };

            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            // Act
            var fetchedProduct = await productRepository.GetProductByIdAsync(product.Id);

            // Assert
            Assert.NotNull(fetchedProduct);
            Assert.Equal(product.Id, fetchedProduct.Id);
            Assert.Equal(product.Name, fetchedProduct.Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            var productRepository = new ProductRepository(_dbContext);
            var nonExistentProductId = 999;

            // Act
            var fetchedProduct = await productRepository.GetProductByIdAsync(nonExistentProductId);

            // Assert
            Assert.Null(fetchedProduct);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldRemoveProduct_WhenProductExists()
        {
            // Arrange
            var productRepository = new ProductRepository(_dbContext);
            var product = new Product
            {
                Name = "Test Product",
                Description = "Description of test product",
                Price = 100.0m,
                IsAvailable = true,
                Quantity = 5,
                UserId = "user123"
            };

            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            // Act
            var isDeleted = await productRepository.DeleteProductAsync(product);

            // Assert
            Assert.True(isDeleted);
            var deletedProduct = await _dbContext.Products.FindAsync(product.Id);
            Assert.Null(deletedProduct); 
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldUpdateProduct_WhenProductExists()
        {
            // Arrange
            var productRepository = new ProductRepository(_dbContext);
            var product = new Product
            {
                Name = "Test Product",
                Description = "Description of test product",
                Price = 100.0m,
                IsAvailable = true,
                Quantity = 5,
                UserId = "user123"
            };

            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            // Act
            product.Name = "Updated Product Name";
            var updatedProductId = await productRepository.UpdateProductAsync(product);

            // Assert
            var updatedProduct = await _dbContext.Products.FindAsync(updatedProductId);
            Assert.NotNull(updatedProduct);
            Assert.Equal("Updated Product Name", updatedProduct.Name);
        }
    }
}
