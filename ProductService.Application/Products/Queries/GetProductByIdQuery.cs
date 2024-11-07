using MediatR;
using ProductService.Models;

namespace ProductService.Application.Products.Querys
{
    public class GetProductByIdQuery : IRequest<Product>
    {
        public int Id { get; set; }
    }
}
