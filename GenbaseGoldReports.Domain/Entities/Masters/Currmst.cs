namespace GenbaseGoldReports.Domain.Entities.Masters
{
    public class Currmst
    {
        public string Currcode { get; set; } = null!;
        public string? Currname { get; set; }
        public decimal? Currrate { get; set; }
        public string? Doctype { get; set; }
        public int? Docno { get; set; }
        public string? Slno { get; set; }
        public DateTime? Docdate { get; set; }
        public string? Curr { get; set; }
        public string? Mdoctype { get; set; }
        public string? Sdoctype { get; set; }
        public string? Subname { get; set; }
        public string? Brncode { get; set; }
        public string? Loccode { get; set; }
        public string? Usercode { get; set; }
    }
}
