using System.Text.Json.Serialization;
#nullable disable
public class Model{
    public int Id { get; set; }
    public string Label { get; set; }
    public int BrandId { get; set; }
    [JsonIgnore]
    public Brand Brand { get; set; }
}