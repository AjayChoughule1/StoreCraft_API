using Microsoft.EntityFrameworkCore;
using StoreCraft_API.Models;
using System.Collections.Generic;

namespace StoreCraft_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductLog> ProductLogs { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Electronic items" },
                new Category { Id = 2, Name = "Clothing", Description = "Fashion items" },
                new Category { Id = 3, Name = "Books", Description = "Books and magazines" }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
