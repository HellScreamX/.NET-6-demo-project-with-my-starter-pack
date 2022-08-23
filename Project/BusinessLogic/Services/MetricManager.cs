using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.DAL;

public interface IMetricManager
{
    Task<int> AddField(Field field);
    Task EditField(Field field);
    Task DeleteField(int id);
    Task<List<Field>> GetFieldList();

    Task<int> AddSkill(Skill skill);
    Task EditSkill(Skill skill);
    Task DeleteSkill(int id);
    Task<GenericListOutput<T>> GetSkillList<T>(SkillSearchFilter filter);

    Task<int> AddBrand(Brand brand);
    Task EditBrand(Brand brand);
    Task DeleteBrand(int id);
    Task<List<Brand>> GetBrandList();

    Task<int> AddModel(Model model);
    Task EditModel(Model model);
    Task DeleteModel(int id);
    Task<GenericListOutput<T>> GetModelList<T>(ModelSearchFilter filter);
}
public class MetricManager : IMetricManager
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public MetricManager(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> AddBrand(Brand brand)
    {
        if (string.IsNullOrWhiteSpace(brand.Label)) throw new CustomException("empty label");

        await _context.Brands.AddAsync(brand);
        await _context.SaveChangesAsync();
        return brand.Id;
    }

    public async Task<int> AddField(Field field)
    {
        if (string.IsNullOrWhiteSpace(field.Label)) throw new CustomException("empty label");

        await _context.Fields.AddAsync(field);
        await _context.SaveChangesAsync();
        return field.Id;
    }

    public async Task<int> AddModel(Model model)
    {
        if (string.IsNullOrWhiteSpace(model.Label)) throw new CustomException("empty label");

        await _context.Models.AddAsync(model);
        await _context.SaveChangesAsync();
        return model.Id;
    }

    public async Task<int> AddSkill(Skill skill)
    {
        if (string.IsNullOrWhiteSpace(skill.Label)) throw new CustomException("empty label");

        await _context.Skills.AddAsync(skill);
        await _context.SaveChangesAsync();
        return skill.Id;
    }

    public async Task DeleteBrand(int id)
    {
        var brandToDelete = await _context.Brands.FindAsync(id) ?? throw new CustomException("brand not found");
        _context.Brands.Remove(brandToDelete);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteField(int id)
    {
        var fieldToDelete = await _context.Fields.FindAsync(id) ?? throw new CustomException("field not found");
        _context.Fields.Remove(fieldToDelete);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteModel(int id)
    {
        var modelToDelete = await _context.Models.FindAsync(id) ?? throw new CustomException("model not found");
        _context.Models.Remove(modelToDelete);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSkill(int id)
    {
        var skillToDelete = await _context.Skills.FindAsync(id) ?? throw new CustomException("skill not found");
        _context.Skills.Remove(skillToDelete);
        await _context.SaveChangesAsync();
    }

    public async Task EditBrand(Brand brand)
    {
        var brandToEdit = await _context.Brands.FindAsync(brand.Id) ?? throw new CustomException("brand not found");
        brandToEdit.Label = brand.Label;
        await _context.SaveChangesAsync();
    }

    public async Task EditField(Field field)
    {
        var fieldToEdit = await _context.Fields.FindAsync(field.Id) ?? throw new CustomException("field not found");
        fieldToEdit.Label = field.Label;
        await _context.SaveChangesAsync();
    }

    public async Task EditModel(Model model)
    {
        var modelToEdit = await _context.Models.FindAsync(model.Id) ?? throw new CustomException("model not found");
        modelToEdit.Label = model.Label;
        await _context.SaveChangesAsync();
    }

    public async Task EditSkill(Skill skill)
    {
        var skillToEdit = await _context.Skills.FindAsync(skill.Id) ?? throw new CustomException("skill not found");
        skillToEdit.Label = skill.Label;
        await _context.SaveChangesAsync();
    }

    public async Task<List<Brand>> GetBrandList()
    {
        var list = await _context.Brands.AsNoTracking().ToListAsync();
        return list;
    }

    public async Task<List<Field>> GetFieldList()
    {
        var list = await _context.Fields.AsNoTracking().ToListAsync();
        return list;
    }

    public async Task<GenericListOutput<T>> GetModelList<T>(ModelSearchFilter filter)
    {
        if(filter.Take<=0 || filter.Take>=50) filter.Take=50;
        var query = _context.Models.AsNoTracking()
        .Where(x=>filter.BrandId==0 || x.BrandId==filter.BrandId)
        .Where(x=>string.IsNullOrWhiteSpace(filter.Keyword) || x.Label.Contains(filter.Keyword));

        switch (filter.OrderBy)
        {
            case 1: query = query.OrderBy(x => x.Id); break;
            default: query = query.OrderByDescending(x => x.Id); break;
        }

        
        var result = await query.Include(x=>x.Brand)
        .Skip(filter.Skip).Take(filter.Take).Select(x => _mapper.Map<T>(x)).ToListAsync();

        return new GenericListOutput<T>
        {
            ResultList = result,
            TotalCount = await query.CountAsync()
        };
    }

    public async Task<GenericListOutput<T>> GetSkillList<T>(SkillSearchFilter filter)
    {
        if(filter.Take<=0 || filter.Take>=50) filter.Take=50;
        var query = _context.Skills.AsNoTracking()
        .Where(x=>filter.FieldId==0 || x.FieldId==filter.FieldId)
        .Where(x=>string.IsNullOrWhiteSpace(filter.Keyword) || x.Label.Contains(filter.Keyword));

        switch (filter.OrderBy)
        {
            case 1: query = query.OrderBy(x => x.Id); break;
            default: query = query.OrderByDescending(x => x.Id); break;
        }
        var result = await query.Include(x=>x.Field)
        .Skip(filter.Skip).Take(filter.Take).Select(x => _mapper.Map<T>(x)).ToListAsync();

        return new GenericListOutput<T>
        {
            ResultList = result,
            TotalCount = await query.CountAsync()
        };
    }
}