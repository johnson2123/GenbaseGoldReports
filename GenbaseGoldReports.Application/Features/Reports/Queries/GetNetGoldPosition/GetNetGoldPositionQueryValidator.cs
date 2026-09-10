using FluentValidation;

namespace GenbaseGoldReports.Application.Features.Reports.Queries.GetNetGoldPosition
{
    public class GetNetGoldPositionQueryValidator : AbstractValidator<GetNetGoldPositionQuery>
    {
        public GetNetGoldPositionQueryValidator()
        {
            RuleFor(x => x.AsOfDate)
                .NotEmpty()
                .WithMessage("As-Of Date is required.");

            RuleFor(x => x.GoldRate)
                .GreaterThan(0)
                .WithMessage("Gold Rate must be greater than zero.");
        }
    }
}
