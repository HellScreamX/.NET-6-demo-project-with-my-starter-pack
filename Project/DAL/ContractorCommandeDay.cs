using System;
#nullable disable

public class ContractorCommandeDay
{
    public int Id { get; set; }
    public int ContractorCommandeId { get; set; }
    public ContractorCommande ContractorCommande { get; set; }
    public DateTime Day { get; set; }
    public string ContractorId { get; set; }
}