namespace GenbaseGoldReports.Domain.Entities.Masters
{
    public class Karatmst
    {
        public string Code { get; set; } = null!;           
        public string? Description { get; set; }           
        public decimal Purity { get; set; }                
        public decimal Spurity { get; set; }               
        public decimal? Mfgpurity { get; set; }            
        public string? Sipcode { get; set; }
        public string? Location { get; set; }
        public string? Loccode { get; set; }
        public string? Usercode { get; set; }
    }
}
