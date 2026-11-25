using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class SettingRepository : GenericRepository<Setting>, ISettingRepository
{
    public SettingRepository(LibraryDbContext context) : base(context) { }

    public async Task<Setting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
    }

    public async Task<string?> GetValueAsync(string key, CancellationToken cancellationToken = default)
    {
        var setting = await _dbSet.FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
        return setting?.Value;
    }

    public async Task<T?> GetValueAsync<T>(string key, CancellationToken cancellationToken = default) where T : struct
    {
        var value = await GetValueAsync(key, cancellationToken);
        if (string.IsNullOrEmpty(value))
            return null;

        var type = typeof(T);
        if (type == typeof(int) && int.TryParse(value, out var intVal))
            return (T)(object)intVal;
        if (type == typeof(decimal) && decimal.TryParse(value, out var decVal))
            return (T)(object)decVal;
        if (type == typeof(bool) && bool.TryParse(value, out var boolVal))
            return (T)(object)boolVal;
        if (type == typeof(DateTime) && DateTime.TryParse(value, out var dtVal))
            return (T)(object)dtVal;

        return null;
    }

    public async Task SetValueAsync(string key, string value, string? description = null, CancellationToken cancellationToken = default)
    {
        var setting = await _dbSet.FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
        if (setting != null)
        {
            setting.Value = value;
            if (description != null)
                setting.Description = description;
            setting.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            setting = new Setting
            {
                Id = Guid.NewGuid(),
                Key = key,
                Value = value,
                Description = description
            };
            await _dbSet.AddAsync(setting, cancellationToken);
        }
    }

    public async Task<IEnumerable<Setting>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.OrderBy(s => s.Key).ToListAsync(cancellationToken);
    }
}