using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Models;
namespace Shahbazi.Store.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);

        modelBuilder.Entity<CartItem>().Property(c => c.UnitPrice).HasPrecision(18, 2);

        modelBuilder.Entity<OrderItem>().Property(o => o.UnitPrice).HasPrecision(18, 2);

        // PhoneNumber is the UserName: it must be unique (and have a max length, SQL Server cannot index nvarchar(max))
        modelBuilder.Entity<User>().Property(u => u.PhoneNumber).HasMaxLength(20);

        modelBuilder.Entity<User>().HasIndex(u => u.PhoneNumber).IsUnique();
        modelBuilder.Entity<Product>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(p => p.SellerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}