using Moq;
using ProductService.Domain.Products;
using ProductService.Models;

namespace ProductService.Domain.Test.Unit
{
    public class ProductServiceDomainTests
    {

        public class ProductServiceTests
        {
            [Fact]
            public async Task CreateProduct_ShouldCallCreateProductAsync()
            {
                // Arrange
                var mockRepo = new Mock<IProductRepository>();
                var product = new Product
                {
                    Id = 1,
                    Name = "Test Product",
                    Description = "A test product",
                    Price = 9.99M,
                    IsAvailable = true,
                    Quantity = 100,
                    UserId = "user123"
                };

                mockRepo.Setup(repo => repo.CreateProductAsync(It.IsAny<Product>()))
                        .ReturnsAsync(1);

                // Act
                var result = await mockRepo.Object.CreateProductAsync(product);

                // Assert
                Assert.Equal(1, result);  
                mockRepo.Verify(repo => repo.CreateProductAsync(It.IsAny<Product>()), Times.Once); 
            }

            [Fact]
            public async Task GetProductById_ShouldReturnProduct()
            {
                // Arrange
                var mockRepo = new Mock<IProductRepository>();
                var productId = 1;
                var expectedProduct = new Product
                {
                    Id = 1,
                    Name = "Test Product",
                    Description = "A test product",
                    Price = 9.99M,
                    IsAvailable = true,
                    Quantity = 100,
                    UserId = "user123"
                };

               mockRepo.Setup(repo => repo.GetProductByIdAsync(productId))
                        .ReturnsAsync(expectedProduct);

                // Act
                var result = await mockRepo.Object.GetProductByIdAsync(productId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(expectedProduct.Id, result.Id);
                Assert.Equal(expectedProduct.Name, result.Name);
            }
        }
    }
}