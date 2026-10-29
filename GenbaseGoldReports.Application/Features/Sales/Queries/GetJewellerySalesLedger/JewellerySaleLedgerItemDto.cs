namespace GenbaseGoldReports.Application.Features.Sales.Queries.GetJewellerySalesLedger
{
    public record JewellerySaleLedgerItemDto
    {
        public string Brncode { get; init; } = string.Empty;
        public string Divcode { get; init; } = string.Empty;
        public string Doctype { get; init; } = string.Empty;
        public int Docno { get; init; }
        public string VoucherNumber => $"{Doctype}-{Docno}";
        public DateTime Docdate { get; init; }
        public string Partycode { get; init; } = string.Empty;
        public string Partyname { get; init; } = string.Empty;
        public string Fixunfix { get; init; } = "UNFIX";
        public decimal Totgrqty { get; init; }
        public decimal Totpuqty { get; init; }
        public decimal Totmkgamt { get; init; }
        public decimal Tottaxamt { get; init; }
        public decimal Partyamount { get; init; }
        public string Curr { get; init; } = "AED";
        public string? Refdocno { get; init; }
    }
}
