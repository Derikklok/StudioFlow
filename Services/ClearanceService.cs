using StudioFlow.DTOs.Clearances;
using StudioFlow.Models;
using StudioFlow.Repositories.Interfaces;
using StudioFlow.Services.Interfaces;

namespace StudioFlow.Services;

public class ClearanceService : IClearanceService
{
    private readonly IClearanceRepository _repository;

    public ClearanceService(IClearanceRepository repository)
    {
        _repository = repository;
    }

    public async Task<ClearanceResponse> CreateClearanceAsync(CreateClearanceDto dto)
    {
        var clearance = new Clearance
        {
            SampleId = dto.SampleId,
            RightsOwner = dto.RightsOwner,
            LicenseType = dto.LicenseType,
            Notes = dto.Notes
        };

        await _repository.AddAsync(clearance);
        await _repository.SaveChangesAsync();

        return MapToDto(clearance);
    }

    public async Task<List<ClearanceResponse>> GetAllAsync()
    {
        var clearances = await _repository.GetAllAsync();

        return clearances.Select(MapToDto).ToList();
    }

    public async Task<ClearanceResponse?> GetByIdAsync(int id)
    {
        var clearance = await _repository.GetByIdAsync(id);

        if (clearance == null)
            return null;

        return MapToDto(clearance);
    }

    public async Task ApproveClearanceAsync(int id)
    {
        var clearance = await _repository.GetByIdAsync(id);

        if (clearance == null)
            throw new Exception("Clearance not found");

        clearance.IsApproved = true;
        clearance.ApprovedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
    }

    private static ClearanceResponse MapToDto(Clearance c)
    {
        return new ClearanceResponse
        {
            Id = c.Id,
            SampleId = c.SampleId,
            RightsOwner = c.RightsOwner,
            LicenseType = c.LicenseType,
            IsApproved = c.IsApproved,
            ApprovedAt = c.ApprovedAt,
            Notes = c.Notes,
            CreatedAt = c.CreatedAt
        };
    }
}