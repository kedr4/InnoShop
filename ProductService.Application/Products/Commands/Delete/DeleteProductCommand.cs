using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.Products.Commands.Delete
{
    public class DeleteProductCommand : IRequest<DeleteProductResponse>
    {
        public int ProductId { get; set; }
    }
}
