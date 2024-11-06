using FluentValidation;
using ProductService.Api.Controllers.Products.Requests;
using static ProductService.Controllers.ProductsController;

namespace ProductService.Api.Controllers.Products.Validators
{
    public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductRequestValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Название не может быть пустым.")
                .When(p => !string.IsNullOrEmpty(p.Name));

            RuleFor(p => p.Description).NotEmpty().WithMessage("Описание не может быть пустым.")
                .When(p => !string.IsNullOrEmpty(p.Description));

            RuleFor(p => p.Price).GreaterThanOrEqualTo(0).WithMessage("Цена должна быть неотрицательной.");

            RuleFor(p => p.IsAvailable).Must(value => value == true || value == false)
                .WithMessage("Доступность должна быть true или false.");

            RuleFor(p => p.Quantity).GreaterThanOrEqualTo(0).WithMessage("Количество должно быть неотрицательным.");
        }
    }
}
