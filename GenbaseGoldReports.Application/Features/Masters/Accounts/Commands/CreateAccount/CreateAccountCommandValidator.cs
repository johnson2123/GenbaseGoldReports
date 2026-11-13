using FluentValidation;

namespace GenbaseGoldReports.Application.Features.Masters.Accounts.Commands.CreateAccount
{
    public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
    {
        public CreateAccountCommandValidator()
        {
            RuleFor(x => x.Acname)
                .NotEmpty().WithMessage("Account / Group Name is required.")
                .MaximumLength(100).WithMessage("Account Name cannot exceed 100 characters.");

            RuleFor(x => x.ParentCode)
                .NotEmpty().WithMessage("Parent Group selection is required for ledger accounts.")
                .When(x => !x.IsGroup);

            RuleFor(x => x.ParentCode)
                .MaximumLength(10).WithMessage("Parent Code cannot exceed 10 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.ParentCode));

            RuleFor(x => x.Actype)
                .NotEmpty().WithMessage("Account Type is required.")
                .MaximumLength(5).WithMessage("Account Type code cannot exceed 5 characters.");

            RuleFor(x => x.Class)
                .NotEmpty().WithMessage("Account Class is required.")
                .MaximumLength(10).WithMessage("Class code cannot exceed 10 characters.");

            RuleFor(x => x.Curr)
                .NotEmpty().WithMessage("Currency is required.")
                .Length(3).WithMessage("Currency code must be exactly 3 characters.");

            RuleFor(x => x.Currrate)
                .GreaterThan(0).WithMessage("Currency rate must be greater than zero.");

            RuleFor(x => x.Taxrate)
                .GreaterThanOrEqualTo(0).WithMessage("Tax rate cannot be negative.")
                .LessThanOrEqualTo(100).WithMessage("Tax rate cannot exceed 100%.");

            // Optional fields length restrictions based on Accmst DB schema
            RuleFor(x => x.Accode)
                .MaximumLength(20).WithMessage("Account Code cannot exceed 20 characters.");

            RuleFor(x => x.Trnno)
                .MaximumLength(30).WithMessage("TRN No cannot exceed 30 characters.")
                .Matches(@"^\d{15}$")
                .When(x => !string.IsNullOrWhiteSpace(x.Trnno) && x.Trnno.Trim() != "0")
                .WithMessage("TRN must be exactly a 15-digit number.");

            RuleFor(x => x.Addr1)
                .MaximumLength(100).WithMessage("Address 1 cannot exceed 100 characters.");

            RuleFor(x => x.Addr2)
                .MaximumLength(100).WithMessage("Address 2 cannot exceed 100 characters.");

            RuleFor(x => x.Addr3)
                .MaximumLength(100).WithMessage("Address 3 cannot exceed 100 characters.");

            RuleFor(x => x.City)
                .MaximumLength(50).WithMessage("City cannot exceed 50 characters.");

            RuleFor(x => x.Country)
                .MaximumLength(50).WithMessage("Country cannot exceed 50 characters.");

            RuleFor(x => x.Area)
                .MaximumLength(20).WithMessage("Area cannot exceed 20 characters.");

            RuleFor(x => x.Phone)
                .MaximumLength(50).WithMessage("Phone cannot exceed 50 characters.");

            RuleFor(x => x.Mobile)
                .MaximumLength(50).WithMessage("Mobile cannot exceed 50 characters.");

            RuleFor(x => x.Contactperson)
                .MaximumLength(50).WithMessage("Contact Person cannot exceed 50 characters.");

            RuleFor(x => x.Email)
                .MaximumLength(50).WithMessage("Email cannot exceed 50 characters.")
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("A valid email address is required.");

            RuleFor(x => x.Website)
                .MaximumLength(100).WithMessage("Website cannot exceed 100 characters.");

            RuleFor(x => x.Licencenumber)
                .MaximumLength(30).WithMessage("License Number cannot exceed 30 characters.");

            RuleFor(x => x.Purusdrate)
                .GreaterThanOrEqualTo(0).WithMessage("Purchase USD rate cannot be negative.");

            RuleFor(x => x.Salusdrate)
                .GreaterThanOrEqualTo(0).WithMessage("Sales USD rate cannot be negative.");

            RuleFor(x => x.Marginperc)
                .GreaterThanOrEqualTo(0).WithMessage("Margin percentage cannot be negative.");

            RuleFor(x => x.Marginconsumeperc)
                .GreaterThanOrEqualTo(0).WithMessage("Margin consume percentage cannot be negative.");

            RuleForEach(x => x.AdditionalCurrencies).ChildRules(currency =>
            {
                currency.RuleFor(c => c.Currcode)
                    .NotEmpty().WithMessage("Additional currency code is required.")
                    .Length(3).WithMessage("Additional currency code must be 3 characters.");
            });
        }
    }
}
