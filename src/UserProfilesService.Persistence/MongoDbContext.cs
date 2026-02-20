
using Contracts;
using Documents;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.ValueGeneration;

namespace Persistence;

public class MongoDbContext: DbContext
{
    public MongoDbContext(DbContextOptions<MongoDbContext> options): base(options){}

    public DbSet<ProfileDocument> Documents {get; init;}
    public DbSet<CharacterDocument> Characters {get; init;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<ProfileDocument>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<StringObjectIdValueGenerator>();
    }

}
