namespace GenbaseGoldReports.Application.Features.Masters.Stocks.Queries.GetMetalStockById
{
    public record MetalStockDetailDto
    {
        public string Stkcode { get; init; } = string.Empty;
        public string Stksubcode { get; init; } = string.Empty;
        public string? Stkname { get; init; }
        public string? Stkdetails { get; init; }
        public string? Doctype { get; init; }
        public int? Docno { get; init; }
        public string? Slno { get; init; }
        public DateTime? Docdate { get; init; }

        public string? Mtltype { get; init; }
        public string? Karat { get; init; }
        public decimal? Spurity { get; init; }
        public decimal? Salpurity { get; init; }
        public decimal? Apurity { get; init; }

        public string? Uom { get; init; }
        public string? Cat { get; init; }
        public string? Subcat { get; init; }
        public string? Type { get; init; }
        public string? Brand { get; init; }
        public string? Lcccode { get; init; }
        public string? Mkglcccode { get; init; }

        public decimal? Mkgrate { get; init; }
        public bool Isratebypcs { get; init; }
        public decimal? Taxrate { get; init; }
        public decimal? Mtltaxrate { get; init; }
        public decimal? Mkgtaxrate { get; init; }

        public bool Ispcs { get; init; }
        public bool Isweight { get; init; }
        public bool Isstone { get; init; }
        public bool Isscrap { get; init; }
        public bool Allowminus { get; init; }
        public decimal? Pcweight { get; init; }

        public decimal? Cost { get; init; }
        public decimal? Costfc { get; init; }
        public string? Curr { get; init; }
        public decimal? Currrate { get; init; }

        public string? Piclocation { get; init; }
        public string? Tray { get; init; }
        public string? Ctry { get; init; }
    }
}
