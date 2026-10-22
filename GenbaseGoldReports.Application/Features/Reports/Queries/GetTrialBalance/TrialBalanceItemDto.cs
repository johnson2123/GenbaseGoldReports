namespace GenbaseGoldReports.Application.Features.Reports.Queries.GetTrialBalance
{
    public record TrialBalanceItemDto(
        decimal KeyCode,
        string AccountCode,
        string AccountName,
        int LevelId,
        string AccountType,
        bool IsAccount,
        int GroupCode,
        int MainGroup,
        decimal? LcAmount,
        decimal? GoldGrams,
        decimal? SilverGrams,
        decimal? NetAmount,
        bool IsSubTotal = false
    );
}
