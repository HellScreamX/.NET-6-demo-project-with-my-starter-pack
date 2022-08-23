#nullable disable
public class CommandePut
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime BeginDate { get; set; }
    public DateTime EndDate { get; set; }
    public int NbrContractors { get; set; }
    public List<int> Models { get; set; }
    public List<int> Skills { get; set; }
    public bool ModelsHasChanged { get; set; }
    public bool SkillsHasChanged { get; set; }
    public CommandeStatus Status { get; set; }
}