#nullable disable
using System.ComponentModel.DataAnnotations;

public class CustomerSearchFilter
{
    [Range(0, 1)]
    public int OrderBy { get; set; }

    [Range(0, 20)]
    public int Take { get; set; }
    public int Skip { get; set; }
    public string Name { get; set; }
    public DateTime BeginDate { get; set; }
    public DateTime EndDate { get; set; }
}