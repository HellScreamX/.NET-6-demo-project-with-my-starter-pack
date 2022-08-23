#nullable disable
public class ContractorMetricsPut
{
    public string ContractorId { get; set; }
    public List<int> SkillIds { get; set; }
    public List<int> ModelIds { get; set; }
    public bool ModelsHasChanged { get; set; }
    public bool SkillsHasChanged { get; set; }
}