using FoxTales.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoxTales.Infrastructure.Data;

public class FoxTalesDbContext(DbContextOptions<FoxTalesDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<DylematyCard> DylematyCards { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.UserId);
            entity.Property(u => u.UserId).ValueGeneratedOnAdd();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.Property(u => u.Password).IsRequired();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(u => u.RoleId);
            entity.Property(u => u.RoleId).ValueGeneratedOnAdd();
            entity.Property(u => u.Name).IsRequired();
        });

        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.ToTable("Achievements");
            entity.HasKey(u => u.AchievementId);
            entity.Property(u => u.AchievementId).ValueGeneratedOnAdd();
            entity.Property(u => u.Title).IsRequired().HasMaxLength(32);
            entity.Property(u => u.Subtitle).IsRequired().HasMaxLength(124);
            entity.Property(u => u.Description).IsRequired().HasMaxLength(256);
        });

        modelBuilder.Entity<DylematyCard>(entity =>
        {
            entity.ToTable("DylematyCards");
            entity.HasKey(u => u.CardId);
            entity.Property(u => u.CardId).ValueGeneratedOnAdd();
            entity.Property(u => u.Text).IsRequired().HasMaxLength(256);
            entity.Property(u => u.Type).IsRequired().HasMaxLength(32);
            entity.HasOne(c => c.Owner)
                .WithMany(u => u.Cards)
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
