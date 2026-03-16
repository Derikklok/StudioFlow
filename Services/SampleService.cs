using StudioFlow.DTOs.Samples;
using StudioFlow.Exceptions;
using StudioFlow.Models;
using StudioFlow.Repositories.Interfaces;
using StudioFlow.Services.Interfaces;

namespace StudioFlow.Services;

public class SampleService : ISampleService
{
    private readonly ISampleRepository _sampleRepository;
    private readonly IProjectRepository _projectRepository;

    public SampleService(
        ISampleRepository sampleRepository,
        IProjectRepository projectRepository)
    {
        _sampleRepository = sampleRepository;
        _projectRepository = projectRepository;
    }

    public async Task<SampleResponse> CreateAsync(int projectId, CreateSampleRequest request)
    {
        var project = await _projectRepository.GetByIdAsync(projectId);

        if (project == null)
            throw new ProjectNotFoundException(projectId);

        var sample = new Sample
        {
            ProjectId = projectId,
            Title = request.Title,
            SourceArtist = request.SourceArtist,
            SourceTrack = request.SourceTrack,
            RightsHolder = request.RightsHolder
        };

        var created = await _sampleRepository.CreateAsync(sample);

        return Map(created);
    }

    public async Task<List<SampleResponse>> GetByProjectAsync(int projectId)
    {
        var samples = await _sampleRepository.GetByProjectIdAsync(projectId);

        return samples.Select(Map).ToList();
    }

    public async Task<SampleResponse> GetByIdAsync(int id)
    {
        var sample = await _sampleRepository.GetByIdAsync(id);

        if (sample == null)
            throw new SampleNotFoundException(id);

        return Map(sample);
    }

    public async Task<SampleResponse> UpdateAsync(int id, UpdateSampleRequest request)
    {
        var sample = await _sampleRepository.GetByIdAsync(id);

        if (sample == null)
            throw new SampleNotFoundException(id);

        sample.Title = request.Title;
        sample.SourceArtist = request.SourceArtist;
        sample.SourceTrack = request.SourceTrack;
        sample.RightsHolder = request.RightsHolder;
        sample.Status = request.Status;
        sample.UpdatedAt = DateTime.UtcNow;

        var updated = await _sampleRepository.UpdateAsync(sample);

        return Map(updated);
    }


    public async Task<SampleResponse> PatchAsync(int id, PatchSampleRequest request)
    {
        var sample = await _sampleRepository.GetByIdAsync(id);

        if (sample == null)
            throw new SampleNotFoundException(id);

        if (!string.IsNullOrEmpty(request.Title))
            sample.Title = request.Title;

        if (request.SourceArtist != null)
            sample.SourceArtist = request.SourceArtist;

        if (request.SourceTrack != null)
            sample.SourceTrack = request.SourceTrack;

        if (request.RightsHolder != null)
            sample.RightsHolder = request.RightsHolder;

        if (request.Status.HasValue)
            sample.Status = request.Status.Value;

        sample.UpdatedAt = DateTime.UtcNow;

        var updated = await _sampleRepository.UpdateAsync(sample);

        return Map(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var sample = await _sampleRepository.GetByIdAsync(id);

        if (sample == null)
            throw new SampleNotFoundException(id);

        await _sampleRepository.DeleteAsync(sample);
    }

    // Mapper
    private static SampleResponse Map(Sample sample)
    {
        return new SampleResponse
        {
            Id = sample.Id,
            ProjectId = sample.ProjectId,
            Title = sample.Title,
            SourceArtist = sample.SourceArtist,
            SourceTrack = sample.SourceTrack,
            RightsHolder = sample.RightsHolder,
            Status = sample.Status,
            CreatedAt = sample.CreatedAt,
            UpdatedAt = sample.UpdatedAt
        };
    }
}