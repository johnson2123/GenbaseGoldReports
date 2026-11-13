namespace GenbaseGoldReports.Domain.Entities.Masters
{
    public class Metalmaster
    {
        public string? Doctype { get; set; } = "MSTK";           
        public int? Docno { get; set; }                          
        public string? Code { get; set; }                        
        public string? Description { get; set; }                 
        public string? Slno { get; set; }                        
        public string Stkcode { get; set; } = null!;             
        public string? Lcccode { get; set; }                     
        public string? Uom { get; set; }                         
        public string? Stkname { get; set; }                     
        public string? Stkdetails { get; set; }                  

        // Legacy smallint flags (0 = false, 1 = true)
        public short? Isbarcodestk { get; set; } = 0;            
        public short? Ispcs { get; set; } = 1;                   
        public short? Isweight { get; set; } = 1;                
        public short? Issingle { get; set; } = 1;                
        public short? Isshowweight { get; set; } = 0;            
        public short? Isstone { get; set; } = 0;                 
        public short? Autopl { get; set; } = 1;                  
        public short? Isscrap { get; set; } = 0;                 
        public short? Isbarcode { get; set; } = 0;               
        public short? Isratebypcs { get; set; } = 0;             
        public short? Allowminus { get; set; } = 0;              

        public string? Stksubcode { get; set; }                  
        public decimal? Pcweight { get; set; } = 0.00m;          
        public string? Suom { get; set; }                        
        public string? Mainuom { get; set; }                     
        public string? Subuom { get; set; }                      
        public string? Tranuom { get; set; }                     

        public decimal? Price1 { get; set; } = 0.00m;            
        public decimal? Price2 { get; set; } = 0.00m;            
        public decimal? Mainuqty { get; set; } = 0.00m;          
        public decimal? Subuqty { get; set; } = 0.00m;           
        public decimal? Tranuqty { get; set; } = 0.00m;          
        public int? Uniqueid { get; set; }                       
        public DateTime? Docdate { get; set; }                   
        public decimal? Cost { get; set; } = 0.00m;              

        public string? Curr { get; set; } = "AED";               
        public decimal? Currrate { get; set; } = 1.00000m;       
        public string? Costunit { get; set; }                    
        public string? Price1unit { get; set; }                 
        public decimal? Price1per { get; set; } = 0.00m;         
        public decimal? Price2per { get; set; } = 0.00m;         
        public decimal? Price3per { get; set; } = 0.00m;         
        public decimal? Price4per { get; set; } = 0.00m;         
        public decimal? Price1fc { get; set; } = 0.00m;          
        public decimal? Price2fc { get; set; } = 0.00m;          
        public decimal? Price3 { get; set; } = 0.00m;            
        public decimal? Price3fc { get; set; } = 0.00m;          
        public decimal? Price4 { get; set; } = 0.00m;            
        public decimal? Price4fc { get; set; } = 0.00m;          
        public decimal? Costfc { get; set; } = 0.00m;            
        public string? Priceunit { get; set; }                   

        public string? Sdoctype { get; set; }                    
        public string? Mdoctype { get; set; }                    
        public string? Mkglcccode { get; set; }                  
        public string? Karat { get; set; }                       
        public decimal? Mkgrate { get; set; } = 0.00m;           
        public decimal? Spurity { get; set; } = 0.00000m;        
        public string? Cat { get; set; }                         
        public string? Subcat { get; set; }                      
        public string? Mtltype { get; set; }                     
        public string? Piclocation { get; set; }                 
        public string? Type { get; set; }                        
        public string? Tray { get; set; }                        
        public string? Brand { get; set; }                       
        public string? Ctry { get; set; }                        
        public string? Price2unit { get; set; }                  
        public string? Price3unit { get; set; }                  

        public decimal? Taxrate { get; set; } = 5.00m;           
        public decimal? Apurity { get; set; } = 0.00m;           
        public decimal? Salpurity { get; set; } = 0.00000m;      
        public decimal? Mtltaxrate { get; set; } = 0.00m;        
        public decimal? Mkgtaxrate { get; set; } = 5.00m;        
        public string? Loccode { get; set; } = "0";              
        public string? Usercode { get; set; } = "0";             
    }
}
