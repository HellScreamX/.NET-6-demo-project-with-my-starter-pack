#nullable disable
public class CommandeSkill
{
    public int Id { get; set; }
    public int CommandeId { get; set; }
    public Commande Commande { get; set; }
    public int SkillId { get; set; }
    public Skill Skill { get; set; }
}