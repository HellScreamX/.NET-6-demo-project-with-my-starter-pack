using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.BusinessLogic.customModels;

[Produces("application/json")]
[Route("api/ContractorMetrics")]
[Authorize(Policy = "Contractor")]
[ApiController]
public class ContractorMetricsController : ControllerBase
{
    private readonly IContractorMetricsManager _contractorMetricsManager;

    public ContractorMetricsController(IContractorMetricsManager contractorManager)
    {
        _contractorMetricsManager = contractorManager;
    }

    [HttpPut("skills")]
    public async Task<IActionResult> EditContractorSkills([FromBody]ContractorSkillsPut skills)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var contractorId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _contractorMetricsManager.EditContractorSkills(skills, contractorId);
        return Ok();
    }
    [HttpPut("models")]
    public async Task<IActionResult> EditContractorModels([FromBody]ContractorModelsPut models)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var contractorId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _contractorMetricsManager.EditContractorModels(models, contractorId);
        return Ok();
    }
    [HttpGet("fields")]
    public async Task<IActionResult> GetFieldList()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var contractorId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _contractorMetricsManager.GetFieldList(contractorId);
        return Ok(result);
    }
    [HttpPost("skills")]
    public async Task<IActionResult> GetSkillList(SkillSearchFilterContractor filter)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var contractorId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _contractorMetricsManager.GetSkillList(filter, contractorId);
        Request.HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "X-Total-Count");
        Request.HttpContext.Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
        return Ok(result.ResultList);
    }
    [HttpGet("brands")]
    public async Task<IActionResult> GetBrandList()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var contractorId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _contractorMetricsManager.GetBrandList(contractorId);
        return Ok(result);
    }
    [HttpPost("models")]
    public async Task<IActionResult> GetModelList(ModelSearchFilterContractor filter)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var contractorId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _contractorMetricsManager.GetModelList(filter, contractorId);
        Request.HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "X-Total-Count");
        Request.HttpContext.Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
        return Ok(result.ResultList);
    }

    
}