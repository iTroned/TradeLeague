using Microsoft.AspNetCore.Mvc;
using StockApplication.Code.Handlers;
using System;
using StockApplication.Code.DAL;
using Microsoft.EntityFrameworkCore;
using StockApplication.Code;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace StockApplication.Controllers
{
    [Route("[controller]/[action]")]
    public class StockController : ControllerBase
    {
        private const string adminName = "admin";
        private readonly IStockRepository _db;
        private readonly IHostedService listenerService;
        public StockController(IStockRepository db, IHostedService listenerService)
        {
            _db = db;
            this.listenerService = listenerService;
        }
        //returns a user by its id
        public async Task<User> GetUserByID(string id)
        {
            return await _db.GetUserByID(Guid.Parse(id));
        }

        //returns a user by a given username
        public async Task<User> GetUserByUsername(string username)
        {
            return await _db.GetUserByUsername(username);
        }
        //returning users with their ids is dangerous but doesnt matter when not using a login system
        //returns all registered users
        public async Task<List<User>> GetAllUsers()
        {
            return await _db.GetAllUsers();
        }

        //update a user, does not work for admin
        public async Task<bool> UpdateUser(User editUser)
        {
            if(editUser.id == (await GetUserByUsername(adminName)).id)
            {
                return false;
            }
            return await _db.UpdateUser(editUser);
        }

        //creates a new user with a given name
        public async Task<bool> CreateUser(string username, string password)
        {
            bool created = await _db.CreateUser(username, password);
            if (created)
            {
                User user = await _db.GetUserByUsername(username);
                SetCurrentUser(user.id.ToString());
            }
            return created;
        }

        //checks if username is taken | WIP when creating user and changing name | client side only
        public async Task<bool> CheckUsername(string username)
        {
            return await _db.CheckUsername(username);
        }

        //creates a new company
        public async Task<bool> CreateCompany(string name)
        {
            return await _db.CreateCompany(name);
        }

        //gets a single company by its id
        public async Task<Company> GetCompanyByID(string id)
        {
            return await _db.GetCompanyByID(Guid.Parse(id));
        }

        //returns a list of all companies registeres
        public async Task<List<Company>> GetAllCompanies()
        {
            return await _db.GetAllCompanies();
        }

        //deletets the user in the session. cannot delete admin
        public async Task<bool> DeleteUser()
        {
            if((await GetCurrentUser()).username != adminName){
                bool deleted = await _db.DeleteUser(await GetCurrentUserID());
                if (deleted)
                {
                    RemoveCurrentUser();
                }
                return deleted;
            }
            return false;
        }
        
        public async Task<bool> DeleteCompany(string id) //not yet implemented
        {
            return false;
            //return await _db.DeleteCompany(id);
        }

        private const string SessionKeyCompany = "_currentCompany";
        //sets a new company in session
        public async Task<bool> SetCurrentCompany(string id)
        {
            HttpContext.Session.SetString(SessionKeyCompany, id);
            //return await GetCurrentCompany() != null;
            return true;
        }

        //gets the company id saved in session
        public string GetCurrentCompanyID()
        {
            return HttpContext.Session.GetString(SessionKeyCompany);
        }

        //gets company linked to id in session
        public async Task<Company> GetCurrentCompany()
        {
            return await _db.GetCompanyByID(Guid.Parse(GetCurrentCompanyID()));
        }

        //clears 
        public void RemoveCurrentCompany()
        {
            HttpContext.Session.SetString(SessionKeyCompany, "");
        }
        private const string SessionKeyUser = "_currentUser";
        
        //sets a new user to the session
        public bool SetCurrentUser(string id)
        {
            HttpContext.Session.SetString(SessionKeyUser, id);
            return true;
        }

        //returns current user id, after setting  a standard in some cases
        public async Task<string> GetCurrentUserID()

        {
            string cur = HttpContext.Session.GetString(SessionKeyUser);
            if (cur != null && cur != "")
            {
                return cur;
            }
            //If current user is not set, set it as admin
            else
            {
                User admin = await _db.GetUserByUsername(adminName);
                SetCurrentUser(admin.id.ToString());
                return admin.id.ToString();
            }
        }

        //returns current user
        public async Task<User> GetCurrentUser()
        {
            return await _db.GetUserByID(Guid.Parse(await GetCurrentUserID()));
        }

        //removes current user
        public void RemoveCurrentUser()
        {
            HttpContext.Session.SetString(SessionKeyUser, "");
        }
        private const string SessionKeyLoggedIn = "_loggedIn";

        //sets a new user to the session
        private void setLoggedInStatus(bool status)
        {
            int val;
            if (status)
            {
                val = 1;
            }
            else
            {
                val = 0;
            }
            HttpContext.Session.SetInt32(SessionKeyLoggedIn, val);
        }

        public bool getLoggedInStatus()
        {
            try
            {
                int val = (int)HttpContext.Session.GetInt32(SessionKeyLoggedIn);
                if(val == 1)
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

        public async Task<bool> LogIn(string username, string password)
        {
            return await _db.LogIn(username, password);
        }
        //custom session | not used
        public void SetCustomSession(string sessionName, string value)
        {
            HttpContext.Session.SetString(sessionName, value);
        }
        public string GetCustomSession(string sessionName)
        {
            return HttpContext.Session.GetString(sessionName);
        }

        //returns list of stocks certain user own
        public async Task<List<StockName>> GetStocksForUser(string id)
        {
            List<Stock> dbList = await _db.GetStocksWithUserID(await GetCurrentUserID());
            List<StockName> stockList = new List<StockName>();
            foreach(Stock stock in dbList)
            {
                stockList.Add(new StockName(stock.companyName, stock.amount, await _db.GetStockValue(stock)));
            }
            return stockList;
        }

        //returns list with total value for every user
        public async Task<List<StockName>> GetUsersValue() 
        {
            return await _db.GetAllUsersTotalValue();
        }

        //get StockName object with specific user's value
        public async Task<StockName> getUsersValueByID(String id)
        {
            return await _db.GetUsersValueByID(id);
        }

       
        

        //tries to buy stock for user and company in session
        public async Task<bool> BuyStock(int amount)
        {
            return await _db.TryToBuyStockForUser(await GetCurrentUserID(), GetCurrentCompanyID(), amount);
        }

        //tries to sell stock for user and company in session
        public async Task<bool> SellStock(int amount)
        {
            return await _db.TryToSellStockForUser(await GetCurrentUserID(), GetCurrentCompanyID(), amount);
        }

        //gets the current stock from user and company in session
        public async Task<Stock> GetCurrentStock()
        {
            return await _db.GetStockWithUserAndCompany(await GetCurrentUserID(), GetCurrentCompanyID());
        }

        //returns amount of shares owned for current user at current company
        public async Task<int> GetCurrentStockAmount()
        {
            Stock stock = await GetCurrentStock();
            if(stock == null)
            {
                return 0;
            }
            return stock.amount;
        }

        //returns current balance for current user
        public async Task<float> GetBalance()
        {
            return (await GetCurrentUser()).balance;
        }

    }
}
