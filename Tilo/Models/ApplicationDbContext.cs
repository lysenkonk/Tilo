using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace Tilo.Models
{
    public class ApplicationDbContext : DbContext  
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {
            //Database.EnsureCreated();
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<FileModel> FileModels { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Product>().HasIndex(p => p.Name);
            modelBuilder.Entity<Product>().HasIndex(p => p.Price);

            modelBuilder.Entity<Category>().HasIndex(p => p.Name);
            modelBuilder.Entity<Product>().HasIndex(p => p.Description);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .IsRequired(false);
        }
    }
}
