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

                var admin = new User(Guid.NewGuid(), "admin", 100000);
                var luddeBassen = new User(Guid.NewGuid(), "Luddebassen", 100000);
                var itroned = new User(Guid.NewGuid(), "iTroned", 100000);


                var company1 = new Company(Guid.NewGuid(), "Random Inc.", 53.72F, JsonConvert.SerializeObject(randomArray(69.69)));
                var company2 = new Company(Guid.NewGuid(), "Organic Chemistry Inc.", 42.2F, JsonConvert.SerializeObject(randomArray(42.2)));
                var company3 = new Company(Guid.NewGuid(), "Starbux Inc.", 12.27F, JsonConvert.SerializeObject(randomArray(12.27)));
                var company4 = new Company(Guid.NewGuid(), "TechHex Inc.", 93.03F, JsonConvert.SerializeObject(randomArray(93.03)));
                var company5 = new Company(Guid.NewGuid(), "Tweeter", 117.2F, JsonConvert.SerializeObject(randomArray(117.2)));
                var company6 = new Company(Guid.NewGuid(), "JSon", 6.38F, JsonConvert.SerializeObject(randomArray(6.38)));
                var company7 = new Company(Guid.NewGuid(), "MeatA", 9.05F, JsonConvert.SerializeObject(randomArray(9.05)));
                var company8 = new Company(Guid.NewGuid(), "Betatech", 14.42F, JsonConvert.SerializeObject(randomArray(14.42)));

                context.UserSet.Add(admin);
                context.UserSet.Add(luddeBassen);
                context.UserSet.Add(itroned);

                context.CompanySet.Add(company1);
                context.CompanySet.Add(company2);
                context.CompanySet.Add(company3);
                context.CompanySet.Add(company4);
                context.CompanySet.Add(company5);
                context.CompanySet.Add(company6);
                context.CompanySet.Add(company7);
                context.CompanySet.Add(company8);
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
