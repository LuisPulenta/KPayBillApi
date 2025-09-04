using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KPayBillApi.Web.Data.Entities;
using Web.Data.Entities;
using Mono.TextTemplating;

namespace KPayBillApi.Web.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Reason> Reasons { get; set; }
        public DbSet<UserCompany> UserCompanies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Company>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<Company>().HasIndex(x => x.Cuil).IsUnique();
            modelBuilder.Entity<Company>().HasIndex(x => x.Email).IsUnique();
            modelBuilder.Entity<Supplier>().HasIndex("Cuil", "CompanyId").IsUnique();
            modelBuilder.Entity<Reason>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<UserCompany>().HasIndex("UserId", "CompanyId").IsUnique();

            modelBuilder.Entity<Bill>()
    .HasIndex(b => new { b.EmitterCompanyId, b.Letra, b.PV, b.Numero })
    .IsUnique();
            modelBuilder.Entity<Bill>().Property(b => b.ImporteNeto).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Bill>().Property(b => b.ImporteIVA).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Bill>().Property(b => b.ImporteTotal).HasColumnType("decimal(18,2)");
        }
    }
}