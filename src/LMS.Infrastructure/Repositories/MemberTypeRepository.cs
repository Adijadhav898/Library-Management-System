using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories;

public class MemberTypeRepository : GenericRepository<MemberType>, IMemberTypeRepository
{
    public MemberTypeRepository(LibraryDbContext context) : base(context) { }

    public async Task<MemberType?> GetDefaultAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(mt => mt.Name == "General", cancellationToken);
    }
}