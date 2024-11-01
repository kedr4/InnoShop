using FluentValidation;
using static ProductService.Controllers.ProductsController;

namespace ProductService.Validators
{
    public class ProductUpdateValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Название не может быть пустым.")
                .When(p => !string.IsNullOrEmpty(p.Name));

            RuleFor(p => p.Description).NotEmpty().WithMessage("Описание не может быть пустым.")
                .When(p => !string.IsNullOrEmpty(p.Description)); 

            RuleFor(p => p.Price).GreaterThanOrEqualTo(0).WithMessage("Цена должна быть неотрицательной.")
                .When(p => p.Price.HasValue); 

            RuleFor(p => p.IsAvailable).Must(value => value == true || value == false)
                .WithMessage("Доступность должна быть true или false.")
                .When(p => p.IsAvailable.HasValue); 

            RuleFor(p => p.Quantity).GreaterThanOrEqualTo(0).WithMessage("Количество должно быть неотрицательным.")
                .When(p => p.Quantity.HasValue); 
        }
    }
}
