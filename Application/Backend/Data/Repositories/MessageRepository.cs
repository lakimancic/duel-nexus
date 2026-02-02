using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;

namespace Backend.Data.Repositories;

public class MessageRepository(DuelNexusDbContext context) : Repository<ChatMessage>(context), IMessageRepository
{
}
