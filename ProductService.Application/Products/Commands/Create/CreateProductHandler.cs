using MediatR;
using ProductService.Domain.Products;

namespace ProductService.Application.Products.Commands.Create
{
    internal class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
    {
        private readonly IProductRepository _repository;
        public CreateProductHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<CreateProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var id = await _repository.CreateProductAsync(request.Product);
            return await Task.FromResult(new CreateProductResponse() { Id = id }); ;
        }


    }
}
