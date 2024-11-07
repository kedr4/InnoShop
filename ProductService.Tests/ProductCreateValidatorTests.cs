namespace ProductService.Tests
{
    public class ProductCreateValidatorTests
    {
        private readonly ProductCreateValidator _validator;

        public ProductCreateValidatorTests()
        {
            _validator = new ProductCreateValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var productDto = new ProductCreateDto { Name = "", Description = "Valid Description", Price = 10, IsAvailable = true, Quantity = 5 };

            var result = _validator.Validate(productDto);

            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Equal("Название не может быть пустым.", result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void Should_Have_Error_When_Description_Is_Empty()
        {
            var productDto = new ProductCreateDto { Name = "Valid Name", Description = "", Price = 10, IsAvailable = true, Quantity = 5 };

            var result = _validator.Validate(productDto);

            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Equal("Описание не может быть пустым.", result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void Should_Have_Error_When_Price_Is_Negative()
        {
            var productDto = new ProductCreateDto { Name = "Valid Name", Description = "Valid Description", Price = -1, IsAvailable = true, Quantity = 5 };

            var result = _validator.Validate(productDto);

            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Equal("Цена должна быть неотрицательной.", result.Errors[0].ErrorMessage);
        }

        [Fact]
        public void Should_Have_Error_When_Quantity_Is_Negative()
        {
            var productDto = new ProductCreateDto { Name = "Valid Name", Description = "Valid Description", Price = 10, IsAvailable = true, Quantity = -1 };

            var result = _validator.Validate(productDto);

            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Equal("Количество должно быть неотрицательным.", result.Errors[0].ErrorMessage);
        }


        [Fact]
        public void Should_Not_Have_Error_When_All_Properties_Are_Valid()
        {
            var productDto = new ProductCreateDto { Name = "Valid Name", Description = "Valid Description", Price = 10, IsAvailable = true, Quantity = 5 };

            var result = _validator.Validate(productDto);

            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
    }
}
