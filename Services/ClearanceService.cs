using StudioFlow.DTOs.Clearances;
using StudioFlow.Exceptions;
using StudioFlow.Models;
using StudioFlow.Repositories.Interfaces;
using StudioFlow.Services.Interfaces;

namespace StudioFlow.Services;

public class ClearanceService : IClearanceService
{
    private readonly IClearanceRepository _repository;
    private readonly ISampleRepository _sampleRepository;

    public ClearanceService(IClearanceRepository repository, ISampleRepository sampleRepository)
    {
        _repository = repository;
        _sampleRepository = sampleRepository;
    }

    public async Task<ClearanceResponse> CreateClearanceAsync(CreateClearanceDto dto)
    {
        // Validate that the sample exists
        var sample = await _sampleRepository.GetByIdAsync(dto.SampleId);
        if (sample == null)
            throw new SampleNotFoundException(dto.SampleId);

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

    public async Task<ClearanceResponse> UpdateClearanceAsync(int id, UpdateClearanceDto dto)
    {
        var clearance = await _repository.GetByIdAsync(id);

        if (clearance == null)
            throw new ClearanceNotFoundException(id);

        if (dto.RightsOwner != null)
            clearance.RightsOwner = dto.RightsOwner;

        if (dto.LicenseType != null)
            clearance.LicenseType = dto.LicenseType;

        if (dto.Notes != null)
            clearance.Notes = dto.Notes;

        if (dto.IsApproved.HasValue && dto.IsApproved.Value && !clearance.IsApproved)
        {
            clearance.IsApproved = true;
            clearance.ApprovedAt = DateTime.UtcNow;
        }

        await _repository.UpdateAsync(clearance);
        await _repository.SaveChangesAsync();

        return MapToDto(clearance);
    }

    public async Task ApproveClearanceAsync(int id)
    {
        var clearance = await _repository.GetByIdAsync(id);

        if (clearance == null)
            throw new ClearanceNotFoundException(id);

        if (clearance.IsApproved)
            throw new InvalidOperationException("Clearance is already approved.");

        clearance.IsApproved = true;
        clearance.ApprovedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
    }

    public async Task DeleteClearanceAsync(int id)
    {
        var clearance = await _repository.GetByIdAsync(id);

        if (clearance == null)
            throw new ClearanceNotFoundException(id);

        await _repository.DeleteAsync(id);
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

