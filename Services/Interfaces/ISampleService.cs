using StudioFlow.DTOs.Samples;

namespace StudioFlow.Services.Interfaces;

public interface ISampleService
{
    Task<SampleResponse> CreateAsync(int projectId, CreateSampleRequest request);

    Task<List<SampleResponse>> GetByProjectAsync(int projectId);

    Task<SampleResponse> GetByIdAsync(int id);

    Task<SampleResponse> UpdateAsync(int id, UpdateSampleRequest request);

    Task<SampleResponse> PatchAsync(int id, PatchSampleRequest request);

    Task DeleteAsync(int id);
}