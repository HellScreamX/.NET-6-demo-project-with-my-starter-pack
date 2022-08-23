using System.ComponentModel.DataAnnotations;
#nullable disable
public class CommandeSearchFilter
{
    [Range(0, 1)]
    public int OrderBy { get; set; }

    [Range(0, 20)]
    public int Take { get; set; }
    public int Skip { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public int NbrContractors { get; set; }
    public CommandeStatus Status { get; set; }
    public String CustomerName { get; set; }
    public string ContractorEmail { get; set; }
}