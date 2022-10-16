using Microsoft.EntityFrameworkCore;
using StockApplication.Code.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;



namespace StockApplication.Code.Handlers
{
    public class StockRepository : IStockRepository
    {
        private readonly StockContext _db;
        private float startBalance = 100.0F;
        private float startValue = 10.0F;
        private Random random;
        private string startValues;
        
        public StockRepository(StockContext db)
        {
            _db = db;

            random = new Random();
            Timer _timer = new Timer(async (e) =>
            {
                //await updateValues();
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }
        public async Task<User> getUserByID(Guid id)
        {
            User user = await _db.Users.FindAsync(id);
            return user;
        }
        public async Task<User> getUserByUsername(string username)
        {
            User[] users = await _db.Users.Where(p => p.username == username).ToArrayAsync();
            if (users.Length == 1)
            {
                return users[0];
            }
            return null;
        }
        public async Task<List<User>> getAllUsers()
        {
            try
            {
                return await _db.Users.Select(u => u.clone()).ToListAsync();
            }
            catch
            {
                return null;
            }
        }
        public async Task<bool> updateUser(User editUser)
        {
            try
            {
                User user = await _db.Users.FindAsync(editUser.id);
                if (user.username != editUser.username)
                {
                    if (!(await checkUsername(editUser.username)))
                    {
                        return false;
                    }
                    else
                    {
                        user.username = editUser.username;
                    }
                }
                else
                {
                    user.username = editUser.username;
                }
                user.balance = editUser.balance;
                user.ownedStocks = editUser.ownedStocks;
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> createUser(string username)
        {
            try
            {
                if (!(await checkUsername(username)) || username == null || username.Equals(""))
                {
                    Console.WriteLine("Username taken");
                    return false;
                }
                Guid id = Guid.NewGuid();
                User user = new User(id, username, null, startBalance);
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }


        }
        public async Task<bool> deleteUser(string id)
        {
            try
            {
                _db.Remove(await getUserByID(Guid.Parse(id)));
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> checkUsername(string username)
        {
            User[] users = await _db.Users.Where(p => p.username == username).ToArrayAsync();
            if (users.Length != 0)
            {
                return false;
            }
            return true;
        }
        public Stock getStockByID(Guid id)
        {
            return new Stock();
        }
        public bool updateStock(Guid id, Stock stock)
        {
            return true;
        }
        public Guid createStock(Stock stock)
        {
            Guid id = Guid.NewGuid();
            return id;
        }
        public async Task<Company> getCompanyByID(Guid id)
        {
            Company company = await _db.Companies.FindAsync(id);
            return company;
        }
        /*public async Task<bool> updateCompany(Company editCompany)
        {
            try
            {
                Company company = await _db.Companies.FindAsync(editCompany.id);
                
                company.value = editUser.balance;
                user.ownedStocks = editUser.ownedStocks;
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }*/
        public async Task<bool> createCompany(string name)
        {
            try
            {
                Guid id = Guid.NewGuid();
                float[] startArray = new float[10];
                Company company = new Company(id, name, startValue, startValues);
                for(int i = 0; i < startArray.Length - 1; ++i)
                {
                    startArray[i] = (company.value * (random.Next(800, 1200)) / 1000);
                }
                startArray = startArray.Append(company.value).ToArray();
                company.values = JsonConvert.SerializeObject(startArray);
                _db.Companies.Add(company);
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> updateValueOnCompany(Guid id, float value)
        {
            try
            {
                Company company = await _db.Companies.FindAsync(id);
                company.value = value;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> deleteCompany(string id)
        {
            try
            {
                _db.Remove(await getCompanyByID(Guid.Parse(id)));
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<List<Company>> getAllCompanies()
        {
            try
            {
                return await _db.Companies.Select(u => u.clone()).ToListAsync();
            }
            catch
            {
                return null;
            }
        }
        public float getBalanceForUser(User user)
        {
            return 0;
        }

        public bool setBalanceForUser(User user, float balance)
        {
            return true;
        }
        public bool removeBalanceFromUser(User user, float value)
        {
            float currentBalance = getBalanceForUser(user);
            if (currentBalance >= value)
            {
                setBalanceForUser(user, currentBalance - value);
                return true;
            }
            return false;
        }
        public bool removeStockFromUser(User user, Stock stock, int amount)
        {
            return false;
        }
        public async Task<bool> updateValues()
        {
            try
            {
                foreach (Company company in await _db.Companies.Select(u => u.clone()).ToListAsync())
                {
                    if (random.Next(2) == 0)
                    {
                        await updateValueOnCompany(company.id, (float)(company.value * (random.Next(800, 1200)) / 1000));
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        
    }
}
