#nullable disable
public class Commande
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime BeginDate { get; set; }
    public DateTime EndDate { get; set; }
    public int NbrContractors { get; set; }
    public List<CommandeModel> CommandeModels { get; set; }
    public List<CommandeSkill> CommandeSkills { get; set; }
    public List<ContractorCommande> ContractorCommandes { get; set; }
    public CommandeStatus Status { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
}
public enum CommandeStatus
{
    New = 1,
    InProgress = 2, 
    Finished = 3, 
    Canceled = 4
}