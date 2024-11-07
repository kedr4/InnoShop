using MediatR;
using ProductService.Domain.Products;

namespace ProductService.Application.Products.Commands.Update
{
    internal class UpdateProductHandler : IRequestHandler<UpdateProductCommand, UpdateProductResponse>
    {
        private readonly IProductRepository _repository;

        public UpdateProductHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<UpdateProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var existingProduct = await _repository.GetProductByIdAsync(request.Product.Id);
            if (existingProduct == null)
            {
                return new UpdateProductResponse { IsSuccessful = false, Message = "Product not found." };
            }

            existingProduct.Name = request.Product.Name;
            existingProduct.Description = request.Product.Description;
            existingProduct.Price = request.Product.Price;
            existingProduct.IsAvailable = request.Product.IsAvailable;
            existingProduct.Quantity = request.Product.Quantity;

            // Обновляем продукт и получаем идентификатор
            var updatedProductId = await _repository.UpdateProductAsync(existingProduct); // Обновление и получение идентификатора

            return new UpdateProductResponse
            {
                IsSuccessful = updatedProductId > 0, // Успех, если идентификатор больше 0
                Message = updatedProductId > 0 ? "Product updated successfully." : "Failed to update product."
            };
        }
    }
}