namespace GenbaseGoldReports.Domain.Entities.Masters
{
    public class Lccmst
    {
        public string Code { get; set; } = null!;           
        public string? Description { get; set; }           
        public string? Doctype { get; set; }
        public int? Docno { get; set; }
        public string? Slno { get; set; }
        public string? Lcctype { get; set; }               
        public DateTime? Docdate { get; set; }
        public string? Loccode { get; set; }
        public string? Usercode { get; set; }
    }
}
