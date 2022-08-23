using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.DAL;

public interface IContractorMetricsManager
{
    Task EditContractorSkills(ContractorSkillsPut skills, string contractorId);
    Task EditContractorModels(ContractorModelsPut models, string contractorId);
    Task<Object> GetFieldList(string contractorId);
    Task<GenericListOutput<SkillListItemContractor>> GetSkillList(SkillSearchFilterContractor filter, string contractorId);
    Task<List<Brand>> GetBrandList(string contractorId);
    Task<GenericListOutput<ModelListItemContractor>> GetModelList(ModelSearchFilterContractor filter, string contractorId);
}

public class ContractorMetricsManager : IContractorMetricsManager
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public ContractorMetricsManager(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task EditContractorModels(ContractorModelsPut models, string contractorId)
    {
        foreach (var item in models.ModelsToAdd)
        {
            await _context.ContractorModels.AddAsync(new ContractorModel
            {
                ContractorId = contractorId,
                ModelId = item
            });
        }
        var ModelsToDelete = _context.ContractorModels.Where(x => models.ModelsToDelete
        .Contains(x.ModelId) && x.ContractorId == contractorId);
        _context.RemoveRange(ModelsToDelete);
        await _context.SaveChangesAsync();
    }

    public async Task EditContractorSkills(ContractorSkillsPut skills, string contractorId)
    {
        foreach (var item in skills.SkillsToAdd)
        {
            await _context.ContractorSkills.AddAsync(new ContractorSkill
            {
                ContractorId = contractorId,
                SkillId = item
            });
        }
        var SkillsToDelete = await _context.ContractorSkills.Where(x => skills.SkillsToDelete
        .Contains(x.SkillId) && x.ContractorId == contractorId).ToListAsync();
        _context.ContractorSkills.RemoveRange(SkillsToDelete);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Brand>> GetBrandList(string contractorId)
    {
        var list = await _context.Brands.AsNoTracking().ToListAsync();
        return list;
    }

    public async Task<Object> GetFieldList(string contractorId)
    {
        var list = await _context.Skills.AsNoTracking().GroupBy(x => new
        {
            label = x.Field.Label,
            fieldId = x.FieldId
        }).Select(x => x.Key).ToListAsync();
        return list;
    }

    public async Task<GenericListOutput<ModelListItemContractor>> GetModelList(ModelSearchFilterContractor filter, string contractorId)
    {
        if (filter.IsOWned)
        {
            if (filter.Take <= 0 || filter.Take >= 50) filter.Take = 50;
            var query = _context.ContractorModels.AsNoTracking()
            .Where(x => x.ContractorId == contractorId)
            .Where(x => filter.BrandId == 0 || x.Model.BrandId == filter.BrandId)
            .Where(x => string.IsNullOrWhiteSpace(filter.Keyword) || x.Model.Label.Contains(filter.Keyword));

            switch (filter.OrderBy)
            {
                case 1: query = query.OrderBy(x => x.Id); break;
                default: query = query.OrderByDescending(x => x.Id); break;
            }

            var result = await query.Include(x => x.Model).ThenInclude(x => x.Brand)
            .Skip(filter.Skip).Take(filter.Take).Select(x => _mapper.Map<ModelListItemContractor>(x.Model)).ToListAsync();

            //changing the bool IsOwned of each row to true
            foreach (var item in result)
            {
                item.IsOwned=true;
            }
            return new GenericListOutput<ModelListItemContractor>
            {
                ResultList = result,
                TotalCount = await query.CountAsync()
            };
        }
        else{
            var query = _context.Models.AsNoTracking()
            .Where(x => filter.BrandId == 0 || x.BrandId == filter.BrandId)
            .Where(x => string.IsNullOrWhiteSpace(filter.Keyword) || x.Label.Contains(filter.Keyword));

            switch (filter.OrderBy)
            {
                case 1: query = query.OrderBy(x => x.Id); break;
                default: query = query.OrderByDescending(x => x.Id); break;
            }
            var allmodels = await query.Include(x => x.Brand)
            .Skip(filter.Skip).Take(filter.Take).ToListAsync();

            //Getting the models that he own from the allmodels that we researched
            var ownedmodels = await _context.ContractorModels.AsNoTracking()
            .Where(x => allmodels.Select(x => x.Id).Contains(x.ModelId) && x.ContractorId == contractorId)
            .Select(x => x.ModelId).ToListAsync();

            //Creating the final list of the allmodels with the IsOwned boolean
            var result = new List<ModelListItemContractor>();
            foreach (var item in allmodels)
            {
                ModelListItemContractor modelToAdd = _mapper.Map<ModelListItemContractor>(item);

                if (ownedmodels.Contains(item.Id))
                {
                    modelToAdd.IsOwned = true;
                }
                else
                {
                    modelToAdd.IsOwned = false;
                }
                result.Add(modelToAdd);
            }
            return new GenericListOutput<ModelListItemContractor>
            {
                ResultList = result,
                TotalCount = await query.CountAsync()
            };
        }
    }

    public async Task<GenericListOutput<SkillListItemContractor>> GetSkillList(SkillSearchFilterContractor filter, string contractorId)
    {
        if (filter.Take <= 0 || filter.Take >= 50) filter.Take = 50;
        if (filter.IsOWned)
        {
            var query = _context.ContractorSkills.AsNoTracking()
            .Where(x => x.ContractorId == contractorId)
            .Where(x => filter.FieldId == 0 || x.Skill.FieldId == filter.FieldId)
            .Where(x => string.IsNullOrWhiteSpace(filter.Keyword) || x.Skill.Label.Contains(filter.Keyword));

            switch (filter.OrderBy)
            {
                case 1: query = query.OrderBy(x => x.Id); break;
                default: query = query.OrderByDescending(x => x.Id); break;
            }
            var result = await query.Include(x => x.Skill).ThenInclude(x => x.Field)
            .Skip(filter.Skip).Take(filter.Take).Select(x => _mapper.Map<SkillListItemContractor>(x)).ToListAsync();

            return new GenericListOutput<SkillListItemContractor>
            {
                ResultList = result,
                TotalCount = await query.CountAsync()
            };
        }
        else
        {
            var query = _context.Skills.AsNoTracking()
            .Where(x => filter.FieldId == 0 || x.FieldId == filter.FieldId)
            .Where(x => string.IsNullOrWhiteSpace(filter.Keyword) || x.Label.Contains(filter.Keyword));

            switch (filter.OrderBy)
            {
                case 1: query = query.OrderBy(x => x.Id); break;
                default: query = query.OrderByDescending(x => x.Id); break;
            }
            var allSkills = await query.Include(x => x.Field)
            .Skip(filter.Skip).Take(filter.Take).ToListAsync();

            //Getting the skills that he own from the allSkills that we researched
            var ownedSkills = await _context.ContractorSkills.AsNoTracking()
            .Where(x => allSkills.Select(x => x.Id).Contains(x.SkillId) && x.ContractorId == contractorId)
            .Select(x => x.SkillId).ToListAsync();

            //Creating the final list of the allSkills with the IsOwned boolean
            var result = new List<SkillListItemContractor>();
            foreach (var item in allSkills)
            {
                SkillListItemContractor skillToAdd = _mapper.Map<SkillListItemContractor>(item);

                if (ownedSkills.Contains(item.Id))
                {
                    skillToAdd.IsOwned = true;
                }
                else
                {
                    skillToAdd.IsOwned = false;
                }
                result.Add(skillToAdd);
            }
            return new GenericListOutput<SkillListItemContractor>
            {
                ResultList = result,
                TotalCount = await query.CountAsync()
            };
        }
    }

}