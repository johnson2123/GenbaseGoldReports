using MediatR;

namespace GenbaseGoldReports.Application.Features.Masters.Stocks.Commands.UpdateMetalStock
{
    public record UpdateMetalStockCommand : IRequest<bool>
    {
        public string Stkcode { get; init; } = null!;
        public string? Stkname { get; init; }
        public string? Stkdetails { get; init; }
        public string Doctype { get; init; } = "MSTK";
        public string Mtltype { get; init; } = "G";
        public string Karat { get; init; } = null!;
        public decimal Spurity { get; init; }
        public decimal Salpurity { get; init; }
        public string Uom { get; init; } = "GMS";
        public string Cat { get; init; } = "GEN";
        public string? Subcat { get; init; }
        public string Type { get; init; } = "GEN";
        public string Brand { get; init; } = "GEN";
        public string Lcccode { get; init; } = "GOLD";
        public string Mkglcccode { get; init; } = "GMKG";
        public decimal Mkgrate { get; init; } = 0.00m;
        public bool Isratebypcs { get; init; } = false;
        public decimal Taxrate { get; init; } = 5.00m;
        public decimal Mtltaxrate { get; init; } = 0.00m;
        public decimal Mkgtaxrate { get; init; } = 5.00m;
        public bool Ispcs { get; init; } = true;
        public bool Isweight { get; init; } = true;
        public bool Isstone { get; init; } = false;
        public bool Isscrap { get; init; } = false;
        public bool Allowminus { get; init; } = false;
        public decimal Pcweight { get; init; } = 0.00m;
        public decimal Cost { get; init; } = 0.00m;
        public string Curr { get; init; } = "AED";
        public decimal Currrate { get; init; } = 1.00000m;
        public string? Piclocation { get; init; }
        public string? Tray { get; init; }
        public string? Ctry { get; init; }
    }
}
