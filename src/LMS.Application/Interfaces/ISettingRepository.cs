using LMS.Domain.Entities;

namespace LMS.Application.Interfaces;

public interface ISettingRepository : IGenericRepository<Setting>
{
    Task<Setting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<string?> GetValueAsync(string key, CancellationToken cancellationToken = default);
}