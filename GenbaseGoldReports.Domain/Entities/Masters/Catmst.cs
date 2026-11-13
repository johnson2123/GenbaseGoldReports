namespace GenbaseGoldReports.Domain.Entities.Masters
{
    public class Catmst
    {
        public string Code { get; set; } = null!;           
        public string? Description { get; set; }           
        public string? Doctype { get; set; }
        public int? Docno { get; set; }
        public string? Slno { get; set; }
        public DateTime? Docdate { get; set; }
        public string? Mdoctype { get; set; }
        public string? Sdoctype { get; set; }
        public string? Loccode { get; set; }
        public string? Usercode { get; set; }
    }
}
