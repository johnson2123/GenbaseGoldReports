using FluentValidation;

namespace GenbaseGoldReports.Application.Features.Sales.Commands.CreateJewellerySale
{
    public class CreateJewellerySaleCommandValidator : AbstractValidator<CreateJewellerySaleCommand>
    {
        public CreateJewellerySaleCommandValidator()
        {
            RuleFor(x => x.Brncode).NotEmpty().MaximumLength(10);
            RuleFor(x => x.Divcode).NotEmpty().MaximumLength(10);
            RuleFor(x => x.Doctype).NotEmpty().MaximumLength(10);
            RuleFor(x => x.Partycode).NotEmpty().WithMessage("Party/Customer is required.");
            RuleFor(x => x.Gozrate).GreaterThan(0).WithMessage("Base gold rate per oz must be greater than zero.");
            RuleFor(x => x.Fixunfix).Must(x => x == "FIX" || x == "UNFIX")
                .WithMessage("Fix Type must be either 'FIX' or 'UNFIX'.");
            RuleFor(x => x.Lines).NotEmpty().WithMessage("At least one sale line item is required.");

            RuleForEach(x => x.Lines).ChildRules(line =>
            {
                line.RuleFor(l => l.Stkcode).NotEmpty().WithMessage("Stock item is required.");
                line.RuleFor(l => l.Grqty).GreaterThan(0).WithMessage("Gross weight must be positive.");
                line.RuleFor(l => l.Apurity).InclusiveBetween(0.001m, 1.000m)
                    .WithMessage("Purity factor must be between 0.001 and 1.000.");
                line.RuleFor(l => l.Pieces).GreaterThanOrEqualTo(0);
                line.RuleFor(l => l.Mkgrate).GreaterThanOrEqualTo(0);
            });
        }
    }
}
