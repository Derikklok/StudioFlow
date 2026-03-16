using Microsoft.AspNetCore.Mvc;
using StudioFlow.DTOs.Projects;
using StudioFlow.Services.Interfaces;

namespace StudioFlow.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    
    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }
    
    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> Create(CreateProjectRequest request)
    {
        var project = await _projectService.CreateAsync(request);

        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    [HttpGet]
    public async Task<ActionResult<List<ProjectResponse>>> GetAll()
    {
        return Ok(await _projectService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectResponse>> GetById(int id)
    {
        return Ok(await _projectService.GetByIdAsync(id));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProjectResponse>> Update(int id, UpdateProjectRequest request)
    {
        return Ok(await _projectService.UpdateAsync(id, request));
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<ProjectResponse>> Patch(int id, PatchProjectRequest request)
    {
        return Ok(await _projectService.PatchAsync(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _projectService.DeleteAsync(id);

        return NoContent();
    }
}