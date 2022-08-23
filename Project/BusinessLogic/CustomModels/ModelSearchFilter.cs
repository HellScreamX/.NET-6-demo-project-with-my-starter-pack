#nullable disable
using System.ComponentModel.DataAnnotations;

public class ModelSearchFilter
{
    [Range(0, 1)]
    public int OrderBy { get; set; }

    [Range(0, 20)]
    public int Take { get; set; }
    public int Skip { get; set; }
    public int BrandId { get; set; }
    public string Keyword { get; set; }
}