using MediatR;
using ProductService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.Products.Commands.Create
{
    public class CreateProductCommand:IRequest<CreateProductResponse>
    {
        public Product Product { get; set; }
    }
}
