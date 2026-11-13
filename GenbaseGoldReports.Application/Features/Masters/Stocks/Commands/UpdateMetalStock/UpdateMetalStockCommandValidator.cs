using FluentValidation;

namespace GenbaseGoldReports.Application.Features.Masters.Stocks.Commands.UpdateMetalStock
{
    public class UpdateMetalStockCommandValidator : AbstractValidator<UpdateMetalStockCommand>
    {
        public UpdateMetalStockCommandValidator()
        {
            RuleFor(x => x.Stkcode)
                .NotEmpty().WithMessage("Item Code is required.")
                .MaximumLength(55).WithMessage("Item Code must not exceed 55 characters.");

            RuleFor(x => x.Stkname)
                .NotEmpty().WithMessage("Item Name is required.")
                .MaximumLength(55).WithMessage("Item Name must not exceed 55 characters.");

            RuleFor(x => x.Mtltype)
                .NotEmpty().WithMessage("Metal Type is required.")
                .MaximumLength(1).WithMessage("Metal Type must be 1 character.");

            RuleFor(x => x.Karat)
                .NotEmpty().WithMessage("Karat is required.")
                .MaximumLength(2).WithMessage("Karat must not exceed 2 characters.");

            RuleFor(x => x.Spurity)
                .GreaterThan(0).WithMessage("Standard Purity must be greater than 0.")
                .LessThanOrEqualTo(1.00000m).WithMessage("Standard Purity cannot exceed 1.00000.");

            RuleFor(x => x.Salpurity)
                .GreaterThan(0).WithMessage("Sales Purity must be greater than 0.")
                .LessThanOrEqualTo(1.00000m).WithMessage("Sales Purity cannot exceed 1.00000.");

            RuleFor(x => x.Uom)
                .NotEmpty().WithMessage("UOM is required.")
                .MaximumLength(3).WithMessage("UOM must not exceed 3 characters.");

            RuleFor(x => x.Mkgrate)
                .GreaterThanOrEqualTo(0).WithMessage("Making rate cannot be negative.");

            RuleFor(x => x.Taxrate)
                .InclusiveBetween(0, 100).WithMessage("Tax rate must be between 0% and 100%.");

            RuleFor(x => x.Mtltaxrate)
                .InclusiveBetween(0, 100).WithMessage("Metal tax rate must be between 0% and 100%.");

            RuleFor(x => x.Mkgtaxrate)
                .InclusiveBetween(0, 100).WithMessage("Making tax rate must be between 0% and 100%.");
        }
    }
}
