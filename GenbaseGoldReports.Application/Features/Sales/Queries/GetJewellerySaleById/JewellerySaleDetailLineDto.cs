namespace GenbaseGoldReports.Application.Features.Sales.Queries.GetJewellerySaleById
{
    public record JewellerySaleDetailLineDto
    {
        public int Slno { get; init; }
        public string Stkcode { get; init; } = string.Empty;
        public string? Stksubcode { get; init; }
        public int Pieces { get; init; }
        public decimal Grqty { get; init; }
        public decimal Apurity { get; init; }
        public decimal Puqty { get; init; }
        public decimal Mkgrate { get; init; }
        public decimal Mkgamt { get; init; }
        public decimal Mtlrate { get; init; }
        public decimal Mtlamt { get; init; }
        public decimal Mkgtaxamt { get; init; }
        public decimal Mtltaxamt { get; init; }
        public decimal Taxamt { get; init; }
        public decimal Totalamt { get; init; }
    }
}
