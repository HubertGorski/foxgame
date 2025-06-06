using FoxTales.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoxTales.Infrastructure.Data;

public class FoxTalesDbContext(DbContextOptions<FoxTalesDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<DylematyCard> DylematyCards { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.Property(u => u.Password).IsRequired();
        });

        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.ToTable("Achievements");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Title).IsRequired().HasMaxLength(32);
            entity.Property(u => u.Subtitle).IsRequired().HasMaxLength(124);
            entity.Property(u => u.Description).IsRequired().HasMaxLength(256);
        });

        modelBuilder.Entity<DylematyCard>(entity =>
        {
            entity.ToTable("DylematyCards");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Text).IsRequired().HasMaxLength(256);
            entity.Property(u => u.Type).IsRequired().HasMaxLength(32);
            entity.HasOne(c => c.Owner)
                .WithMany(u => u.Cards)
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
