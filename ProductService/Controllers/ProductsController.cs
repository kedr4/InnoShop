using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;
using ProductService.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ProductsController : ControllerBase
    {
        private readonly ProductContext _context;

        public ProductsController(ProductContext context)
        {
            _context = context;
        }

        public class ProductUpdateDto
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal? Price { get; set; }
            public bool? IsAvailable { get; set; }
            public int? Quantity { get; set; }
        }

        public class ProductCreateDto
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public bool IsAvailable { get; set; }
            public int Quantity { get; set; }
        }

        // POST: api/Products
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] ProductCreateDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Пользователь не авторизован.");
            }

            string userId = userIdClaim.Value;

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                IsAvailable = productDto.IsAvailable,
                Quantity = productDto.Quantity,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }



        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            return await _context.Products.ToListAsync();
        }

        // GET: api/Products/filter
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Product>>> FilterProducts(
            string? name = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? isAvailable = null)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (isAvailable.HasValue)
            {
                query = query.Where(p => p.IsAvailable == isAvailable.Value);
            }

            return await query.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto updatedProductDto)
        {
            var validator = new ProductUpdateValidator();
            var validationResult = await validator.ValidateAsync(updatedProductDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound("Продукт не найден.");
            }

            if (!IsProductOwner(existingProduct))
            {
                return StatusCode(403, "Вы не можете редактировать этот товар.");
            }

            if (!string.IsNullOrWhiteSpace(updatedProductDto.Name))
            {
                existingProduct.Name = updatedProductDto.Name;
            }

            if (!string.IsNullOrWhiteSpace(updatedProductDto.Description))
            {
                existingProduct.Description = updatedProductDto.Description;
            }

            if (updatedProductDto.Price.HasValue)
            {
                existingProduct.Price = updatedProductDto.Price.Value;
            }

            if (updatedProductDto.IsAvailable.HasValue)
            {
                existingProduct.IsAvailable = updatedProductDto.IsAvailable.Value;
            }

            if (updatedProductDto.Quantity.HasValue)
            {
                existingProduct.Quantity = updatedProductDto.Quantity.Value;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            if (!IsProductOwner(product))
            {
                return StatusCode(403, "Вы не можете редактировать этот товар.");
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
       
        private bool IsProductOwner(Product product)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null && product.UserId == userIdClaim.Value;
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
