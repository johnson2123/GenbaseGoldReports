namespace GenbaseGoldReports.Domain.Entities.Masters
{
    public class Accdet
    {
        public string Accode { get; set; } = string.Empty;
        public string Curr { get; set; } = "AED";
        public string? Doctype { get; set; } = "ACC";
        public string? Docno { get; set; } = "0";
        public string? Docdoctype { get; set; } = "0";
        public string? Mdoctype { get; set; } = "ACC";
        public string? Sdoctype { get; set; } = "ACC";
        public string? Currname { get; set; } = "AED";
        public DateTime? Docdate { get; set; }
        public string? Levelcode { get; set; } = "0";
        public decimal? Uniqueid { get; set; }
        public string? Loccode { get; set; } = "0";
        public string? Usercode { get; set; } = "0";
    }
}
