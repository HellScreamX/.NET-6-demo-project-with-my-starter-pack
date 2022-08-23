using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.BusinessLogic.customModels;

[Produces("application/json")]
[Route("api/Commande")]

[ApiController]
public class CommandeController : ControllerBase
{
    private readonly ICommandeManager _commandeManager;

    public CommandeController(ICommandeManager commandeManager)
    {
        _commandeManager = commandeManager;
    }

    [Authorize(Policy = "SuperAdmin")]
    [HttpPost("")]
    public async Task<IActionResult> AddCommande([FromBody]CommandePost commande)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _commandeManager.AddCommande(commande);
        return Ok(result);
    }
    [Authorize(Policy = "SuperAdmin")]
    [HttpPut("")]
    public async Task<IActionResult> EditCommande([FromBody]CommandePut commande)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _commandeManager.EditCommande(commande);
        return Ok();
    }
    [Authorize(Policy = "SuperAdmin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommandeDetails(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _commandeManager.GetCommandeDetails(id, "");
        return Ok(result);
    }
    [Authorize(Policy = "SuperAdmin")]
    [HttpPost("List")]
    public async Task<IActionResult> GetCommandeList([FromBody]CommandeSearchFilter filter)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _commandeManager.GetCommandeList<CommandeListItem>(filter);
        Request.HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "X-Total-Count");
        Request.HttpContext.Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
        return Ok(result.ResultList);
    }
    [Authorize(Policy = "SuperAdmin")]
    [HttpPost("ListDaily")]
    public async Task<IActionResult> GetCommandeList([FromBody]CommandeListDayInput filter)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _commandeManager.GetCommandeList<CommandeListItem>(filter.Day,"");
        Request.HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "X-Total-Count");
        Request.HttpContext.Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
        return Ok(result.ResultList);
    }
    [Authorize(Policy = "SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCommande(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _commandeManager.DeleteCommande(id);
        return Ok();
    }
    [Authorize(Policy = "SuperAdmin")]
    [HttpGet("match/{id}")]
    public async Task<IActionResult> matchSkills(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var matchedContractors = await _commandeManager.MatchMetrics(id);
        return Ok(matchedContractors);
    }
    [Authorize(Policy = "SuperAdmin")]
    [HttpPost("AssociateContractor")]
    public async Task<IActionResult> AssociateContractor(CommandeContractorInput infos)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _commandeManager.AssociateContractor(infos);
        return Ok();
    }
    [Authorize(Policy = "Contractor")]
    [HttpGet("Owner/{id}")]
    public async Task<IActionResult> GetOwnerCommandeDetails(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var contractorId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _commandeManager.GetCommandeDetails(id, contractorId);
        return Ok(result);
    }
    [Authorize(Policy = "Contractor")]
    [HttpPost("Owner/List")]
    public async Task<IActionResult> GetOwnerCommandeList([FromBody]CommandeSearchFilter filter)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        filter.ContractorEmail = HttpContext.User.FindFirstValue(ClaimTypes.Name);
         
        var result = await _commandeManager.GetCommandeList<CommandeListItem>(filter);
        Request.HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "X-Total-Count");
        Request.HttpContext.Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
        return Ok(result.ResultList);
    }
    [Authorize(Policy = "SuperAdmin")]
    [HttpGet("{id}/Contractors")]
    public async Task<IActionResult> GetAssociatedContractors(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _commandeManager.GetAssociatedContractors(id);

        return Ok(result);
    }
    [Authorize(Policy = "SuperAdmin")]
    [HttpPost("RemoveContract")]
    public async Task<IActionResult> DesassociatedContractor(DesassociateContractor infos)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _commandeManager.DesassociatedContractor(infos);

        return Ok();
    }
    [Authorize(Policy = "SuperAdmin")]
    [HttpPost("DailyCount")]
    public async Task<IActionResult> DailyMissionsCount(DateRange infos)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _commandeManager.DailyMissionsCount(infos.Begin, infos.End);

        return Ok(result);
    }
}