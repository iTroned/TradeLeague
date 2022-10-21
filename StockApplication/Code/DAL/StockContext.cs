using Microsoft.EntityFrameworkCore;

namespace StockApplication.Code.DAL
{
    public class StockContext : DbContext
    {
        public StockContext(DbContextOptions<StockContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> UserSet { get; set; }
        public DbSet<Company> CompanySet { get; set; }
        public DbSet<Stock> StockSet { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseLazyLoadingProxies();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stock>()
                .HasKey(ue => new { ue.Userid, ue.Companyid });

            modelBuilder.Entity<Stock>()
                .HasOne(ue => ue.User)
                .WithMany(u => u.stocks)
                .HasForeignKey(ue => ue.Userid);
            modelBuilder.Entity<Stock>()
                .HasOne(ue => ue.Company)
                .WithMany(u => u.stocks)
                .HasForeignKey(ue => ue.Companyid);



        }
    }
}
