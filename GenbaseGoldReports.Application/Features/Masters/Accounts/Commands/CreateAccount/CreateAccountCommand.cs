using GenbaseGoldReports.Application.Features.Masters.Accounts.Dtos;
using MediatR;

namespace GenbaseGoldReports.Application.Features.Masters.Accounts.Commands.CreateAccount
{
    public record CreateAccountCommand : IRequest<AccountTreeItemDto>
    {
        public bool IsGroup { get; init; }
        public string Acname { get; init; } = string.Empty;
        public string? Accode { get; init; } // Optional: server generates next if omitted/blank
        public string ParentCode { get; init; } = "0";
        public string Actype { get; init; } = "G";
        public string Class { get; init; } = "010";
        public string Curr { get; init; } = "AED";
        public decimal Currrate { get; init; } = 1.0m;

        public string? Trnno { get; init; }
        public decimal Taxrate { get; init; } = 0.0m;
        public bool Isvatgroup { get; init; }

        public string? Addr1 { get; init; }
        public string? Addr2 { get; init; }
        public string? Addr3 { get; init; }
        public string? City { get; init; }
        public string? Country { get; init; }
        public string? Area { get; init; } = "DXB";
        public string? Phone { get; init; }
        public string? Mobile { get; init; }
        public string? Contactperson { get; init; }
        public string? Email { get; init; }
        public string? Website { get; init; }

        public string? Licencenumber { get; init; }
        public string? Licencedate { get; init; }

        public decimal Purusdrate { get; init; } = 0m;
        public decimal Salusdrate { get; init; } = 0m;
        public decimal Marginperc { get; init; } = 0m;
        public decimal Marginconsumeperc { get; init; } = 0m;

        public List<AccountCurrencyInputDto> AdditionalCurrencies { get; init; } = new();
    }
}
