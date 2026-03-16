using Microsoft.AspNetCore.Mvc;
using StudioFlow.DTOs.Samples;
using StudioFlow.Services.Interfaces;

namespace StudioFlow.Controllers;

[ApiController]
[Route("api")]
public class SamplesController : ControllerBase
{
    private readonly ISampleService _sampleService;

    public SamplesController(ISampleService sampleService)
    {
        _sampleService = sampleService;
    }
    
    [HttpPost("projects/{projectId}/samples")]
    public async Task<ActionResult<SampleResponse>> Create(
        int projectId,
        CreateSampleRequest request)
    {
        var sample = await _sampleService.CreateAsync(projectId, request);

        return CreatedAtAction(nameof(GetById), new { id = sample.Id }, sample);
    }
    
    [HttpGet("projects/{projectId}/samples")]
    public async Task<ActionResult<List<SampleResponse>>> GetByProject(int projectId)
    {
        return Ok(await _sampleService.GetByProjectAsync(projectId));
    }

    [HttpGet("samples/{id}")]
    public async Task<ActionResult<SampleResponse>> GetById(int id)
    {
        return Ok(await _sampleService.GetByIdAsync(id));
    }
    
    [HttpPut("samples/{id}")]
    public async Task<ActionResult<SampleResponse>> Update(
        int id,
        UpdateSampleRequest request)
    {
        return Ok(await _sampleService.UpdateAsync(id, request));
    }

    [HttpPatch("samples/{id}")]
    public async Task<ActionResult<SampleResponse>> Patch(
        int id,
        PatchSampleRequest request)
    {
        return Ok(await _sampleService.PatchAsync(id, request));
    }

    [HttpDelete("samples/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _sampleService.DeleteAsync(id);
        return NoContent();
    }
}