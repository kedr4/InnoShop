using MediatR;
using ProductService.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

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
