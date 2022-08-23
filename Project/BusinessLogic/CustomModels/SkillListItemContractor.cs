using System.Text.Json.Serialization;
#nullable disable
public class SkillListItemContractor{
    public int Id { get; set; }
    public string Label { get; set; }
    public int FieldId { get; set; }
    public string FieldLabel { get; set; }
    public bool IsOwned { get; set; }
}