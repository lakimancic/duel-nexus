using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;

namespace Backend.Data.Repositories;

public class PlayerGameRepository(DuelNexusDbContext context) : Repository<PlayerGame>(context), IPlayerGameRepository
{

}
