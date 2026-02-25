using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.ValueGeneration;
using Persistence.Documents;

namespace Persistence.Contexts;

public class ContentDbContext : DbContext
{
    public ContentDbContext(DbContextOptions<ContentDbContext> options) : base(options) { }

    public DbSet<RaceDocument> Races { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RaceDocument>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<StringObjectIdValueGenerator>();
            
        modelBuilder.Entity<RaceDocument>()
            .HasKey(x => x.Id);
        modelBuilder.Entity<RaceDocument>()
            .HasAlternateKey(x => x.Slug);
    }

}
