namespace GenbaseGoldReports.Application.Features.Masters.Accounts.Dtos
{
    public class AccountTreeItemDto
    {
        public string Code { get; set; } = string.Empty;
        public string Grpcode { get; set; } = string.Empty;
        public string Acname { get; set; } = string.Empty;
        public string Accode { get; set; } = string.Empty;
        public string Grpname { get; set; } = string.Empty;
        public bool Isacc { get; set; }
        public int Levelid { get; set; }
        public decimal Keycode { get; set; }
        public string Actype { get; set; } = string.Empty;
        public string ActypeDescription { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public string Curr { get; set; } = "AED";
        public string Trnno { get; set; } = string.Empty;
        public decimal Taxrate { get; set; }
    }
}
