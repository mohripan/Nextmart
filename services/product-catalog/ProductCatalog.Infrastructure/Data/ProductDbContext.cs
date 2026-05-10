using Microsoft.EntityFrameworkCore;
using ProductCatalog.Models.Entities;

namespace ProductCatalog.Infrastructure.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
}