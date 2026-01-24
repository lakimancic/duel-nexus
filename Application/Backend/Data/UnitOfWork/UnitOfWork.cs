using Backend.Data.Context;
using Backend.Data.Repositories;
using Backend.Data.Repositories.Interfaces;

namespace Backend.Data.UnitOfWork;

public class UnitOfWork(DuelNexusDbContext context) : IUnitOfWork
{
    private readonly DuelNexusDbContext _context = context;
    public IUserRepository Users { get; private set; } = new UserRepository(context);
    private bool _disposed = false;

    public DuelNexusDbContext Context => _context;

    public async Task CompleteAsync() => await _context.SaveChangesAsync();
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
