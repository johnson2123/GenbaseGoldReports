namespace GenbaseGoldReports.Domain.Entities.Sales
{
    public class Msal
    {
        public string Brncode { get; set; } = string.Empty;
        public string Divcode { get; set; } = string.Empty;
        public string Doctype { get; set; } = string.Empty;
        public int Docno { get; set; }

        public string Slno { get; set; } = string.Empty;
        public string Mdoctype { get; set; } = string.Empty;
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

        public string? Mtltype { get; set; }
        public string? Ratetype { get; set; }
        public decimal Gsrate { get; set; }
        public decimal Gozrate { get; set; }
        public decimal Usdrate { get; set; }
        public decimal Ozfactor { get; set; }
        public decimal Gramrate { get; set; }

        public string Curr { get; set; } = string.Empty;
        public string? Curr1 { get; set; }
        public string Currcode { get; set; } = string.Empty;
        public decimal Currrate { get; set; }
        public string? Lccurr { get; set; }
        public string? Basecurr { get; set; }

        public string Fixunfix { get; set; } = string.Empty;
        public string? Impexptype { get; set; }

        public decimal Totpcs { get; set; }
        public decimal Pieces { get; set; }
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

        public decimal Goldamt { get; set; }
        public decimal Makingvalue { get; set; }
        public decimal Totalvat { get; set; }
        public decimal Totalpartyamt { get; set; }
        public decimal Totnetamtfc { get; set; }
        public decimal Totnetamtbc { get; set; }
        public decimal Stnamt { get; set; }
        public decimal Totamtfc { get; set; }
        public decimal Amtpaid { get; set; }
        public decimal Totdedamtfc { get; set; }
        public decimal Totaddamtfc { get; set; }
        public decimal Refcharges { get; set; }

        public decimal Premiumperc { get; set; }
        public decimal Premiumamount { get; set; }
        public decimal Discountperc { get; set; }
        public decimal Discountamount { get; set; }

        public decimal Partyamount { get; set; }

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
