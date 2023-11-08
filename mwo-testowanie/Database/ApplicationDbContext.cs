using Microsoft.EntityFrameworkCore;
using mwo_testowanie.Models;

namespace mwo_testowanie.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<Client> Clients { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<ProductQuantity> ProductQuantities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>()
            .HasIndex(c => c.Email)
            .IsUnique();

        modelBuilder.Entity<Order>()
            .HasMany<Product>()
            .WithMany()
            .UsingEntity<ProductQuantity>();
    }
}