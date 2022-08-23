#nullable disable
using System.ComponentModel.DataAnnotations;

public class SkillSearchFilterContractor
{
    [Range(0, 1)]
    public int OrderBy { get; set; }

    [Range(0, 20)]
    public int Take { get; set; }
    public int Skip { get; set; }
    public int FieldId { get; set; }
    public string Keyword { get; set; }
    public bool IsOWned { get; set; }
}