#nullable disable
public class ContractorSkill
{
    public int Id { get; set; }
    public string ContractorId { get; set; }
    public Contractor Contractor { get; set; }
    public int SkillId { get; set; }
    public Skill Skill { get; set; }
}