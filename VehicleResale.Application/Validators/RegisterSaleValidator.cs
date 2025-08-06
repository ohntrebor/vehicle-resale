using FluentValidation;
using System.Text.RegularExpressions;
using VehicleResale.Application.Commands;

namespace VehicleResale.Application.Validators
{
    public class RegisterSaleValidator : AbstractValidator<RegisterSaleCommand>
    {
        public RegisterSaleValidator()
        {
            RuleFor(x => x.VehicleId)
                .NotEmpty().WithMessage("VehicleId is required");

            RuleFor(x => x.BuyerCpf)
                .NotEmpty().WithMessage("BuyerCpf is required")
                .Must(BeValidCpf).WithMessage("Invalid CPF format");
        }

        private bool BeValidCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            // Remove non-numeric characters
            cpf = Regex.Replace(cpf, @"[^\d]", "");

            // CPF must have 11 digits
            return cpf.Length == 11;
        }
    }
}