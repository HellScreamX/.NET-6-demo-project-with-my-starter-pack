#nullable disable
public class CommandeModel
{
    public int Id { get; set; }
    public int CommandeId { get; set; }
    public Commande Commande { get; set; }
    public int ModelId { get; set; }
    public Model Model { get; set; }
}