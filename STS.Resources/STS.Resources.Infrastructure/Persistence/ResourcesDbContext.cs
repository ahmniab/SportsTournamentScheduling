using Microsoft.EntityFrameworkCore;
using STS.Resources.Domain.Entities;

namespace STS.Resources.Infrastructure.Persistence;

public class ResourcesDbContext : DbContext
{
    public DbSet<League> Leagues => Set<League>();
    public ResourcesDbContext(DbContextOptions<ResourcesDbContext> options)
        : base(options){}
}