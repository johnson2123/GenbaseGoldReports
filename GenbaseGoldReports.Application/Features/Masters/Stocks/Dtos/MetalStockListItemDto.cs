namespace GenbaseGoldReports.Application.Features.Masters.Stocks.Dtos
{
    public class MetalStockListItemDto
    {
        public string Stkcode { get; set; } = string.Empty;
        public string Stksubcode { get; set; } = string.Empty;
        public string? Stkname { get; set; }
        public string? Doctype { get; set; }
        public int? Docno { get; set; }
        public string? Slno { get; set; }
        public string? Mtltype { get; set; }
        public string? Karat { get; set; }
        public decimal? Spurity { get; set; }
        public decimal? Salpurity { get; set; }
        public string? Uom { get; set; }
        public string? Cat { get; set; }
        public string? Subcat { get; set; }
        public string? Type { get; set; }
        public string? Brand { get; set; }
        public decimal? Mkgrate { get; set; }
        public bool Isratebypcs { get; set; }
        public decimal? Taxrate { get; set; }
        public decimal? Cost { get; set; }
        public DateTime? Docdate { get; set; }
    }
}
