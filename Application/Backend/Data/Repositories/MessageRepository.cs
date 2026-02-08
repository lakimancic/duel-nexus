using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class MessageRepository(DuelNexusDbContext context) : Repository<ChatMessage>(context), IMessageRepository
{
    public Task<ChatMessage?> GetWithSenderById(Guid messageId)
    {
        return _dbSet
            .Where(cm => cm.Id == messageId)
            .Include(cm => cm.Sender)
            .FirstOrDefaultAsync();
    }
}
