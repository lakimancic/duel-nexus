using Backend.Data.Models;

namespace Backend.Data.Repositories.Interfaces;

public interface IMessageRepository : IRepository<ChatMessage>
{
    Task<ChatMessage?> GetWithSenderById(Guid messageId);
}
