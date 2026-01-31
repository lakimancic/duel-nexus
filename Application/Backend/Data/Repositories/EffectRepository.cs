using Backend.Data.Context;
using Backend.Data.Models;
using Backend.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories;

public class EffectRepository(DuelNexusDbContext context) : Repository<Effect>(context), IEffectRepository
{   
}