using System.Text.Json.Serialization;
#nullable disable
public class Skill{
    public int Id { get; set; }
    public string Label { get; set; }
    public int FieldId { get; set; }
    [JsonIgnore]
    public Field Field { get; set; }
}