using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Newtonsoft.Json;

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


                var company1 = new Company(Guid.NewGuid(), "Random Inc.", 69.69F, JsonConvert.SerializeObject(randomArray(69.69)));
                var company2 = new Company(Guid.NewGuid(), "Organic Chemistry Inc.", 42.2F, JsonConvert.SerializeObject(randomArray(42.2)));

                context.UserSet.Add(admin);
                context.UserSet.Add(luddeBassen);
                context.UserSet.Add(itroned);

                context.CompanySet.Add(company1);
                context.CompanySet.Add(company2);
                context.SaveChanges();
            }
           
        }
        public static double[] randomArray(double start)
        {
            Random random = new Random();
            double[] randomArray = new double[10];
            for (int i = 0; i < randomArray.Length - 1; ++i)
            {
                randomArray[i] = (start * (random.Next(800, 1200)) / 1000);
            }
            randomArray[randomArray.Length - 1] = start;

            return randomArray;
        }
    }
}
