using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KPayBillApi.Web.Data.Entities;

namespace KPayBillApi.Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        
        public DbSet<Company> Companies { get; set; }
        public DbSet<Bill> Bills { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Company>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<Company>().HasIndex(x => x.Cuil).IsUnique();
        }
    }
}