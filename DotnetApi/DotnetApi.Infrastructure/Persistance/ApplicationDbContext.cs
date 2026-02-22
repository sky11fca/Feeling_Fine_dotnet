using DotnetApi.Domains.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Infrastructure.Persistance;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<Review> Reviews => Set<Review>();

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
            e.Property(x => x.SubmitedOn).IsRequired();
            e.Property(x => x.CreatedAt).IsRequired();
        });
    }
    
}