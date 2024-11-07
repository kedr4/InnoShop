using MediatR;

namespace ProductService.Application.Products.Commands.Delete
{
    public class DeleteProductCommand : IRequest<DeleteProductResponse>
    {
        public int ProductId { get; set; }
    }
}
