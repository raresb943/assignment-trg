using Dispatcher.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Dispatcher.Core.Services;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<BrowsingTask> BrowsingTasks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BrowsingTask>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Url).IsRequired();
            entity.Property(e => e.Result).HasColumnType("text");
            entity.Property(e => e.Status).HasConversion<string>();
        });

        base.OnModelCreating(modelBuilder);
    }
}
