using FluentValidation;

namespace GenbaseGoldReports.Application.Features.Reports.Queries.GetJewelleryStockBalance
{
    public class GetJewelleryStockBalanceQueryValidator : AbstractValidator<GetJewelleryStockBalanceQuery>
    {
        public GetJewelleryStockBalanceQueryValidator()
        {
            RuleFor(x => x.AsOfDate)
                .NotEmpty()
                .WithMessage("As-Of Date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
                .WithMessage("As-Of Date cannot be in the future.");

            RuleFor(x => x.GoldRate)
                .GreaterThan(0)
                .WithMessage("Gold Rate must be greater than zero.");

            RuleFor(x => x.SearchItemCode)
                .MaximumLength(50)
                .When(x => !string.IsNullOrWhiteSpace(x.SearchItemCode))
                .WithMessage("Search Item Code cannot exceed 50 characters.");
        }
    }
}
