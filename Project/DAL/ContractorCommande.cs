#nullable disable
public class ContractorCommande
{
    public int Id { get; set; }
    public int CommandeId { get; set; }
    public Commande Commande { get; set; }
    public string ContractorId { get; set; }
    public Contractor Contractor { get; set; }
}