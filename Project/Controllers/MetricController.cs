using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.BusinessLogic.customModels;

[Produces("application/json")]
[Route("api/Metric")]
[Authorize(Policy = "AdminOrContractor")]
[ApiController]
public class MetricController : ControllerBase
{
    private readonly IMetricManager _metricManager;

    public MetricController(IMetricManager metricManager)
    {
        _metricManager = metricManager;
    }

    [HttpPost("field")]
    public async Task<IActionResult> AddField([FromBody] Field field)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _metricManager.AddField(field);
        return Ok(result);
    }
    [HttpPost("skill")]
    public async Task<IActionResult> AddSkill([FromBody] Skill skill)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _metricManager.AddSkill(skill);
        return Ok(result);
    }
    [HttpPost("brand")]
    public async Task<IActionResult> AddBrand([FromBody] Brand brand)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _metricManager.AddBrand(brand);
        return Ok(result);
    }
    [HttpPost("model")]
    public async Task<IActionResult> AddModel([FromBody] Model model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _metricManager.AddModel(model);
        return Ok(result);
    }

    [HttpPut("field")]
    public async Task<IActionResult> EditField([FromBody] Field field)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _metricManager.EditField(field);
        return Ok();
    }
    [HttpPut("skill")]
    public async Task<IActionResult> EditSkill([FromBody] Skill skill)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _metricManager.EditSkill(skill);
        return Ok();
    }
    [HttpPut("brand")]
    public async Task<IActionResult> EditBrand([FromBody] Brand brand)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _metricManager.EditBrand(brand);
        return Ok();
    }
    [HttpPut("model")]
    public async Task<IActionResult> EditModel([FromBody] Model model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _metricManager.EditModel(model);
        return Ok();
    }

    [HttpDelete("field/{id}")]
    public async Task<IActionResult> DeleteField(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _metricManager.DeleteField(id);
        return Ok();
    }
    [HttpDelete("skill/{id}")]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _metricManager.DeleteSkill(id);
        return Ok();
    }
    [HttpDelete("brand/{id}")]
    public async Task<IActionResult> DeleteBrand(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _metricManager.DeleteBrand(id);
        return Ok();
    }
    [HttpDelete("model/{id}")]
    public async Task<IActionResult> DeleteModel(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _metricManager.DeleteModel(id);
        return Ok();
    }


    [HttpGet("fields")]
    public async Task<IActionResult> GetFieldList()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _metricManager.GetFieldList();
        return Ok(result);
    }
    [HttpPost("skills")]
    public async Task<IActionResult> GetSkillList(SkillSearchFilter filter)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _metricManager.GetSkillList<SkillListItem>(filter);
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
        var result = await _metricManager.GetBrandList();
        return Ok(result);
    }
    [HttpPost("models")]
    public async Task<IActionResult> GetModelList(ModelSearchFilter filter)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _metricManager.GetModelList<ModelListItem>(filter);
        Request.HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "X-Total-Count");
        Request.HttpContext.Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
        return Ok(result.ResultList);
    }
}