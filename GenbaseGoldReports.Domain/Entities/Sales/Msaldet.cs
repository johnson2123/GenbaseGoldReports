namespace GenbaseGoldReports.Domain.Entities.Sales
{
    public class Msaldet
    {
        public string Brncode { get; set; } = string.Empty;
        public string Divcode { get; set; } = string.Empty;
        public string Doctype { get; set; } = string.Empty;
        public int Docno { get; set; }
        public int Slno { get; set; }

        public int Slno2 { get; set; } = 1;
        public decimal Nfactor { get; set; } = -1;

        public string Mdoctype { get; set; } = "MTL";
        public string Sdoctype { get; set; } = string.Empty;
        public DateTime Docdate { get; set; }
        public string? Stimestamp { get; set; }
        public string? Loccode { get; set; }
        public string? Usercode { get; set; }
        public string? Partycode { get; set; }
        public string? Impexptype { get; set; }
        public string? Fixunfix { get; set; }
        public string? Sman { get; set; }

        public string Stkcode { get; set; } = string.Empty;
        public string? Stksubcode { get; set; }
        public int Pieces { get; set; }
        public int Pcs { get; set; }
        public decimal Grqty { get; set; }
        public decimal Apurity { get; set; }
        public decimal Spurity { get; set; }
        public decimal Puqty { get; set; }

        public decimal Mkgrate { get; set; }
        public decimal Mkgamt { get; set; }
        public decimal Mtlrate { get; set; }
        public decimal Mtlamt { get; set; }
        public decimal Mkgtaxamt { get; set; }
        public decimal Mtltaxamt { get; set; }
        public decimal Taxamt { get; set; }
        public decimal Totalamt { get; set; }

        public string Currcode { get; set; } = "AED";
        public decimal Currrate { get; set; } = 1.0m;
    }
}
