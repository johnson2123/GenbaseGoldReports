namespace GenbaseGoldReports.Application.Common.Interfaces
{
    public interface IReportExportService<in TReportDto>
    {
        byte[] GenerateExcel(TReportDto data, string tenantName);
        byte[] GeneratePdf(TReportDto data, string tenantName);
        byte[] GenerateWord(TReportDto data, string tenantName);
    }
}
