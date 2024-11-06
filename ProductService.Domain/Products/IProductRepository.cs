using ProductService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain.Products;

public interface IProductRepository
{
    public Task<int> CreateProductAsync(Product product);
    public Task<Product> GetProductByIdAsync(int Id);
    public Task<List<Product>> GetAllProductsAsync();
    public Task<int> UpdateProductAsync(Product product);
    public Task<bool> DeleteProductAsync(Product product);

}
