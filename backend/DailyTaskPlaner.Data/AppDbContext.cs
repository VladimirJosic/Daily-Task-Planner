using DailyTaskPlaner.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DailyTaskPlaner.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<DailyTask> DailyTasks { get; set; }
    public DbSet<SharedTask> SharedTasks { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<AIChatHistory> AIChatHistory { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        base.OnConfiguring(options);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DailyTask>()
            .HasOne(dt => dt.User)
            .WithMany(u => u.OwnedTasks)
            .HasForeignKey(dt => dt.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<SharedTask>()
           .HasKey(st => new { st.UserId, st.DailyTaskId }); 

        modelBuilder.Entity<SharedTask>()
            .HasOne(st => st.User)
            .WithMany(u => u.SharedTasks)
            .HasForeignKey(st => st.UserId);

        modelBuilder.Entity<SharedTask>()
            .HasOne(st => st.DailyTask)
            .WithMany(dt => dt.SharedWith)
            .HasForeignKey(st => st.DailyTaskId);

        base.OnModelCreating(modelBuilder);
    }
}
