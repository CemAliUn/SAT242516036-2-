namespace DbContexts;

using Microsoft.EntityFrameworkCore;
using YourProjectNamespace.Models;

public class MyDbModel_DbContext(DbContextOptions<MyDbModel_DbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<LoyaltyPoint> LoyaltyPoints { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }

}