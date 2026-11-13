namespace GenbaseGoldReports.Application.Features.Masters.Stocks.Dtos
{
    public class MetalStockFormInitDto
    {
        public List<LookupItemDto> MetalTypes { get; set; } = new();
        public List<KaratLookupDto> Karats { get; set; } = new();
        public List<LookupItemDto> Uoms { get; set; } = new();
        public List<LookupItemDto> Categories { get; set; } = new();
        public List<LookupItemDto> SubCategories { get; set; } = new();
        public List<LookupItemDto> Types { get; set; } = new();
        public List<LookupItemDto> Brands { get; set; } = new();
        public List<LookupItemDto> Lccs { get; set; } = new();
        public List<LookupItemDto> MakingLccs { get; set; } = new();
        public List<string> DocTypes { get; set; } = new() { "MSTK", "GSTK" };

        // Suggested sequential metadata
        public int SuggestedDocNo { get; set; }
        public string SuggestedSlNo { get; set; } = string.Empty;
    }

}
