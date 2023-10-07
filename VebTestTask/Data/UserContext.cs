using Microsoft.EntityFrameworkCore;
using VebTestTask.Models;

namespace VebTestTask.Data;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<UserRole>();
            // .UsingEntity(
            //     j =>
            //     {
            //         j.IndexerProperty<int>("Id");
            //         j.HasKey("Id");
            //     });
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
}