using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;
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

        // POST: api/Products
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] ProductCreateDto productDto)
        {
            if (productDto == null || string.IsNullOrWhiteSpace(productDto.Name))
            {
                return BadRequest("Название продукта не может быть пустым.");
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
            if (updatedProductDto == null)
            {
                return BadRequest("Данные продукта не могут быть пустыми.");
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound("Продукт не найден.");
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

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
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

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
