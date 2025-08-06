using FluentValidation;
using VehicleResale.Application.Commands;

namespace VehicleResale.Application.Validators
{
    public class CreateVehicleValidator : AbstractValidator<CreateVehicleCommand>
    {
        public CreateVehicleValidator()
        {
            RuleFor(x => x.Brand)
                .NotEmpty().WithMessage("Brand is required")
                .MaximumLength(50).WithMessage("Brand must not exceed 50 characters");

            RuleFor(x => x.Model)
                .NotEmpty().WithMessage("Model is required")
                .MaximumLength(50).WithMessage("Model must not exceed 50 characters");

            RuleFor(x => x.Year)
                .InclusiveBetween(1900, 2030).WithMessage("Year must be between 1900 and 2030");

            RuleFor(x => x.Color)
                .NotEmpty().WithMessage("Color is required")
                .MaximumLength(30).WithMessage("Color must not exceed 30 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0");
        }
    }
}