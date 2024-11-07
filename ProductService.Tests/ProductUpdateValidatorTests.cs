using FluentValidation;

namespace ProductService.Tests
{
    public class ProductUpdateValidatorTests
    {
        private readonly ProductUpdateValidator _validator;

        public ProductUpdateValidatorTests()
        {
            _validator = new ProductUpdateValidator();
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Empty()
        {
            var productDto = new ProductUpdateDto { Name = "", Description = "Valid Description", Price = 10, IsAvailable = true, Quantity = 5 };

            var result = _validator.Validate(productDto);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Empty()
        {
            var productDto = new ProductUpdateDto { Name = "Valid Name", Description = "", Price = 10, IsAvailable = true, Quantity = 5 };

            var result = _validator.Validate(productDto);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Should_Have_Error_When_Price_Is_Negative()
        {
            var productDto = new ProductUpdateDto { Name = "Valid Name", Description = "Valid Description", Price = -1, IsAvailable = true, Quantity = 5 };

            var result = _validator.Validate(productDto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(productDto.Price) && e.ErrorMessage == "Цена должна быть неотрицательной.");
        }

        [Fact]
        public void Should_Have_Error_When_Quantity_Is_Negative()
        {
            var productDto = new ProductUpdateDto { Name = "Valid Name", Description = "Valid Description", Price = 10, IsAvailable = true, Quantity = -1 };

            var result = _validator.Validate(productDto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(productDto.Quantity) && e.ErrorMessage == "Количество должно быть неотрицательным.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Properties_Are_Valid()
        {
            var productDto = new ProductUpdateDto { Name = "Valid Name", Description = "Valid Description", Price = 10, IsAvailable = true, Quantity = 5 };

            var result = _validator.Validate(productDto);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
    }
}
