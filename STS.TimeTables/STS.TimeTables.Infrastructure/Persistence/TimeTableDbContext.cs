using Microsoft.EntityFrameworkCore;
using STS.TimeTables.Domain.Entities;

namespace STS.TimeTables.Infrastructure.Persistence;

public class TimeTableDbContext : DbContext
{
    public DbSet<League> Leagues { get; set; }
    public DbSet<Match> Matches { get; set; }
    
    public TimeTableDbContext(DbContextOptions<TimeTableDbContext> options)
        : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<League>(entity =>
        {
            entity.Property(e => e.GeneratedAt)
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        });
        
        modelBuilder.Entity<Match>(entity =>
        {
            entity.Property(e => e.Date)
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
        });
    }
}