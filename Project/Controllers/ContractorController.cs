using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("Api/Contractor")]
[Produces("application/json")]
public class ContractorController : ControllerBase
{
    IContractorManager _contractorManager;
    public ContractorController(IContractorManager contractorManager)
    {
        _contractorManager = contractorManager;
    }
    [Authorize(Policy = "Contractor")]
    [HttpPost("UnavDays")]
    public async Task<IActionResult> AddUnavDays(List<DateTime> days)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var contractorId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _contractorManager.AddUnavailableDays(contractorId, days);
        return Ok();
    }
    [Authorize(Policy = "Contractor")]
    [HttpDelete("UnavDays")]
    public async Task<IActionResult> RemoveUnavDays(List<DateTime> days)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var contractorId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _contractorManager.RemoveUnavailableDays(contractorId, days);
        return Ok();
    }
    [Authorize(Policy = "Contractor")]
    [HttpPost("UnavDays/list")]
    public async Task<IActionResult> GetUnavDays(DateRange range)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var contractorEmail = HttpContext.User.FindFirstValue(ClaimTypes.Name);
        var result = await _contractorManager.GetUnavailableDays(contractorEmail, range);
        return Ok(result);
    }
    [Authorize(Policy = "Contractor")]
    [HttpPost("CommandeDays")]
    public async Task<IActionResult> CommandeDays(DateRange range)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var contractorEmail = HttpContext.User.FindFirstValue(ClaimTypes.Name);
        var result = await _contractorManager.GetCommandeDays(contractorEmail, range);
        return Ok(result);
    }
    [Authorize(Policy = "SuperAdmin")]
    [HttpPost("Any/UnavDays/list")]
    public async Task<IActionResult> GetAnyContractorUnavDays(DateRangeWithContractor infos)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _contractorManager
        .GetUnavailableDays(infos.ContractorEmail, new DateRange{Begin=infos.Begin, End=infos.End});

        return Ok(result);
    }
    [Authorize(Policy = "SuperAdmin")]
    [HttpPost("Any/CommandeDays")]
    public async Task<IActionResult> GetAnyContractorCommandeDays(DateRangeWithContractor infos)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _contractorManager.GetCommandeDays(infos.ContractorEmail, new DateRange{Begin=infos.Begin, End=infos.End});
        return Ok(result);
    }
    [Authorize(Policy = "SuperAdmin")]
    [HttpPost("DailyCount")]
    public async Task<IActionResult> DailyCount(DateRange infos)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _contractorManager.GetCountDaily(infos.Begin, infos.End);

        return Ok(result);
    }
    [Authorize(Policy = "SuperAdmin")]
    [HttpPost("SpecificDay")]
    public async Task<IActionResult> SpecificDay(ContractorListDayInput infos)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _contractorManager.WorkingByDay(infos.Day);

        return Ok(result);
    }
}