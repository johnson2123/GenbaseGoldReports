using FluentValidation;

namespace GenbaseGoldReports.Application.Features.Reports.Queries.GetTrialBalance
{
    public class GetTrialBalanceQueryValidator : AbstractValidator<GetTrialBalanceQuery>
    {
        public GetTrialBalanceQueryValidator()
        {
            RuleFor(x => x.AsOfDate)
                .NotEmpty()
                .WithMessage("As-Of Date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow.AddYears(1))
                .WithMessage("As-Of Date cannot be in the distant future.");

            RuleFor(x => x.GoldRate)
                .GreaterThan(0)
                .WithMessage("Gold Rate must be greater than zero.");

            RuleFor(x => x.SilverRate)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Silver Rate cannot be negative.");
        }
    }
}
