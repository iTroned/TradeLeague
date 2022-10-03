using Microsoft.EntityFrameworkCore;
using StockApplication.Code.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApplication.Code.Handlers
{
    public class StockRepository : IStockRepository
    {
        private readonly StockContext _db;
        private float startBalance = 100.0F;
        public StockRepository(StockContext db)
        {
            _db = db;
        }
        public async Task<User> getUserByID(Guid id)
        {
            User user = await _db.Users.FindAsync(id);
            return user;
        }
        public async Task<User> getUserByUsername(string username)
        {
            User[] users = await _db.Users.Where(p => p.username == username).ToArrayAsync();
            if(users.Length == 1)
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
        public async Task<bool> updateUser(Guid id, string username)
        {
            try
            {
                if (!(await checkUsername(username)))
                {
                    return false;
                }
                User user = await _db.Users.FindAsync(id);
                user.username = username;
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
                if(!(await checkUsername(username)) || username == null || username.Equals(""))
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
        public Stock getCompanyByID(Guid id)
        {
            return new Stock();
        }
        public bool updateCompany(Guid id, Company company)
        {
            return true;
        }
        public Guid createCompany(Company company)
        {
            Guid id = Guid.NewGuid();
            return id;
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
            if(currentBalance >= value)
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
    }
}
