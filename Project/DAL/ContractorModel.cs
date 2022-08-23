#nullable disable
public class ContractorModel
{
    public int Id { get; set; }
    public string ContractorId { get; set; }
    public Contractor Contractor { get; set; }
    public int ModelId { get; set; }
    public Model Model { get; set; }
}