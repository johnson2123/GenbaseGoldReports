namespace GenbaseGoldReports.Application.Features.Sales.Queries.GetJewellerySaleById
{
    public record JewellerySaleDetailsDto
    {
        public string Brncode { get; init; } = string.Empty;
        public string Divcode { get; init; } = string.Empty;
        public string Doctype { get; init; } = string.Empty;
        public int Docno { get; init; }
        public string VoucherNumber => $"{Doctype}-{Docno}";
        public DateTime Docdate { get; init; }
        public string? Stimestamp { get; init; }

        public string Partycode { get; init; } = string.Empty;
        public string? Partyname { get; init; }
        public string? Partytrn { get; init; }
        public string? Addr1 { get; init; }
        public string? Sman { get; init; }

        public string? Ratetype { get; init; }
        public decimal Gozrate { get; init; }
        public decimal Usdrate { get; init; }
        public decimal Gramrate { get; init; }
        public string Curr { get; init; } = "AED";
        public decimal Currrate { get; init; }
        public string Fixunfix { get; init; } = "UNFIX";
        public string? Impexptype { get; init; }

        public int Totpcs { get; init; }
        public decimal Totgrqty { get; init; }
        public decimal Totpuqty { get; init; }
        public decimal Totmkgamt { get; init; }
        public decimal Totalmtlamt { get; init; }
        public decimal Tottaxamt { get; init; }
        public decimal Totgrossamt { get; init; }
        public decimal Partyamount { get; init; }

        public decimal Premiumperc { get; init; }
        public decimal Premiumamount { get; init; }
        public decimal Discountperc { get; init; }
        public decimal Discountamount { get; init; }
        public int Ishedging { get; init; }
        public int Autohedgingdocno { get; init; }
        public string? Refdocno { get; init; }
        public string? Narration { get; init; }

        public List<JewellerySaleDetailLineDto> Lines { get; init; } = new();
    }
}
