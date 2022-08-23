using System.Text.Json.Serialization;
#nullable disable
public class Field{
    public int Id { get; set; }
    public string Label { get; set; }
    [JsonIgnore]
    public List<Skill> Skills {get; set;}
}