using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.Products.Commands.Delete
{
    public class DeleteProductResponse
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
    }
}
