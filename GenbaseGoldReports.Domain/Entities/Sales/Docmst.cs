namespace GenbaseGoldReports.Domain.Entities.Sales
{
    public class Docmst
    {
        public string Brncode { get; set; } = string.Empty;
        public string Divcode { get; set; } = string.Empty;
        public string Doctype { get; set; } = string.Empty;
        public string? Lastdocno { get; set; }
        public string? Docprefix { get; set; }
        public string? Mdoctype { get; set; }
        public string? Sdoctype { get; set; }
    }
}
