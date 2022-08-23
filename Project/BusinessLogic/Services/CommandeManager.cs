using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.DAL;

public interface ICommandeManager
{
    Task<int> AddCommande(CommandePost commandePost);
    Task EditCommande(CommandePut commandePut);
    Task DeleteCommande(int id);
    Task<GenericListOutput<T>> GetCommandeList<T>(CommandeSearchFilter model);
    Task<GenericListOutput<T>> GetCommandeList<T>(DateTime day, string ownerId);
    Task<CommandeDetails> GetCommandeDetails(int id, string ownerId);
    Task<List<string>> MatchMetrics(int commandeId);
    Task AssociateContractor(CommandeContractorInput infos);
    Task<List<string>> GetAssociatedContractors(int commandeId);
    Task DesassociatedContractor(DesassociateContractor infos);
    Task<List<CommandeCountDaily>> DailyMissionsCount(DateTime begin, DateTime end);
}

public class CommandeManager : ICommandeManager
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IContractorManager _contractorManager;
    public CommandeManager(ApplicationDbContext context, IMapper mapper, IContractorManager contractorManager)
    {
        _context = context;
        _mapper = mapper;
        _contractorManager = contractorManager;
    }
    public async Task<int> AddCommande(CommandePost commandePost)
    {
        if (string.IsNullOrWhiteSpace(commandePost.Name)) throw new CustomException("empty name");
        Commande commande = new Commande();
        _mapper.Map<CommandePost, Commande>(commandePost, commande);


        //Adding the list of skills to the new created Commande
        commande.CommandeSkills = new List<CommandeSkill>();
        foreach (var item in commandePost.Skills)
        {
            commande.CommandeSkills.Add(new CommandeSkill
            {
                CommandeId = commande.Id,
                SkillId = item
            });
        }

        //Adding the list of Models to the new created Commande
        commande.CommandeModels = new List<CommandeModel>();
        foreach (var item in commandePost.Models)
        {
            commande.CommandeModels.Add(new CommandeModel
            {
                CommandeId = commande.Id,
                ModelId = item
            });
        }
        commande.Status = CommandeStatus.New;
        await _context.Commandes.AddAsync(commande);
        await _context.SaveChangesAsync();
        return commande.Id;
    }

    public async Task AssociateContractor(CommandeContractorInput infos)
    {
        var commande = await _context.Commandes.FindAsync(infos.CommandeId) ?? throw new CustomException("Commande not found");
        var contractor = await _context.Contractors.Where(x => x.Email == infos.ContractorEmail).FirstOrDefaultAsync() ?? throw new CustomException("Contractor not found");
        if (!await _contractorManager.IsFree(contractor.Id, new DateRange { Begin = commande.BeginDate, End = commande.EndDate }))
            throw new CustomException("Contractor is not free");

        var contractorCommande = new ContractorCommande
        {
            CommandeId = infos.CommandeId,
            ContractorId = contractor.Id
        };
        await _context.ContractorCommandes.AddAsync(contractorCommande);
        await _context.SaveChangesAsync();

        for (DateTime i = commande.BeginDate; i <= commande.EndDate; i = i.AddDays(1))
        {
            await _context.ContractorCommandeDays.AddAsync(new ContractorCommandeDay
            {
                ContractorCommandeId = contractorCommande.Id,
                ContractorId = contractor.Id,
                Day = i
            });
        }
        await _context.SaveChangesAsync();
    }

    public async Task<List<CommandeCountDaily>> DailyMissionsCount(DateTime begin, DateTime end)
    {
        List<CommandeCountDaily> missionsCount = new List<CommandeCountDaily>();
        var missions = await _context.Commandes.Where(x => !(x.EndDate < begin || x.BeginDate > end)).AsNoTracking().ToListAsync();
        for (DateTime i = begin; i <= end; i = i.AddDays(1))
        {
            var currentDay = new CommandeCountDaily();
            currentDay.Day = i;
            foreach (var item in missions)
            {
                if (item.BeginDate <= i && item.EndDate >= i)
                    currentDay.Count++;
            }
            if (currentDay.Count > 0) missionsCount.Add(currentDay);
        }
        return missionsCount;
    }

    public async Task DeleteCommande(int id)
    {
        var Commande = await _context.Commandes.FindAsync(id) ?? throw new CustomException("commande not found");
        ;
        _context.Commandes.Remove(Commande);
        await _context.SaveChangesAsync();
    }

    public async Task DesassociatedContractor(DesassociateContractor infos)
    {
        var contractorCommande = await _context.ContractorCommandes
        .Where(x => x.CommandeId == infos.CommandeId && x.Contractor.Email == infos.ContractorEmail).FirstOrDefaultAsync();

        if (contractorCommande != null)
        {
            _context.ContractorCommandes.Remove(contractorCommande);
        }
        await _context.SaveChangesAsync();
    }

    public async Task EditCommande(CommandePut commandePut)
    {
        if (string.IsNullOrWhiteSpace(commandePut.Name)) throw new CustomException("empty name");

        var commande = await _context.Commandes.Where(x => x.Id == commandePut.Id)
        .Include(x => x.CommandeSkills)
        .Include(x => x.CommandeModels)
        .FirstOrDefaultAsync() ?? throw new CustomException("Commande not found");

        _mapper.Map<CommandePut, Commande>(commandePut, commande);

        if (commandePut.SkillsHasChanged)
        {
            //removing old skills
            _context.CommandeSkills.RemoveRange(commande.CommandeSkills);
            //Adding the list of skills to the new created Commande
            foreach (var item in commandePut.Skills)
            {
                commande.CommandeSkills.Add(new CommandeSkill
                {
                    CommandeId = commande.Id,
                    SkillId = item
                });
            }

        }

        if (commandePut.ModelsHasChanged)
        {
            //removing old brands
            _context.CommandeModels.RemoveRange(commande.CommandeModels);
            //Adding the list of Models to the new created Commande
            foreach (var item in commandePut.Models)
            {
                commande.CommandeModels.Add(new CommandeModel
                {
                    CommandeId = commande.Id,
                    ModelId = item
                });
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<string>> GetAssociatedContractors(int commandeId)
    {
        var contractors = await _context.ContractorCommandes.Where(x => x.CommandeId == commandeId).Select(x => x.Contractor.Email).AsNoTracking().ToListAsync();
        return contractors;
    }

    public async Task<CommandeDetails> GetCommandeDetails(int id, string ownerId)
    {
        var Commande = await _context.Commandes.AsNoTracking().Where(x => x.Id == id)
        .Include(x => x.CommandeSkills).ThenInclude(x => x.Skill).ThenInclude(x => x.Field)
        .Include(x => x.CommandeModels).ThenInclude(x => x.Model).ThenInclude(x => x.Brand)
        .Where(x => String.IsNullOrWhiteSpace(ownerId) || x.ContractorCommandes.Where(x => x.ContractorId == ownerId).Any())
        .Include(x => x.Customer)
        .FirstOrDefaultAsync() ?? throw new CustomException("Commande not found");
        return _mapper.Map<CommandeDetails>(Commande);
    }

    public async Task<GenericListOutput<T>> GetCommandeList<T>(CommandeSearchFilter filter)
    {

        //Check if the contractor exist and then get his Id 
        var contractorId = "";
        if (!String.IsNullOrWhiteSpace(filter.ContractorEmail))
        {
            var contractor = await _context.Contractors.Where(x => x.Email == filter.ContractorEmail)
               .FirstOrDefaultAsync() ?? throw new CustomException("Contractor not found");
            contractorId = contractor.Id;
        }

        if (filter.Take <= 0 || filter.Take >= 50) filter.Take = 50;
        var query = _context.Commandes.AsNoTracking()
        .Where(x => String.IsNullOrWhiteSpace(filter.Name) || x.Name.Contains(filter.Name))
        .Where(c => filter.Date == DateTime.MinValue || (c.BeginDate >= filter.Date && c.EndDate >= filter.Date))
        .Where(x => String.IsNullOrWhiteSpace(contractorId) || x.ContractorCommandes.Where(x => x.ContractorId == contractorId).Any());

        switch (filter.OrderBy)
        {
            case 1: query = query.OrderBy(x => x.Id); break;
            default: query = query.OrderByDescending(x => x.Id); break;
        }
        var result = await query.Include(x => x.Customer)
        .Skip(filter.Skip).Take(filter.Take).Select(x => _mapper.Map<Commande, T>(x)).ToListAsync();

        return new GenericListOutput<T>
        {
            ResultList = result,
            TotalCount = await query.CountAsync()
        };
    }

    public async Task<GenericListOutput<T>> GetCommandeList<T>(DateTime day, string ownerId)
    {

        var query = _context.Commandes.AsNoTracking()
        .Where(c => day == DateTime.MinValue || (c.BeginDate <= day && c.EndDate >= day))
        .Where(x => ownerId == "" || x.ContractorCommandes.Where(x => x.ContractorId == ownerId).Any());

        query = query.OrderByDescending(x => x.Id);

        var result = await query.Include(x => x.Customer)
        .Select(x => _mapper.Map<Commande, T>(x)).ToListAsync();

        return new GenericListOutput<T>
        {
            ResultList = result,
            TotalCount = await query.CountAsync()
        };
    }

    public async Task<List<string>> MatchMetrics(int commandeId)
    {
        //get the commande to match with all skills and models required
        var commandeToMatch = await _context.Commandes
        .AsNoTracking().Where(x => x.Id == commandeId)
        .Include(x => x.CommandeSkills).ThenInclude(x => x.Skill)
        .Include(x => x.CommandeModels).ThenInclude(x => x.Model)
        .FirstOrDefaultAsync() ?? throw new CustomException("Commande not found");

        //get all the contractors
        var contractors = await _context.Contractors.AsNoTracking().ToListAsync();

        //instanciate the Skills and the Models from the command that we have to match
        List<int> skillsToMatch = commandeToMatch.CommandeSkills.Select(x => x.SkillId).ToList();
        List<int> modelsToMatch = commandeToMatch.CommandeModels.Select(x => x.ModelId).ToList();

        //prepare an empty list for the futur matched candidates
        List<string> matchedContractors = new List<string>();

        //check every contractor if he got the right skills and models matched to the commande
        foreach (var item in contractors)
        {
            var matchedSkillsCount = await _context.ContractorSkills.AsNoTracking()
            .Where(x => x.ContractorId == item.Id && skillsToMatch.Contains(x.SkillId)).CountAsync();

            var matchedModelsCount = await _context.ContractorModels.AsNoTracking()
            .Where(x => x.ContractorId == item.Id && modelsToMatch.Contains(x.ModelId)).CountAsync();

            var isFree = await _contractorManager.IsFree(item.Id, new DateRange { Begin = commandeToMatch.BeginDate, End = commandeToMatch.EndDate });

            if (matchedModelsCount == modelsToMatch.Count & matchedSkillsCount == skillsToMatch.Count && isFree) matchedContractors.Add(item.Email);
        }
        return matchedContractors;
    }
}