using LMS.Domain.Entities;

namespace LMS.Application.Interfaces;

public interface IMemberTypeRepository : IGenericRepository<MemberType>
{
    Task<MemberType?> GetDefaultAsync(CancellationToken cancellationToken = default);
}