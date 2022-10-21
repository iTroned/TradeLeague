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
        
    }
}
