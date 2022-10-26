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
        private float startBalance = 1000.0F;
        private float startValue = 10.0F;
        private Random random;
        private string startValues;
        public StockRepository(StockContext db)
        {
            _db = db;
            random = new Random();
        }
        public async Task<bool> tryToBuyStockForUser(string userID, string companyID, int amount)
        {
            try
            {
                if(amount <= 0)
                {
                    return false;
                }
                User user = await getUserByID(Guid.Parse(userID));
                Company company = await getCompanyByID(Guid.Parse(companyID)); 
                //System.Diagnostics.Debug.WriteLine(user.username + " is buying " + amount + " of " + company.name);
                float totalPrice = company.value * amount;
                if(!(user.balance > totalPrice))
                {
                    return false;
                }
                if(!(await removeBalanceFromUser(user, totalPrice)))
                {
                    return false;
                }
                if(!(await addStockToUser(user, company, amount)))
                {
                    return false;
                }
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> tryToSellStockForUser(string userID, string companyID, int amount)
        {
            try
            {
                if (amount <= 0)
                {
                    return false;
                }
                User user = await getUserByID(Guid.Parse(userID));
                Company company = await getCompanyByID(Guid.Parse(companyID));
                float totalPrice = company.value * amount;
                Stock stock = await getStockWithUserAndCompany(user, company);
                if(stock == null || stock.amount < amount)
                {
                    return false;
                }
                if(!(await removeStockFromUser(stock, amount)))
                {
                    return false;
                }
                addBalanceToUser(user, totalPrice);
                await _db.SaveChangesAsync();
                return true;
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
                User user = new User(id, username, startBalance);
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
            User[] users = await _db.UserSet.Where(p => p.username.ToLower() == username.ToLower()).ToArrayAsync();
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
                float[] startArray = new float[10];
                Company company = new Company(id, name, startValue, startValues);
                for (int i = 0; i < startArray.Length -1; ++i)
                {
                    startArray[i] = (company.value * (random.Next(800, 1200)) / 1000);
                }
                startArray[startArray.Length - 1] = company.value;
                company.values = JsonConvert.SerializeObject(startArray);
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
                value = (float) Math.Round(value * 100f) / 100f;
                company.value = value;
                float[] arr = JsonConvert.DeserializeObject<float[]>(company.values);
                //System.Diagnostics.Debug.WriteLine("Length: " + arr.Length);
                for (int i = 0; i < arr.Length - 1; i++)
                {
                    //System.Diagnostics.Debug.WriteLine(arr[i] + "/" + arr[i + 1]);
                    arr[i] = arr[i + 1];
                }
                arr[arr.Length - 1] = value;
                company.values = JsonConvert.SerializeObject(arr);
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

        public void setBalanceForUser(User user, float balance)
        {
            user.balance = balance;
        }
        public async Task<Stock> getStockByID(Guid id)
        {
            return await _db.StockSet.FindAsync(id);
        }
        public async Task<List<Stock>> getAllStocks()
        {
            try
            {
                return await _db.StockSet.Select(u => u.clone()).ToListAsync();
            }
            catch
            {
                return null;
            }
        }
        public async Task<bool> deleteStock(Stock stock, bool save)
        {
            try
            {
                _db.StockSet.Remove(stock);
                if (save)
                {
                    await _db.SaveChangesAsync();
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void addBalanceToUser(User user, float value)
        {
            user.balance += value;
        }
        public async Task<bool> removeBalanceFromUser(User user, float value)
        {
            try
            {
                float currentBalance = await getBalanceForUser(user.id);
                if (currentBalance >= value)
                {
                    setBalanceForUser(user, currentBalance - value);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
            
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
                    if (stock == null)
                    {
                        return false;
                    }
                    stock.amount += amount;
                    //await _db.SaveChangesAsync();
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
                Stock stock = new Stock(Guid.NewGuid(), amount, user.id, company.id, company.name);
                _db.StockSet.Add(stock);
                //System.Diagnostics.Debug.WriteLine("Creating stock " + stock.id);
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> removeStockFromUser(Stock stock, int amount)
        {
            try
            {
                //Stock stock = await getStockWithUserAndCompany(user, company);
                if (stock == null || stock.amount < amount)
                {
                    return false;
                }
                
                int newAmount = stock.amount - amount;
                if (newAmount == 0)
                {
                    return await deleteStock(stock, false);
                }
                stock.amount = newAmount;
                //await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }

        }
        public async Task<List<Stock>> getStocksWithUserID(Guid id)
        {
            try
            {
                return await _db.StockSet.Where(p => p.Userid == id).ToListAsync();
            }
            catch
            {
                return null;
            }
            
        }
        public async Task<List<Stock>> getStocksWithUserID(string id)
        {
            return await getStocksWithUserID(Guid.Parse(id));
        }
        public async Task<List<Stock>> getStocksWithUser(User user)
        {
            return await getStocksWithUserID(user.id);
        }
        public async Task<List<Stock>> getStocksWithCompany(Company company)
        {
            try
            {
                return await _db.StockSet.Where(p => p.Companyid == company.id).ToListAsync();
            }
            catch
            {
                return null;
            }
            
        }
        public async Task<Stock> getStockWithUserAndCompany(Guid uid, Guid cid)
        {
            try
            {
                Stock[] arr =  await _db.StockSet.Where(p => p.Companyid == cid && p.Userid == uid).ToArrayAsync();
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
        public async Task<Stock> getStockWithUserAndCompany(User user, Company company)
        {
            return await getStockWithUserAndCompany(user.id, company.id);
        }
        public async Task<Stock> getStockWithUserAndCompany(string uid, string cid)
        {
            return await getStockWithUserAndCompany(Guid.Parse(uid), Guid.Parse(cid));
        }
        public async Task<bool> userHasStocksWithCompany(User user, Company company)
        {
            return await getStockWithUserAndCompany(user, company) != null;
        }
        public async Task updateValues()
        {
            foreach (Company company in await getAllCompanies())
            {
                float multiplier = (random.Next(8000, 12001)) / 10000F;
         
                await updateValueOnCompany(company.id, company.value * multiplier);
            }
           
        }
        public async Task<float> getUsersTotalValue(String id)
        {
            float totalValue = 0;
            List<Stock> dbList = await getStocksWithUserID(id); //creating a list with all stocks
            foreach(Stock stock in dbList) //iterating through list and adding value to totalValue
            {
                Company company = await getCompanyByID(stock.Companyid);
                totalValue += (stock.amount* company.value);
            }
            totalValue += await getBalanceForUser(Guid.Parse(id)); //adding user's balance at the end and returning totalvalue

            return totalValue;
        }



    }
}
