using System.Text.Json.Serialization;
#nullable disable
public class ModelListItemContractor{
    public int Id { get; set; }
    public string Label { get; set; }
    public int BrandId { get; set; }
    public string BrandLabel { get; set; }
    public bool IsOwned { get; set; }
}