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
