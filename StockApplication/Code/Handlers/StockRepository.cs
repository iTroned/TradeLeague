using Microsoft.EntityFrameworkCore;
using StockApplication.Code.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;



namespace StockApplication.Code.Handlers
{
    public class StockRepository : IStockRepository
    {
        private readonly StockContext _db;
        private float startBalance = 100.0F;
        private float startValue = 10.0F;
        private Random random;
        public StockRepository(StockContext db)
        {
            _db = db;

            random = new Random();
            Timer _timer = new Timer(async (e) =>
            {
                //System.Diagnostics.Debug.WriteLine(DateTime.Now);
                //await updateValues();
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }
        public async Task<bool> tryToBuyStockForUser(string userID, string companyID, int amount)
        {
            try
            {
                User user = await getUserByID(Guid.Parse(userID));
                Company company = await getCompanyByID(Guid.Parse(companyID));
                float totalPrice = company.value * amount;
                if(!(await userHasEnoughBalance(user, totalPrice)))
                {
                    return false;
                }
                if (await addStockToUser(user, company, amount) && await removeBalanceFromUser(user.id, totalPrice))
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public async Task<User> getUserByID(Guid id)
        {
            return await _db.UserSet.FindAsync(id);
        }
        public async Task<User> getUserByUsername(string username)
        {
            User[] users = await _db.UserSet.Where(p => p.username == username).ToArrayAsync();
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
                return await _db.UserSet.Select(u => u.clone()).ToListAsync();
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
                User user = await _db.UserSet.FindAsync(editUser.id);
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
                    return false;
                }
                Guid id = Guid.NewGuid();
                User user = new User(id, username, null, startBalance);
                _db.UserSet.Add(user);
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
                _db.UserSet.Remove(await getUserByID(Guid.Parse(id)));
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
            User[] users = await _db.UserSet.Where(p => p.username == username).ToArrayAsync();
            if (users.Length != 0)
            {
                return false;
            }
            return true;
        }
        public async Task<Company> getCompanyByID(Guid id)
        {
            Company company = await _db.CompanySet.FindAsync(id);
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
                Company company = new Company(id, name, startValue);
                _db.CompanySet.Add(company);
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
                Company company = await _db.CompanySet.FindAsync(id);
                company.value = value;
                await _db.SaveChangesAsync();
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
                return await _db.CompanySet.Select(u => u.clone()).ToListAsync();
            }
            catch
            {
                return null;
            }
        }
        public async Task<float> getBalanceForUser(Guid id)
        {
            User user = await getUserByID(id);
            return user.balance;
        }

        public async Task<bool> setBalanceForUser(Guid id, float balance)
        {
            try
            {
                User user = await getUserByID(id);
                user.balance = balance;
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
            
        }
        public async Task<Stock> getStockByID(Guid id)
        {
            return await _db.StockSet.FindAsync(id);
        }
        public async Task<bool> deleteStock(Guid id)
        {
            try
            {
                _db.StockSet.Remove(await getStockByID(id));
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> removeBalanceFromUser(Guid id, float value)
        {
            float currentBalance = await getBalanceForUser(id);
            if (currentBalance >= value)
            {
                await setBalanceForUser(id, currentBalance - value);
                return true;
            }
            return false;
        }
        public async Task<bool> userHasEnoughBalance(User user, float value)
        {
            return await getBalanceForUser(user.id) >= value;
        }
        public async Task<bool> addStockToUser(User user, Company company, int amount)
        {
            try
            {
                if(await userHasStocksWithCompany(user, company))
                {
                    Stock stock = await getStockWithUserAndCompany(user, company);
                    //should never happen, but for safe measures
                    if(stock == null)
                    {
                        return false;
                    }
                    stock.amount += amount;
                    await _db.SaveChangesAsync();
                    return true;
                }
                return await createStock(user, company, amount);
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> createStock(User user, Company company, int amount)
        {
            try
            {
                Stock stock = new Stock(Guid.NewGuid(), amount, user, company);
                _db.StockSet.Add(stock);
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> removeStockFromUser(User user, Company company, int amount)
        {
            Stock stock = await getStockWithUserAndCompany(user, company);
            if(stock == null || stock.amount < amount)
            {
                return false;
            }
            int newAmount = stock.amount - amount;
            if(newAmount == 0)
            {
                await deleteStock(stock.id);
                return true;
            }
            stock.amount = newAmount;
            await _db.SaveChangesAsync();
            return true;

        }
        public async Task<List<Stock>> getStocksWithUser(User user)
        {
            try
            {
                return await _db.StockSet.Where(p => p.owner == user).ToListAsync();
            }
            catch
            {
                return null;
            }
            
        }
        public async Task<List<Stock>> getStocksWithCompany(Company company)
        {
            try
            {
                return await _db.StockSet.Where(p => p.company == company).ToListAsync();
            }
            catch
            {
                return null;
            }
            
        }
        public async Task<Stock> getStockWithUserAndCompany(User user, Company company)
        {
            try
            {
                Stock[] arr =  await _db.StockSet.Where(p => p.company == company && p.owner == user).ToArrayAsync();
                if(arr.Length != 1)
                {
                    return null;
                }
                return arr[0];
            }
            catch
            {
                return null;
            }
        }
        public async Task<bool> userHasStocksWithCompany(User user, Company company)
        {
            return await getStockWithUserAndCompany(user, company) != null;
        }
        //still under testing
        public async Task<bool> updateValues()
        {
            try
            {
                foreach (Company company in await _db.CompanySet.Select(u => u.clone()).ToListAsync())
                {
                    if (random.Next(2) == 0)
                    {
                        System.Diagnostics.Debug.WriteLine(company.name);
                        //await updateValueOnCompany(company.id, (float)(company.value * (random.Next(800, 1200)) / 1000));
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
