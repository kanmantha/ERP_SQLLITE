using Microsoft.EntityFrameworkCore;
using ERP.Web.Models;

namespace ERP.Web.Data {
  public class ERPDbContext : DbContext
  {
    public ERPDbContext(DbContextOptions<ERPDbContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      // Simple seed data for quick start
      modelBuilder.Entity<Customer>().HasData(
        new Customer { Id = 1, Name = "Acme Corp", Email = "contact@acme.com", Phone = "555-0100" },
        new Customer { Id = 2, Name = "Globex Ltd", Email = "sales@globex.com", Phone = "555-0101" }
      );
      modelBuilder.Entity<Product>().HasData(
        new Product { Id = 1, Name = "Widget", Description = "Basic widget", Price = 9.99m, Stock = 100 },
        new Product { Id = 2, Name = "Gadget", Description = "Advanced gadget", Price = 19.99m, Stock = 50 }
      );
    }
  }
}
