using Microsoft.EntityFrameworkCore;

namespace StockApplication.Code.DAL
{
    public class StockContext : DbContext
    {
        public StockContext(DbContextOptions<StockContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        //creating three database-sets, one for each Object (User, company, stock)
        public DbSet<User> UserSet { get; set; }
        public DbSet<Company> CompanySet { get; set; }
        public DbSet<Stock> StockSet { get; set; }

        //creating relationship between object, documentation: https://learn.microsoft.com/en-us/ef/ef6/fundamentals/relationships and https://www.entityframeworktutorial.net/code-first/configure-one-to-many-relationship-in-code-first.aspx
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stock>()
                .HasKey(ue => new { ue.Userid, ue.Companyid }); //Stock-object has two foreign keys

            modelBuilder.Entity<Stock>() //One-to-many relationship: One User can have Many Stocks, stock have foreignkey Userid
                .HasOne(ue => ue.User)
                .WithMany(u => u.stocks)
                .HasForeignKey(ue => ue.Userid);
            modelBuilder.Entity<Stock>() //One-to-many relationship: One Company can have Many Stocks, stock have foreignkey Companyid
                .HasOne(ue => ue.Company)
                .WithMany(u => u.stocks)
                .HasForeignKey(ue => ue.Companyid);



        }
    }
}
