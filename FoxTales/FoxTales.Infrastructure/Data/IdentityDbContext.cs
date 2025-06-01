using Hub.Identity.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoxTales.Infrastructure.Data;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Achievement> Achievements { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.Property(u => u.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.ToTable("Achievements");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Title).IsRequired().HasMaxLength(32);
            entity.Property(u => u.Subtitle).IsRequired().HasMaxLength(124);
            entity.Property(u => u.Description).IsRequired().HasMaxLength(256);
        });
    }
}
