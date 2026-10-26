namespace GenbaseGoldReports.Domain.Entities.Sales
{
    public class Msal
    {

        public string Brncode { get; set; } = string.Empty;
        public string Divcode { get; set; } = string.Empty;
        public string Doctype { get; set; } = string.Empty;
        public int Docno { get; set; }


        public int Slno { get; set; }
        public string Mdoctype { get; set; } = "MTL";
        public string Sdoctype { get; set; } = string.Empty;
        public DateTime Docdate { get; set; }
        public string? Stimestamp { get; set; }
        public string? Loccode { get; set; }
        public string? Usercode { get; set; }


        public string Partycode { get; set; } = string.Empty;
        public string? Partyname { get; set; }
        public string? Partytrn { get; set; }
        public string? Addr1 { get; set; }
        public string? Sman { get; set; }


        public string? Ratetype { get; set; }
        public decimal Gozrate { get; set; }
        public decimal Usdrate { get; set; } = 3.67400m;
        public decimal Ozfactor { get; set; } = 31.10350m;
        public decimal Gramrate { get; set; }

        public string Curr { get; set; } = "AED";
        public string Currcode { get; set; } = "AED";
        public decimal Currrate { get; set; } = 1.0m;
        public string Lccurr { get; set; } = "AED";


        public string Fixunfix { get; set; } = "UNFIX";
        public string? Impexptype { get; set; } = "B2BSAL";


        public int Totpcs { get; set; }
        public int Pieces { get; set; }
        public decimal Totgrqty { get; set; }
        public decimal Totpuqty { get; set; }
        public decimal Totmkgamt { get; set; }
        public decimal Totalmakingamount { get; set; }
        public decimal Totalmtlamt { get; set; }
        public decimal Totalmetalamount { get; set; }
        public decimal Tottaxamt { get; set; }
        public decimal Totalvatamount { get; set; }
        public decimal Totgrossamt { get; set; }
        public decimal Gtotalamt { get; set; }
        public decimal Totalamount { get; set; }


        public decimal Premiumperc { get; set; }
        public decimal Premiumamount { get; set; }
        public decimal Discountperc { get; set; }
        public decimal Discountamount { get; set; }


        public decimal Partyamount { get; set; }
        public decimal Totnetamtbc { get; set; }


        public int Ishedging { get; set; }
        public int Autohedgingdocno { get; set; }
        public int Mpydocno { get; set; }
        public string? Refdocno { get; set; }
        public DateTime? Refdocdate { get; set; }
        public string? Narration { get; set; }
        public string? Hnarration { get; set; }


        public ICollection<Msaldet> Details { get; set; } = new List<Msaldet>();
    }
}
