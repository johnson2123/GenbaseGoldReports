namespace GenbaseGoldReports.Application.Features.Sales.Commands.CreateJewellerySale
{
    public record JewellerySaleLineDto
    {
        public string Stkcode { get; init; } = string.Empty;
        public string? Stksubcode { get; init; }
        public int Pieces { get; init; }
        public decimal Grqty { get; init; }
        public decimal Apurity { get; init; }
        public decimal Mkgrate { get; init; }
        public decimal Mkgtaxrate { get; init; }
        public decimal Mtltaxrate { get; init; }
    }
}
