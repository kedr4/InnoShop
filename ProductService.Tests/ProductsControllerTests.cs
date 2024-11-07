using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Controllers;
using ProductService.Models;
using System.Security.Claims;

public class ProductsControllerTests
{
    private readonly DbContextOptions<ProductContext> _contextOptions;
    private readonly ProductContext _context;

    public ProductsControllerTests()
    {
        _contextOptions = new DbContextOptionsBuilder<ProductContext>()
            .UseInMemoryDatabase("InMemoryDb")
            .Options;

        _context = new ProductContext(_contextOptions);
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        _context.Products.AddRange(new List<Product>
        {
            new Product { Id = 1, Name = "Apple", Price = 1.00m, IsAvailable = true, Quantity = 100, UserId = "user1" },
            new Product { Id = 2, Name = "Banana", Price = 0.50m, IsAvailable = true, Quantity = 200, UserId = "user1" },
            new Product { Id = 3, Name = "Cherry", Price = 2.00m, IsAvailable = false, Quantity = 0, UserId = "user2" }
        });
        _context.SaveChanges();
    }

    private ProductsController CreateControllerWithUser(string userId)
    {
        var controller = new ProductsController(_context);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuthentication"));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        return controller;
    }

    //[Fact]
    //public async Task CreateProduct_ValidProduct_ReturnsCreatedResult()
    //{
    //    var controller = CreateControllerWithUser("user1"); 
    //    var productDto = new ProductsController.ProductCreateDto
    //    {
    //        Name = "Test Product",
    //        Description = "Test Description",
    //        Price = 10,
    //        IsAvailable = true,
    //        Quantity = 5
    //    };

    //    var result = await controller.CreateProduct(productDto);

    //    var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
    //    Assert.NotNull(createdResult);
    //    var product = Assert.IsType<Product>(createdResult.Value);
    //    Assert.Equal("Test Product", product.Name);
    //}

    [Fact]
    public async Task GetAllProducts_ReturnsProductList()
    {
        var controller = CreateControllerWithUser("user");
        var result = await controller.GetAllProducts();

        var okResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);

        Assert.Equal(3, products.Count());
    }
}
