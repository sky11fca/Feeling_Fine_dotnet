using DotnetApi.Domains.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Infrastructure.Persistance;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Reply> Replies => Set<Reply>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("public");
        modelBuilder.Entity<Business>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired();
            e.Property(x => x.Industry).IsRequired();
            e.Property(x => x.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<Review>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.BusinessId).IsRequired();
            e.Property(x => x.RawText).IsRequired();
            e.Property(x => x.SubmittedOn).IsRequired();
            e.Property(x => x.CreatedAt).IsRequired();
        });
        
        modelBuilder.Entity<Client>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Username).IsRequired();
            e.Property(x => x.Email).IsRequired();
            e.Property(x => x.PhoneNumber).IsRequired();
        });

        modelBuilder.Entity<Reply>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.ReviewId).IsRequired();
            e.Property(x => x.ToClientId).IsRequired();
            e.Property(x => x.RawText).IsRequired();
        });
        
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.BusinessId).IsRequired();
            e.Property(x => x.Username).IsRequired();
            e.Property(x => x.Email).IsRequired();
            e.Property(x => x.Password).IsRequired();
            e.Property(x => x.UserRole).IsRequired();
        });
    }
    
}