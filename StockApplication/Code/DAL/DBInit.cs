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

                Random random = new Random();
                var admin = new User(Guid.NewGuid(), "admin", 100000);
                var luddeBassen = new User(Guid.NewGuid(), "Luddebassen", 100000);
                var itroned = new User(Guid.NewGuid(), "iTroned", 100000);

                context.UserSet.Add(admin);
                context.UserSet.Add(luddeBassen);
                context.UserSet.Add(itroned);

                string[] names = { "Random Inc.", "Organic Chemistry Inc.", "Starbux Inc.", "Techhex Inc.", "Tweeter", "JSon", "MeatA", "Betatech", "Mathworkz", "Metaverse", "Harrison Trumpets", "Out of Ideas Inc.", "Wowverse", "Nightfall", "Warhorse" };
                foreach (string name in names)
                {
                    float value = random.Next(1, 100);
                    Company company = new Company(Guid.NewGuid(), name, value, JsonConvert.SerializeObject(randomArray(value)));
                    context.CompanySet.Add(company);
                }
             
                context.SaveChanges();
            }
           
        }
        public static float[] randomArray(float start)
        {
            Random random = new Random();
            float[] randomArray = new float[10];
            float curval = start;
            for (int i = 0; i < randomArray.Length - 1; ++i)
            {
                curval = (curval * (random.Next(8000, 12001)) / 10000F);
                randomArray[i] = curval;
            }
            randomArray[randomArray.Length - 1] = start;

            return randomArray;
        }
    }
}
