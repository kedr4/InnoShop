using MediatR;
using ProductService.Domain.Products;
using ProductService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.Products.Querys
{
    public class GetProductsQueryWithFilterHandler : IRequestHandler<GetProductsQueryWithFilter, List<Product>>
    {
        private readonly IProductRepository _repository;

        public GetProductsQueryWithFilterHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Product>> Handle(GetProductsQueryWithFilter request, CancellationToken cancellationToken)
        {
            var products = await _repository.GetAllProductsAsync();

            if (!string.IsNullOrEmpty(request.Name))
            {
                products = products.Where(p => p.Name.Contains(request.Name)).ToList();
            }

            if (request.MinPrice.HasValue)
            {
                products = products.Where(p => p.Price >= request.MinPrice.Value).ToList();
            }

            if (request.MaxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= request.MaxPrice.Value).ToList();
            }

            if (request.IsAvailable.HasValue)
            {
                products = products.Where(p => p.IsAvailable == request.IsAvailable.Value).ToList();
            }

            if (request.Quantity.HasValue)
            {
                products = products.Where(p => p.Quantity == request.Quantity.Value).ToList();
            }

            return products;
        }
    }
}
