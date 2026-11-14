using FluentValidation;

namespace GenbaseGoldReports.Application.Features.Sales.Commands.UpdateJewellerySale
{
    public class UpdateJewellerySaleCommandValidator : AbstractValidator<UpdateJewellerySaleCommand>
    {
        public UpdateJewellerySaleCommandValidator()
        {
            RuleFor(x => x.Brncode).NotEmpty().MaximumLength(10);
            RuleFor(x => x.Divcode).NotEmpty().MaximumLength(10);
            RuleFor(x => x.Doctype).NotEmpty().MaximumLength(10);
            RuleFor(x => x.Docno).GreaterThan(0);
            RuleFor(x => x.Docdate).NotEmpty();

            RuleFor(x => x.Partycode).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Curr).NotEmpty().MaximumLength(10);
            RuleFor(x => x.Currrate).GreaterThan(0);
            RuleFor(x => x.Fixunfix)
                .NotEmpty()
                .Must(v => v == "FIX" || v == "UNFIX")
                .WithMessage("Fixunfix must be FIX or UNFIX.");

            RuleFor(x => x.Gozrate).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Lines).NotEmpty().WithMessage("At least one line item is required.");

            RuleForEach(x => x.Lines).ChildRules(line =>
            {
                line.RuleFor(l => l.Stkcode).NotEmpty().MaximumLength(50);
                line.RuleFor(l => l.Pieces).GreaterThanOrEqualTo(0);
                line.RuleFor(l => l.Grqty).GreaterThan(0);
                line.RuleFor(l => l.Apurity).GreaterThan(0).LessThanOrEqualTo(1);
                line.RuleFor(l => l.Mkgrate).GreaterThanOrEqualTo(0);
                line.RuleFor(l => l.Mkgtaxrate).GreaterThanOrEqualTo(0);
                line.RuleFor(l => l.Mtltaxrate).GreaterThanOrEqualTo(0);
            });
        }
    }
}
