using MediatR;
using ProductService.Models;

namespace ProductService.Application.Products.Querys
{
    public class GetProductsQuery : IRequest<List<Product>>
    {
    }
}
