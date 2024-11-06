using MediatR;
using ProductService.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.Products.Commands.Delete
{
    internal class DeleteProductHandler : IRequestHandler<DeleteProductCommand, DeleteProductResponse>
    {
        private readonly IProductRepository _repository;

        public DeleteProductHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<DeleteProductResponse> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetProductByIdAsync(request.ProductId);
            if (product == null)
            {
                return new DeleteProductResponse { IsSuccessful = false, Message = "Product not found." };
            }

            var result = await _repository.DeleteProductAsync(product);
            return new DeleteProductResponse { IsSuccessful = result, Message = result ? "Product deleted successfully." : "Failed to delete product." };
        }
    }
}
