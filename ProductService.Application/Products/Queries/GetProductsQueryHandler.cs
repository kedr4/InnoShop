using MediatR;
using ProductService.Domain.Products;
using ProductService.Models;

namespace ProductService.Application.Products.Querys
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<Product>>
    {
        private readonly IProductRepository _repository;

        public GetProductsQueryHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetAllProductsAsync();
        }
    }
}
