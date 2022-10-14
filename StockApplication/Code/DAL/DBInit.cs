using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace StockApplication.Code.DAL
{
    public class DBInit
    {
        public static void Initialize(IApplicationBuilder app)
        {
            using(var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<StockContext>();
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var admin = new User(Guid.NewGuid(), "admin", null, 99999999999);
                var luddeBassen = new User(Guid.NewGuid(), "Luddebassen", null, 100000);
                var itroned = new User(Guid.NewGuid(), "iTroned", null, 100000);

                var company1 = new Company(Guid.NewGuid(), "Random Inc.", 69.69F);
                var company2 = new Company(Guid.NewGuid(), "Organic Chemistry Inc.", 42.2F);

                context.Users.Add(admin);
                context.Users.Add(luddeBassen);
                context.Users.Add(itroned);

                context.Companies.Add(company1);
                context.Companies.Add(company2);
                context.SaveChanges();
            }
           
        }
    }
}
