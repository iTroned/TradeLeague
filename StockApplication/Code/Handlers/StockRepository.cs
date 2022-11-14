using Microsoft.EntityFrameworkCore;
using StockApplication.Code.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace StockApplication.Code.Handlers
{
    public class StockRepository : IStockRepository
    //code inspired by lectures and Entity framework documentation: https://learn.microsoft.com/en-us/ef/
    {
        private readonly StockContext _db; //stock context created in Code/DAL/StockContext.cs
        private float startBalance = 1000.0F;
        private float startValue = 10.0F;
        private Random random;
        private string startValues;
        public StockRepository(StockContext db)
        {
            _db = db;
            random = new Random();
        }

        //Buy stock
        public async Task<bool> TryToBuyStockForUser(string userID, string companyID, int amount) 
        {
            try
            {
                if(amount <= 0) //if user try to buy 0 or less, return false
                {
                    return false;
                }
                User user = await GetUserByID(Guid.Parse(userID)); //using userID to get User-entity with getUserByID, GUID.Parse converts from string to GUID
                Company company = await GetCompanyByID(Guid.Parse(companyID)); //using companyID to get Company-entity with getCompanyByID
                
                float totalPrice = company.value * amount; //totalprice = value of share * amount of shares user wants
                if(!(user.balance > totalPrice)) //user dont have enough money
                {
                    return false;
                }
                if(!(await RemoveBalanceFromUser(user, totalPrice))) //if removeBalance function fails return false
                {
                    return false;
                }
                if(!(await AddStockToUser(user, company, amount))) //if no stock was added to user return false
                {
                    return false;
                }
                await _db.SaveChangesAsync(); //saving if everything is ok -> transaction: multiple operations on database, only saving if every operation is successful
                return true;
            }
            catch
            {
                return false; //something went wrong
            }
        }

        //try to sell stock
        public async Task<bool> TryToSellStockForUser(string userID, string companyID, int amount) 
        {
            try
            {
                if (amount <= 0) //if user try to buy 0 or less, return false
                {
                    return false;
                }
                User user = await GetUserByID(Guid.Parse(userID)); //using userID to get User-entity with getUserByID
                Company company = await GetCompanyByID(Guid.Parse(companyID)); //using companyID to get Company-entity with getCompanyByID

                float totalPrice = company.value * amount; ////totalprice = value of share * amount of shares user wants to sell
                Stock stock = await GetStockWithUserAndCompany(user, company); //get stock entity that is connected with User-entity and Company-entity

                if(stock == null || stock.amount < amount) //if stock does not exist, or user try to sell more than user own
                {
                    return false;
                }
                if(!(await RemoveStockFromUser(stock, amount))) //if something went wrong while removing the stock
                {
                    return false;
                }
                AddBalanceToUser(user, totalPrice); //give money to user
                await _db.SaveChangesAsync(); //saving if everything is ok -> transaction: multiple operations on database, only saving if every operation is successful
                return true;
            }
            catch
            {
                return false; //something went wrong
            }
        }

        //get user-entity by id
        public async Task<User> GetUserByID(Guid id)
        {
            return await _db.UserSet.FindAsync(id); //returns entity with given primary key
        }

        //get user-entity by using username (all usernames are unique)
        public async Task<User> GetUserByUsername(string username)
        {
            User[] users = await _db.UserSet.Where(p => p.username == username).ToArrayAsync(); //get array of User-entities with given username
            if (users.Length == 1) //checking if there only exists 1 entity with current username
            {
                return users[0]; //returns user-entity
            }
            return null;
        }

        //get list with all users
        public async Task<List<User>> GetAllUsers()
        {
            try
            {
                return await _db.UserSet.Select(u => u.Clone()).ToListAsync(); //clone all Users saved in UserSet, and converting it to a List
            }
            catch
            {
                return null; //something went wrong
            }
        }

        //update user with new values
        public async Task<bool> UpdateUser(User editUser) //parameter is User-entity with updated values
        {
            try
            {
                User user = await _db.UserSet.FindAsync(editUser.id); //getting a User entity with previous values (ID always stays the same)
                if (user.username != editUser.username) //if username is updated
                {
                    if (!(await CheckUsername(editUser.username))) //if new username is taken, return false
                    {
                        return false;
                    }
                    else
                    {
                        user.username = editUser.username; //updating username
                    }
                }
                else
                {
                    user.username = editUser.username; //if Username for both entities are equal, it stays the same
                }
                user.balance = editUser.balance; //updating balance with new values
                await _db.SaveChangesAsync(); //saving if all operations successful
                return true;
            }
            catch
            {
                return false; //something went wrong
            }
        }

        //create a new user with username
        public async Task<bool> CreateUser(string username, string password)
        {
            try
            {
                if (!(await CheckUsername(username)) || username == null || username.Equals("")) //If username does already exists, is null or equals "", return false
                {
                    return false;
                }
                Guid id = Guid.NewGuid(); //generate a new Guid for user, (primary key)
                User user = new User(id, username, startBalance); //new user entity with id generated, username from input and startBalance configured at line 19
                _db.UserSet.Add(user); //add user to databaseset
                await _db.SaveChangesAsync(); //saving
                return true;
            }
            catch
            {
                return false; //something went wrong
            }
        }

        //delete user with id
        public async Task<bool> DeleteUser(string id)
        {
            try
            {
                _db.UserSet.Remove(await GetUserByID(Guid.Parse(id))); //function getUserByID gives us the user entity that will be removed
                await _db.SaveChangesAsync(); //save changes
                return true;
            }
            catch
            {
                return false; //something went wrong
            }
        }

        //check if username exists
        public async Task<bool> CheckUsername(string username)
        {
            User[] users = await _db.UserSet.Where(p => p.username.ToLower() == username.ToLower()).ToArrayAsync(); //creating array with all entities with current username
            if (users.Length != 0) //if 0 we know username does not exists
            {
                return false; //username exists
            }
            return true; //username does not exists
        }

        //get company-entity with primary key
        public async Task<Company> GetCompanyByID(Guid id)
        {
            Company company = await _db.CompanySet.FindAsync(id); //return company-entity with given primary key values
            return company;
        }

        //UPDATE COMPANY NOT IMPLEMENTED YET, BUT WILL IN THE FUTURE
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

        //create company with name from input
        public async Task<bool> CreateCompany(string name)
        {
            try
            {
                Guid id = Guid.NewGuid(); //generate new GUID
                float[] startArray = new float[10]; //array with history of values
                Company company = new Company(id, name, startValue, startValues); //new company-entity
                for (int i = 0; i < startArray.Length -1; ++i)
                {
                    startArray[i] = (company.value * (random.Next(800, 1200)) / 1000); //fill array with random values (displayed in chart frontend)
                }
                startArray[startArray.Length - 1] = company.value; //last value in array is the current value of company
                company.values = JsonConvert.SerializeObject(startArray); //serialize as JSON, cant save array in database
                _db.CompanySet.Add(company); //add to databaseset
                await _db.SaveChangesAsync(); //save changes
                return true;
            }
            catch
            {
                return false;
            }
        }

        //update value on company
        public async Task<bool> UpdateValueOnCompany(Guid id, float value)
        {
            try
            {
                
                Company company = await _db.CompanySet.FindAsync(id); //finding entity with given primary key
                value = (float) Math.Round(value * 100f) / 100f; //random new value
                company.value = value; //updating company value

                float[] arr = JsonConvert.DeserializeObject<float[]>(company.values); //deserializing json to update array with history of values
                
                for (int i = 0; i < arr.Length - 1; i++)
                {
                    
                    arr[i] = arr[i + 1]; //rotating every value on place to the left, first value gets thrown to the void
                }
                arr[arr.Length - 1] = value; //last value is company value
                company.values = JsonConvert.SerializeObject(arr); //serializing json to store in db
                await _db.SaveChangesAsync(); //saving
                return true;
            }
            catch
            {
                return false;
            }
        }

        //deleting company (not used) yet
        public async Task<bool> DeleteCompany(string id)
        {
            try
            {
                _db.CompanySet.Remove(await GetCompanyByID(Guid.Parse(id))); //get company-entity with given primary key and remove it
                await _db.SaveChangesAsync(); //save changes
                return true;
            }
            catch
            {
                return false;
            }
        }

        //get list of all companies
        public async Task<List<Company>> GetAllCompanies()
        {
            try
            {
                return await _db.CompanySet.Select(u => u.Clone()).ToListAsync(); //clone all entities in databaseset, convert them to list and return it
            }
            catch
            {
                return null;
            }
        }

        //get balance for user
        public async Task<float> GetBalanceForUser(Guid id)
        {
            User user = await GetUserByID(id); //get user-entity from given primary key, return balance
            return user.balance;
        }

        //set balance for user
        public void SetBalanceForUser(User user, float balance) //update user-balance, but no saving -> used by other functions, only save if all operations done is completed (transactions)
        {
            user.balance = balance;
        }

        //get stock entity with primary key
        public async Task<Stock> GetStockByID(Guid id)
        {
            return await _db.StockSet.FindAsync(id);
        }

        //get list of all stocks
        public async Task<List<Stock>> GetAllStocks()
        {
            try
            {
                return await _db.StockSet.Select(u => u.Clone()).ToListAsync(); //cloned all entities in databaseset and convert to list
            }
            catch
            {
                return null;
            }
        }

        //delete stock
        public async Task<bool> DeleteStock(Stock stock, bool save) //bool save is for transactions. If True we get the confirmation that we can save, if false abort.
        {
            try
            {
                _db.StockSet.Remove(stock); //remove stock from stockset
                if (save)
                {
                    await _db.SaveChangesAsync(); //save if ok
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        //add balance to user
        public void AddBalanceToUser(User user, float value) //increase user-balance with float value (not saving because of transactions)
        {
            user.balance += value;
        }

        //remove balance from user
        public async Task<bool> RemoveBalanceFromUser(User user, float value) //decrease user-balance with float value (not saving because of transactions)
        {
            try
            {
                float currentBalance = await GetBalanceForUser(user.id); //current balance for user
                if (currentBalance >= value) //if user have enough balance
                {
                    SetBalanceForUser(user, currentBalance - value); //update balance
                    return true;
                }
                return false; //user does not have enough balance
            }
            catch
            {
                return false;
            }
            
        }

        
        

        //add stock to user-entity
        public async Task<bool> AddStockToUser(User user, Company company, int amount) //amount of shares from a Company the User wants
        {
            try
            {
                if(await UserHasStocksWithCompany(user, company)) //if a stock with this User and this Company exists, we want to use the same entity
                {
                    
                    Stock stock = await GetStockWithUserAndCompany(user, company); //get entity
                    
                    //should never happen, but for safe measures
                    if (stock == null)
                    {
                        return false;
                    }
                    stock.amount += amount; //increasing stock.amount
                    //not saving because of transactions
                    return true;
                }
                return await CreateStock(user, company, amount); //if no stock-entity with this user and this company exists, createStock function
            }
            catch
            {
                return false;
            }
        }

        //create stock for user and company, amount of shares
        public async Task<bool> CreateStock(User user, Company company, int amount)
        {
            try
            {
                Stock stock = new Stock(Guid.NewGuid(), amount, user.id, company.id, company.name); //create new Stock-entity with Guid as primary key, user.id and company.id as foreign keys
                _db.StockSet.Add(stock); //add stock to stockSet
                //System.Diagnostics.Debug.WriteLine("Creating stock " + stock.id);
                await _db.SaveChangesAsync(); //save changes
                return true;
            }
            catch
            {
                return false;
            }
        }

        //remove stock from user
        public async Task<bool> RemoveStockFromUser(Stock stock, int amount) //remove stock entity
        {
            try
            {
                
                if (stock == null || stock.amount < amount) //if amount to be removed is more than stock.amount return false
                {
                    return false;
                }
                
                int newAmount = stock.amount - amount; //new amount of shares
                if (newAmount == 0) //if 0 we need to delete the stock
                {
                    return await DeleteStock(stock, false); //delete stock function 
                }
                stock.amount = newAmount; //if not zero new amount is updated

                //not saved because of transactions : function is called in tryToSellStock and saved if all operations is okay
                return true;
            }
            catch
            {
                return false;
            }

        }

        //get list of stock entities user owns
        public async Task<List<Stock>> GetStocksWithUserID(Guid id)
        {
            try
            {
                return await _db.StockSet.Where(p => p.Userid == id).ToListAsync(); //return list of stocks belonging to this user
            }
            catch
            {
                return null;
            }
            
        }

        //input is string, converting to Guid and calling function to get list of stocks this user own
        public async Task<List<Stock>> GetStocksWithUserID(string id)
        {
            return await GetStocksWithUserID(Guid.Parse(id));
        }

        //input is user-entity, converting to Guid and calling function to get list of stocks this user own
        public async Task<List<Stock>> GetStocksWithUser(User user)
        {
            return await GetStocksWithUserID(user.id);
        }

        //get stock connected to this company-entity
        public async Task<List<Stock>> GetStocksWithCompany(Company company)
        {
            try
            {
                return await _db.StockSet.Where(p => p.Companyid == company.id).ToListAsync(); //list of stocks connected to this company-entity
            }
            catch
            {
                return null;
            }
            
        }

        //get stock connected to this user AND this company
        public async Task<Stock> GetStockWithUserAndCompany(Guid uid, Guid cid)
        {
            try
            {
                Stock[] arr =  await _db.StockSet.Where(p => p.Companyid == cid && p.Userid == uid).ToArrayAsync(); //creating array with all stocks that are connected to this user AND this company
                if(arr.Length != 1) //max 1 stock should be connected to User AND Company
                {
                    return null;
                }
                return arr[0]; //returning stock if exists
            }
            catch
            {
                return null;
            }
        }
        //get stock with entity, convert to Guid and call on function line 505: getStockWithUserAndCompany(Guid uid, Guid cid)
        public async Task<Stock> GetStockWithUserAndCompany(User user, Company company)
        {
            return await GetStockWithUserAndCompany(user.id, company.id);
        }
        //get stock with string, convert to Guid and call on function line 505: getStockWithUserAndCompany(Guid uid, Guid cid)
        public async Task<Stock> GetStockWithUserAndCompany(string uid, string cid)
        {
            return await GetStockWithUserAndCompany(Guid.Parse(uid), Guid.Parse(cid));
        }

        //check if user has stock with a company, return true if true, false if false
        public async Task<bool> UserHasStocksWithCompany(User user, Company company)
        {
            return await GetStockWithUserAndCompany(user, company) != null;
        }

        //update values of company
        public async Task UpdateValues()
        {
            foreach (Company company in await GetAllCompanies()) //get list of all companies, iterate through and update a random value to every company
            {
                float multiplier = (random.Next(8000, 12001)) / 10000F;
         
                await UpdateValueOnCompany(company.id, company.value * multiplier); //update value with new value
            }
           
        }

        //get list of all users with their total value
        public async Task<List<StockName>> GetAllUsersTotalValue()
        {
            
            List<User> userList = await GetAllUsers(); //get list of all users
            List<StockName> stockList = new List<StockName>(); 
            foreach (User user in userList) //for each user, go through all owned stocks and add to the value
            {
                stockList.Add(await GetUsersValueByID(user.id.ToString())); //returning user name with total amount of shares owned and total value
            }
            stockList.Sort(new StockNameComparer());
             return stockList;
        }

        //get total value for this specific user
        public async Task<StockName> GetUsersValueByID(String id)
        {
            float totalValue = 0; //total value 
            int amount = 0; //amount of shares
            User user = await GetUserByID(Guid.Parse(id));
            List<Stock> dbList = await GetStocksWithUserID(id);
            foreach (Stock stock in dbList) //iterating through list and adding value to totalValue
            {
                amount += stock.amount;
                totalValue += (await GetStockValue(stock));
            }
            List<StockName> stockList = new List<StockName>();
            totalValue += user.balance; //adding user's balance at the end and returning totalvalue
            StockName stockName = new StockName(user.username, amount, totalValue);
            return stockName;
        }

        //get value of stock
        public async Task<float> GetStockValue(Stock stock)
        {
            float value = 0;
            Company company = await GetCompanyByID(stock.Companyid); //get company-entity
            value = (stock.amount * company.value); //return value of stock
            return value;
        }
       

        private string HashPassword(string password, string salt)
        {
            byte[] passwordArr = Encoding.UTF8.GetBytes(password);
            byte[] saltArr = Convert.FromBase64String(salt);

            byte[] passwordSalt = new byte[passwordArr.Length + saltArr.Length];

            for (int i = 0; i < passwordArr.Length; i++)
            {
                passwordSalt[i] = passwordArr[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                passwordSalt[passwordArr.Length + i] = saltArr[i];
            }
            byte[] hash = new SHA256Managed().ComputeHash(passwordSalt);
            return Convert.ToBase64String(hash);
        }
        private string GetSalt(int size)
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number.
            return Convert.ToBase64String(buff);
        }

        public async Task<bool> LogIn(string username, string password)
        {
            throw new NotImplementedException();
        }

  
    }
}
