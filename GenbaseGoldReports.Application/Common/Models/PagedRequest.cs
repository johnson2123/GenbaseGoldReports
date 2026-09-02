namespace GenbaseGoldReports.Application.Common.Models
{
    public record PagedRequest
    {
        private const int MaxPageSize = 100;
        private int _pageSize = 10;

        public int PageNumber { get; init; } = 1;

        public int PageSize
        {
            get => _pageSize;
            init => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? 10 : value;
        }

        public string? SearchTerm { get; init; }
    }
}
