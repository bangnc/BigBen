using BangKaDomain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BangKaData.DBContext
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2); // Độ chính xác 18 và quy mô 2


        }
    }
}
