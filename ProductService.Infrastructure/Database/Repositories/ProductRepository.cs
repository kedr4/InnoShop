using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Products;
using ProductService.Models;

namespace ProductService.Infrastructure.Database.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DatabaseContext _context;

        public ProductRepository(DatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> CreateProductAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product.Id; // Assuming `Id` is the primary key
        }

        public async Task<bool> DeleteProductAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            _context.Products.Remove(product);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public Task<Product> GetProductById(Product product)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<List<Product>> GetAllProductsAsync(Product product)
        {
            throw new NotImplementedException();
        }

        public async Task<int> UpdateProductAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product.Id;
        }
    }
}

