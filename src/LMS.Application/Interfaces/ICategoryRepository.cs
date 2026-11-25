using LMS.Domain.Entities;

namespace LMS.Application.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<CategoryStatsInfo>> GetCategoryStatsAsync(CancellationToken cancellationToken = default);
}

public class CategoryStatsInfo
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int TotalBooks { get; set; }
    public int TotalLoans { get; set; }
}