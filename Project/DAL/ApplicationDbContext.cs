using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Project.DAL
{
    public class ApplicationDbContext : IdentityDbContext<ProjectUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            /* builder.Entity<Interview>(entity =>
            {
                entity.HasIndex(e => e.EmployeeMail).IsUnique();
            }); */
            builder.Entity<ContractorSkill>()
            .HasIndex(p => new { p.SkillId, p.ContractorId }).IsUnique();
            builder.Entity<ContractorModel>()
            .HasIndex(p => new { p.ModelId, p.ContractorId }).IsUnique();
        }

        public DbSet<ExceptionLog> ExceptionLogs => Set<ExceptionLog>();
        public DbSet<Administrator> Administrators => Set<Administrator>();
        public DbSet<ImageValidationTemp> ImageValidationTemps => Set<ImageValidationTemp>();
        public DbSet<Field> Fields => Set<Field>();
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<Model> Models => Set<Model>();
        public DbSet<Skill> Skills => Set<Skill>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Commande> Commandes => Set<Commande>();
        public DbSet<CommandeModel> CommandeModels => Set<CommandeModel>();
        public DbSet<CommandeSkill> CommandeSkills => Set<CommandeSkill>();
        public DbSet<Contractor> Contractors => Set<Contractor>();
        public DbSet<ContractorSkill> ContractorSkills => Set<ContractorSkill>();
        public DbSet<ContractorModel> ContractorModels => Set<ContractorModel>();
        public DbSet<ContractorCommande> ContractorCommandes => Set<ContractorCommande>();
        public DbSet<ContractorCommandeDay> ContractorCommandeDays => Set<ContractorCommandeDay>();
        public DbSet<ContractorUnavailability> ContractorUnavailabilities => Set<ContractorUnavailability>();


    }
}