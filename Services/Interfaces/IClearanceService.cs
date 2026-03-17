using StudioFlow.DTOs.Clearances;

namespace StudioFlow.Services.Interfaces;

public interface IClearanceService
{
    Task<ClearanceResponse> CreateClearanceAsync(CreateClearanceDto dto);

    Task<List<ClearanceResponse>> GetAllAsync();

    Task<ClearanceResponse?> GetByIdAsync(int id);

    Task<ClearanceResponse> UpdateClearanceAsync(int id, UpdateClearanceDto dto);

    Task ApproveClearanceAsync(int id);

    Task DeleteClearanceAsync(int id);
}