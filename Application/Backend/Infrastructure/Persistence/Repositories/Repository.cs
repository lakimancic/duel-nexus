using AutoMapper;
using Backend.Infrastructure.Persistence.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Persistence.Repositories;

public class Repository<TDomain, TEntity>(DuelNexusDbContext context, IMapper mapper) : IRepository<TDomain>
    where TEntity : class
    where TDomain : class
{
    protected readonly DuelNexusDbContext _context = context;
    protected readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
    protected readonly IMapper _mapper = mapper;

    public async Task AddAsync(TDomain domainEntity)
    {
        var dbEntity = _mapper.Map<TEntity>(domainEntity);
        _dbSet.Add(dbEntity);
        await _context.SaveChangesAsync();
    }

    public async Task<TDomain?> GetByIdAsync(Guid id)
    {
        var dbEntity = await _dbSet.FindAsync(id);
        return _mapper.Map<TDomain>(dbEntity);
    }

    public async Task<IEnumerable<TDomain>> GetAllAsync()
    {
        var dbEntities = await _dbSet.ToListAsync();
        return _mapper.Map<IEnumerable<TDomain>>(dbEntities);
    }

    public async Task UpdateAsync(TDomain domainEntity)
    {
        var dbEntity = _mapper.Map<TEntity>(domainEntity);
        _dbSet.Update(dbEntity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TDomain domainEntity)
    {
        var dbEntity = _mapper.Map<TEntity>(domainEntity);
        _dbSet.Remove(dbEntity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.FindAsync(id) != null;
    }
}
