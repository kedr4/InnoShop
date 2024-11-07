using MediatR;
using ProductService.Models;

namespace ProductService.Application.Products.Commands.Create
{
    public class CreateProductCommand : IRequest<CreateProductResponse>
    {
        public Product Product { get; set; }
    }
}
