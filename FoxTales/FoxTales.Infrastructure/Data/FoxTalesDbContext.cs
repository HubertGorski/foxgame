using FoxTales.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoxTales.Infrastructure.Data;

public class FoxTalesDbContext(DbContextOptions<FoxTalesDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<DylematyCard> DylematyCards { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<FoxGame> FoxGames { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<UserLimit> UserLimits { get; set; }
    public DbSet<LimitDefinition> LimitDefinitions { get; set; }
    public DbSet<LimitThreshold> LimitThresholds { get; set; }
    public DbSet<Avatar> Avatars { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Catalog> Catalogs { get; set; }
    public DbSet<CatalogType> CatalogTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.UserId);
            entity.Property(u => u.UserId).ValueGeneratedOnAdd();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.RoleId).IsRequired().HasDefaultValue(1);
            entity.Property(u => u.AvatarId).IsRequired().HasDefaultValue(1);
            entity.HasMany(u => u.UserLimits)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId)
                .IsRequired();
        });

        modelBuilder.Entity<Catalog>(entity =>
        {
            entity.HasOne(c => c.Owner)
                .WithMany(u => u.Catalogs)
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(c => c.CatalogType)
                .WithMany()
                .HasForeignKey(c => c.CatalogTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(c => c.Questions)
                .WithMany(q => q.Catalogs)
                .UsingEntity<Dictionary<string, object>>(
                    "CatalogQuestions",
                    j => j.HasOne<Question>()
                        .WithMany()
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Catalog>()
                        .WithMany()
                        .HasForeignKey("CatalogId")
                        .OnDelete(DeleteBehavior.Restrict),
                    j =>
                    {
                        j.HasKey("CatalogId", "QuestionId");
                        j.ToTable("CatalogQuestions");
                    });
        });

        modelBuilder.Entity<CatalogType>(entity =>
        {
            entity.ToTable("CatalogTypes");
            entity.HasKey(u => u.CatalogTypeId);
            entity.Property(u => u.CatalogTypeId).ValueGeneratedNever();
            entity.Property(u => u.Name).IsRequired().HasConversion<string>();
            entity.Property(u => u.Size).IsRequired();
            entity.HasMany(q => q.Catalogs)
                .WithMany(c => c.AvailableTypes)
                .UsingEntity<Dictionary<string, object>>(
                    "CatalogTypeDefinitions",
                    j => j.HasOne<Catalog>()
                        .WithMany()
                        .HasForeignKey("CatalogId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<CatalogType>()
                        .WithMany()
                        .HasForeignKey("CatalogTypeId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("CatalogId", "CatalogTypeId");
                        j.ToTable("CatalogTypeDefinitions");
                    });
        });


        modelBuilder.Entity<UserLimit>(entity =>
        {
            entity.HasKey(ul => new { ul.UserId, ul.Type, ul.LimitId });

            entity.HasOne(ul => ul.LimitDefinition)
                .WithMany()
                .HasForeignKey(ul => new { ul.Type, ul.LimitId })
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<LimitDefinition>(entity =>
        {
            entity.HasKey(ld => new { ld.Type, ld.LimitId });
        });

        modelBuilder.Entity<LimitThreshold>(entity =>
        {
            entity.HasKey(lt => lt.Id);

            entity.HasOne(lt => lt.LimitDefinition)
                .WithMany(ld => ld.Thresholds)
                .HasForeignKey(lt => new { lt.Type, lt.LimitId })
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(u => u.RoleId);
            entity.Property(u => u.RoleId).ValueGeneratedNever();
            entity.Property(u => u.Name).IsRequired().HasConversion<string>();
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

        modelBuilder.Entity<FoxGame>(entity =>
        {
            entity.ToTable("FoxGames");
            entity.HasKey(u => u.FoxGameId);
            entity.Property(u => u.FoxGameId).ValueGeneratedNever();
            entity.Property(u => u.Name).IsRequired().HasMaxLength(32).HasConversion<string>();
        });

        modelBuilder.Entity<Avatar>(entity =>
        {
            entity.ToTable("Avatars");
            entity.HasKey(u => u.AvatarId);
            entity.Property(u => u.AvatarId).ValueGeneratedNever();
            entity.Property(u => u.IsPremium).IsRequired();
            entity.Property(u => u.Source).IsRequired();
            entity.Property(u => u.Name).IsRequired().HasMaxLength(32).HasConversion<string>();
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");
            entity.HasKey(u => u.TokenId);
            entity.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
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

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(q => q.Id);
            entity.Property(q => q.Text).IsRequired();
            entity.Property(q => q.Language).IsRequired();
            entity.Property(q => q.CreatedDate).IsRequired();
            entity.Property(q => q.IsPublic).IsRequired();
            entity.HasOne(q => q.Owner)
                .WithMany(u => u.Questions)
                .HasForeignKey(q => q.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
