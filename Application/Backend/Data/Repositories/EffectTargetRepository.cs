using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;

namespace Backend.Data.Repositories;

public class EffectTargetRepository(DuelNexusDbContext context) : Repository<EffectTarget>(context), IEffectTargetRepository
{
}
