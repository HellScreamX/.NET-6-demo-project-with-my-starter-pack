#nullable disable
public class CommandeDetails
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime BeginDate { get; set; }
    public DateTime EndDate { get; set; }
    public int NbrContractors { get; set; }
    public CommandeStatus Status { get; set; }
    public List<ModelListItem> Models { get; set; }
    public List<SkillListItem> Skills { get; set; }
    public String CustomerName { get; set; }
    public int CustomerId { get; set; }
}