using System.Text.Json.Serialization;
#nullable disable
public class ModelListItem{
    public int Id { get; set; }
    public string Label { get; set; }
    public int BrandId { get; set; }
    public string BrandLabel { get; set; }
}