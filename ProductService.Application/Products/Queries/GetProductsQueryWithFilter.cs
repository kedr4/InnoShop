using MediatR;
using ProductService.Models;

namespace ProductService.Application.Products.Querys
{
    public class GetProductsQueryWithFilter : IRequest<List<Product>>
    {
        public string Name { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsAvailable { get; set; }
        public int? Quantity { get; set; }

    }
}
