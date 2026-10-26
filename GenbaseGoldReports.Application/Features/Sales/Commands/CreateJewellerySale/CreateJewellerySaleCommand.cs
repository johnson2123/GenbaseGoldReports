using MediatR;

namespace GenbaseGoldReports.Application.Features.Sales.Commands.CreateJewellerySale
{
    public record CreateJewellerySaleCommand : IRequest<CreateJewellerySaleResult>
    {
        public string Brncode { get; init; } = "HO";
        public string Divcode { get; init; } = "DV01";
        public string Doctype { get; init; } = "MSJ";
        public DateTime Docdate { get; init; } = DateTime.UtcNow.Date;

        public string Partycode { get; init; } = string.Empty;
        public string? Partyname { get; init; }
        public string? Partytrn { get; init; }
        public string? Addr1 { get; init; }
        public string? Sman { get; init; }

        public string Ratetype { get; init; } = "GMS";
        public decimal Gozrate { get; init; }
        public decimal Usdrate { get; init; } = 3.674m;
        public string Fixunfix { get; init; } = "UNFIX";
        public string Curr { get; init; } = "AED";
        public decimal Currrate { get; init; } = 1.0m;
        public string Impexptype { get; init; } = "B2BSAL";

        public decimal Premiumperc { get; init; }
        public decimal Premiumamount { get; init; }
        public decimal Discountperc { get; init; }
        public decimal Discountamount { get; init; }

        public bool Ishedging { get; init; }
        public int HedgingDocno { get; init; }
        public string? Refdocno { get; init; }
        public string? Narration { get; init; }

        public List<JewellerySaleLineDto> Lines { get; init; } = new();
    }
}
