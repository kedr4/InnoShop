using MediatR;
using ProductService.Domain.Products;
using ProductService.Models;

namespace ProductService.Application.Products.Querys
{
    namespace ProductService.Application.Products.Queries
    {
        public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product>
        {
            private readonly IProductRepository _repository;

            public GetProductByIdQueryHandler(IProductRepository repository)
            {
                _repository = repository;
            }

            public async Task<Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
            {
                return await _repository.GetProductByIdAsync(request.Id);
            }
        }
    }
}
