using System.Text.Json.Serialization;
#nullable disable
public class Brand{
    public int Id { get; set; }
    public string Label { get; set; }
    [JsonIgnore]
    public List<Model> Models {get; set;}
}