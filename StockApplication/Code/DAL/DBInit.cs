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
                context.Database.EnsureDeleted(); //initializing Database, remove this if you want to keep data for every session
                context.Database.EnsureCreated();

                Random random = new Random();
                //creating 3 standard users
                /*var admin = new User(Guid.NewGuid(), "admin", 100);
                var luddeBassen = new User(Guid.NewGuid(), "Luddebassen", 100);
                var itroned = new User(Guid.NewGuid(), "iTroned", 100);

                //add users to databaseset for users (StockContext)
                context.UserSet.Add(admin);
                context.UserSet.Add(luddeBassen);
                context.UserSet.Add(itroned);*/

                //creating standard companies
                string[] names = { "Random Inc.", "Organic Chemistry Inc.", "Starbux Inc.", "Techhex Inc.", "Tweeter", "JSon", "MeatA", "Betatech", "Mathworkz", "Metaverse", "Harrison Trumpets", "Out of Ideas Inc.", "Wowverse", "Nightfall", "Warhorse" };
                foreach (string name in names)
                {
                    float value = random.Next(1, 100); //giving random value to companies
                    Company company = new Company(Guid.NewGuid(), name, value, JsonConvert.SerializeObject(randomArray(value))); //serializing array because not possible to save array in database
                    context.CompanySet.Add(company);
                }
             
                context.SaveChanges(); //saving all changes !!
            }
           
        }
        //functions to generate a random array of 10 values, the last value is the current value of the company
        public static float[] randomArray(float start)
        {
            Random random = new Random();
            float[] randomArray = new float[10];
            float curval = start;
            for (int i = 0; i < randomArray.Length - 1; ++i)
            {
                curval = (curval * (random.Next(8000, 12001)) / 10000F); //random values
                randomArray[i] = curval;
            }
            randomArray[randomArray.Length - 1] = start;

            return randomArray; //returning random array
        }
    }
}
