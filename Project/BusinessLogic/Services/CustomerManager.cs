using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.DAL;

public interface ICustomerManager{
    Task<int> AddCustomer(CustomerPost customerPost);
    Task EditCustomer(CustomerPut customerPut);
    Task DeleteCustomer(int id);
    Task<GenericListOutput<T>> GetCustomerList<T>(CustomerSearchFilter model);
    Task<CustomerDetails> GetCustomerDetails(int id);
}

public class CustomerManager : ICustomerManager
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public CustomerManager(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<int> AddCustomer(CustomerPost customerPost)
    {
        if (string.IsNullOrWhiteSpace(customerPost.Name)) throw new CustomException("empty name");
        Customer customer = new Customer();
        _mapper.Map<CustomerPost, Customer>(customerPost, customer);
        customer.CreationDate= DateTime.Now;
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
        return customer.Id;
    }

    public async Task DeleteCustomer(int id)
    {
        var customer = await _context.Customers.FindAsync(id) ?? throw new CustomException("customer not found");
        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
    }

    public async Task EditCustomer(CustomerPut customerPut)
    {
        if (string.IsNullOrWhiteSpace(customerPut.Name)) throw new CustomException("empty name");
        var customer = await _context.Customers.FindAsync(customerPut.Id) ?? throw new CustomException("customer not found");
        _mapper.Map<CustomerPut, Customer>(customerPut, customer);
        await _context.SaveChangesAsync();
    }

    public async Task<CustomerDetails> GetCustomerDetails(int id)
    {
        var customer = await _context.Customers.FindAsync(id) ?? throw new CustomException("customer not found");
        return _mapper.Map<CustomerDetails>(customer);
    }

    public async Task<GenericListOutput<T>> GetCustomerList<T>(CustomerSearchFilter filter)
    {
        if(filter.Take<=0 || filter.Take>=50) filter.Take=50;
        var query = _context.Customers.AsNoTracking()
        .Where(x => String.IsNullOrWhiteSpace(filter.Name) || x.Name.Contains(filter.Name))
        .Where(c => filter.BeginDate == DateTime.MinValue || c.CreationDate >= filter.BeginDate)
        .Where(c => filter.EndDate == DateTime.MinValue || c.CreationDate <= filter.EndDate);
        switch (filter.OrderBy)
        {
            case 1: query = query.OrderBy(x => x.Id); break;
            default: query = query.OrderByDescending(x => x.Id); break;
        }
        var result = await query
        .Skip(filter.Skip).Take(filter.Take).Select(x => _mapper.Map<Customer, T>(x)).ToListAsync();

        return new GenericListOutput<T>
        {
            ResultList = result,
            TotalCount = await query.CountAsync()
        };
    }
}