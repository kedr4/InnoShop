using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Api.Controllers.Products.Requests;
using ProductService.Application.Products.Commands.Create;
using ProductService.Application.Products.Commands.Delete;
using ProductService.Application.Products.Commands.Update;
using ProductService.Application.Products.Querys;
using ProductService.Models;


namespace ProductService.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }



        // POST: api/Products
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProductAsync([FromBody] CreateProductRequest request)
        {

            var cmd = new CreateProductCommand()

            {
                Product = new Product()
                {
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    IsAvailable = request.IsAvailable,
                    Quantity = request.Quantity,
                    CreatedAt = DateTime.UtcNow
                }
            };
            var id = await _mediator.Send(cmd);


            return Ok(id);
        }


        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProductsAsync()
        {
            var query = new GetProductsQuery();
            var products = await _mediator.Send(query);
            return Ok(products);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductByIdAsync(int id)
        {
            var query = new GetProductByIdQuery { Id = id };
            var product = await _mediator.Send(query);

            if (product == null)
                return NotFound();

            return Ok(product);
        }


        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync(int id, [FromBody] UpdateProductRequest request)
        {
            var cmd = new UpdateProductCommand
            {
                Product = new Product
                {
                    Id = id,
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    IsAvailable = request.IsAvailable,
                    Quantity = request.Quantity
                }
            };

            var response = await _mediator.Send(cmd);

            if (!response.IsSuccessful)
                return NotFound(response.Message);

            return NoContent();
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            var cmd = new DeleteProductCommand { ProductId = id };
            var response = await _mediator.Send(cmd);

            if (!response.IsSuccessful)
                return NotFound(response.Message);

            return NoContent();
        }

        // GET: api/products/filter
        [HttpGet("filter")]
        public async Task<ActionResult<List<Product>>> GetProductsByFilterAsync([FromQuery] string name, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] bool? isAvailable, [FromQuery] int? quantity)
        {
            var query = new GetProductsQueryWithFilter
            {
                Name = name,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                IsAvailable = isAvailable,
                Quantity = quantity
            };

            var products = await _mediator.Send(query);
            return Ok(products);
        }


    }
}
