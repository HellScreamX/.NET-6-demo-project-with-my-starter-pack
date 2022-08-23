using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.DAL;

public interface IContractorManager
{
    Task AddUnavailableDays(string contractorId, List<DateTime> days);
    Task RemoveUnavailableDays(string contractorId, List<DateTime> days);
    Task<List<DateTime>> GetUnavailableDays(string contractorEmail, DateRange range);
    Task<List<CommandeContractorDaysOutput>> GetCommandeDays(string contractorEmail, DateRange range);
    Task<List<ContractorCountDaily>> GetCountDaily(DateTime begin, DateTime end);
    Task<List<String>> WorkingByDay(DateTime day);
    Task<bool> IsFree(string contractorId, DateRange range);

}
public class ContractorManager : IContractorManager
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public ContractorManager(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task AddUnavailableDays(string contractorId, List<DateTime> days)
    {
        //check if is there missions days in the unavailability days that we want to add
        var hasGotCommande = await _context.ContractorCommandeDays        
        .Where(x=>days.Contains(x.Day) && x.ContractorId == contractorId).AnyAsync();
        if(hasGotCommande) throw new CustomException("days range conflicting with missions");

        var currentUnavDays = await _context.ContractorUnavailabilities.Where(x => days.Contains(x.Day)&& x.ContractorId==contractorId)
        .AsNoTracking().Select(x => x.Day).ToListAsync();

        

        foreach (var item in days)
        {
            if (!currentUnavDays.Contains(item))
            {
                await _context.AddAsync(new ContractorUnavailability
                {
                    ContractorId = contractorId,
                    Day = item
                });
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task<List<CommandeContractorDaysOutput>> GetCommandeDays(string contractorEmail, DateRange range)
    {
        var contractor = await _context.Contractors.Where(x => x.Email == contractorEmail).FirstOrDefaultAsync() ?? throw new CustomException("Contractor not found");

        var currentCommandeDays = await _context.ContractorCommandeDays
        .Where(x => x.ContractorId==contractor.Id && x.Day>=range.Begin && x.Day <=range.End)
        .Select(x =>new CommandeContractorDaysOutput{Day = x.Day, CommandeId = x.ContractorCommande.CommandeId}).ToListAsync();

        return currentCommandeDays;
    }

    public async Task<List<ContractorCountDaily>> GetCountDaily(DateTime begin, DateTime end)
    {
        var contractorsDailyCount = await _context.ContractorCommandeDays
        .Where(x=>x.Day>=begin && x.Day<=end)
        .GroupBy(x=>x.Day.Date)
        .Select(x=>new ContractorCountDaily{Count = x.Count(), Day=x.Key}).AsNoTracking().ToListAsync();;

        return contractorsDailyCount;
    }

    public async Task<List<DateTime>> GetUnavailableDays(string contractorEmail, DateRange range)
    {
        var contractor = await _context.Contractors.Where(x => x.Email == contractorEmail).FirstOrDefaultAsync() ?? throw new CustomException("Contractor not found");
        var currentUnavDays = await _context.ContractorUnavailabilities
        .Where(x => x.ContractorId==contractor.Id && x.Day>=range.Begin && x.Day <=range.End).Select(x => x.Day).ToListAsync();
        
        return currentUnavDays;
    }

    public async Task<bool> IsFree(string contractorId, DateRange range)
    {
        //get count of unavailabilities in this time range
        var unavailabilitiesCount = await _context.ContractorUnavailabilities
        .Where(x=>x.Day>=range.Begin && x.Day<=range.End && x.ContractorId == contractorId).CountAsync();

        //get count of commande days in this time range
        var commandeDaysCount = await _context.ContractorCommandeDays
        .Where(x=>x.Day>=range.Begin && x.Day<=range.End && x.ContractorId == contractorId).CountAsync();

        //no busy days and no commande days means that he is free in this range of time
        if(unavailabilitiesCount == 0 && commandeDaysCount == 0) return true;
        else return false;
    }

    public async Task RemoveUnavailableDays(string contractorId, List<DateTime> days)
    {
        var currentUnavDays = await _context.ContractorUnavailabilities
        .Where(x => days.Contains(x.Day) && x.ContractorId==contractorId)
        .ToListAsync();

        _context.RemoveRange(currentUnavDays);
        await _context.SaveChangesAsync();
    }

    public async Task<List<string>> WorkingByDay(DateTime day)
    {
        var contractors = await _context.ContractorCommandes.Where(x=>x.Commande.BeginDate<=day && x.Commande.EndDate>=day)
        .Select(x=>x.Contractor.Email).ToListAsync();
        return contractors;
    }
}