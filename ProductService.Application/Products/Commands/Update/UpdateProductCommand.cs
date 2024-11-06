using MediatR;
using ProductService.Application.Products.Commands.Delete;
using ProductService.Models;

namespace ProductService.Application.Products.Commands.Update
{
    public class UpdateProductCommand : IRequest<UpdateProductResponse>
    {
        public Product Product { get; set; }
    }
}
