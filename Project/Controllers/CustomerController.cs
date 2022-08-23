using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.BusinessLogic.customModels;

[Produces("application/json")]
[Route("api/Customer")]
[Authorize(Policy = "SuperAdmin")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ICustomerManager _customerManager;

    public CustomerController(ICustomerManager customerManager)
    {
        _customerManager = customerManager;
    }

    
    [HttpPost("")]
    public async Task<IActionResult> AddCustomer([FromBody]CustomerPost customer)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _customerManager.AddCustomer(customer);
        return Ok(result);
    }
    [HttpPut("")]
    public async Task<IActionResult> EditCustomer([FromBody]CustomerPut customer)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _customerManager.EditCustomer(customer);
        return Ok();
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomerDetails(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _customerManager.GetCustomerDetails(id);
        return Ok(result);
    }
    [HttpPost("List")]
    public async Task<IActionResult> GetCustomerList([FromBody]CustomerSearchFilter filter)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _customerManager.GetCustomerList<CustomerListItem>(filter);
        Request.HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "X-Total-Count");
        Request.HttpContext.Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
        return Ok(result.ResultList);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _customerManager.DeleteCustomer(id);
        return Ok();
    }
}