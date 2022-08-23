#nullable disable
public class CommandePost
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime BeginDate { get; set; }
    public DateTime EndDate { get; set; }
    public int NbrContractors { get; set; }
    public List<int> Models { get; set; }
    public List<int> Skills { get; set; }
    public int CustomerId { get; set; }
}