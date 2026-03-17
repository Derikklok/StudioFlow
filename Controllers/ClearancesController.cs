using Microsoft.AspNetCore.Mvc;
using StudioFlow.DTOs.Clearances;
using StudioFlow.Services.Interfaces;

namespace StudioFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClearancesController:ControllerBase
{
    private readonly IClearanceService _service;

    public ClearancesController(IClearanceService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClearanceDto dto)
    {
        var result = await _service.CreateClearanceAsync(dto);
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clearances = await _service.GetAllAsync();
        return Ok(clearances);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var clearance = await _service.GetByIdAsync(id);

        if (clearance == null)
            return NotFound();

        return Ok(clearance);
    }
    
    [HttpPut("{id}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        await _service.ApproveClearanceAsync(id);
        return Ok("Clearance approved");
    }
}