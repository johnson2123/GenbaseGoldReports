namespace GenbaseGoldReports.Domain.Entities.Masters
{
    public class Accmst
    {
        public string Code { get; set; } = string.Empty;
        public string? Grpcode { get; set; }
        public string? Acname { get; set; }
        public string? Accode { get; set; }
        public string? Grpname { get; set; }
        public short? Isacc { get; set; }
        public int? Levelid { get; set; }
        public decimal? Keycode { get; set; }
        public string? Actype { get; set; }
        public string? Class { get; set; }
        public string? Brncode { get; set; } = "HO";
        public string? Curr { get; set; } = "AED";
        public decimal? Currrate { get; set; } = 1.0m;
        public string? Trnno { get; set; }
        public decimal? Taxrate { get; set; }
        public short? Isvatgroup { get; set; }
        public string? Addr1 { get; set; }
        public string? Addr2 { get; set; }
        public string? Addr3 { get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string? Contactperson { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Area { get; set; }
        public string? Licencenumber { get; set; }
        public string? Licencedate { get; set; }
        public short? Selected { get; set; }
        public DateTime? Docdate { get; set; }
        public decimal? Purusdrate { get; set; }
        public decimal? Salusdrate { get; set; }
        public decimal? Marginperc { get; set; }
        public decimal? Marginconsumeperc { get; set; }
        public string? Loccode { get; set; }
        public string? Usercode { get; set; }

        // --- Hardened Legacy Sequence & Audit Fields ---
        public int? Docno { get; set; } = 0;
        public string? Mrtype { get; set; } = "0";
        public int? Srno { get; set; } = 0;
        public string? Mstsrno { get; set; } = "0";
        public string? Doctype { get; set; } = "ACC";
        public string? Slno { get; set; } = "000000";
        public string? Parcode { get; set; } = "0";
        public int? Maingroup { get; set; } = 1;
        public string? Mdoctype { get; set; } = "ACC";
        public string? Sdoctype { get; set; } = "ACC";
        public string? Bnkcode { get; set; } = "0";
        public string? Bnkactype { get; set; } = "0";
        public string? Bnkacno { get; set; } = "0";
        public string? Mobileno { get; set; } = "0";
        public short? Isconfacc { get; set; } = 0;
        public string? Cvrno { get; set; } = "0";
        public string? Isspecialacc { get; set; } = "0";
        public string? Custcat { get; set; } = "0";
        public string? Custtype { get; set; } = "0";
        public short? Isalloc { get; set; } = 0;
        public decimal? Purgozrate { get; set; } = 0m;
        public decimal? Salgozrate { get; set; } = 0m;
        public decimal? Purconvrate { get; set; } = 0m;
        public decimal? Salconvrate { get; set; } = 0m;
        public string? Purtxntype { get; set; } = "0";
        public string? Saltxntype { get; set; } = "0";
        public string? Custarea { get; set; } = "0";
        public decimal? Sharepercentage { get; set; } = 0m;
    }
}
