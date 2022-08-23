#nullable disable
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreationDate { get; set; }  
    public List<Commande> Commandes { get; set; }
}