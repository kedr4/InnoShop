using FluentValidation;
using ProductService.Models;
using static ProductService.Controllers.ProductsController;

namespace ProductService.Validators
{
    public class ProductCreateValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Название не может быть пустым.");
            RuleFor(p => p.Description).NotEmpty().WithMessage("Описание не может быть пустым.");
            RuleFor(p => p.Price).GreaterThanOrEqualTo(0).WithMessage("Цена должна быть неотрицательной.");
            RuleFor(p => p.IsAvailable).Must(value => value == true || value == false).WithMessage("Доступность должна быть true или false.");
            RuleFor(p => p.Quantity).GreaterThanOrEqualTo(0).WithMessage("Количество должно быть неотрицательным.");
            
        }
    }
}

