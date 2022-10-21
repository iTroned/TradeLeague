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
        public async Task<User> getUserByID(string id)
        {
            return await _db.getUserByID(Guid.Parse(id));
        }
        //returns a user by a given username
        public async Task<User> getUserByUsername(string username)
        {
            return await _db.getUserByUsername(username);
        }
        //returning users with their ids is dangerous but doesnt matter when not using a login system
        //returns all registered users
        public async Task<List<User>> getAllUsers()
        {
            return await _db.getAllUsers();
        }
        //update a user, does not work for admin
        public async Task<bool> updateUser(User editUser)
        {
            if(editUser.id == (await getUserByUsername(adminName)).id)
            {
                return false;
            }
            return await _db.updateUser(editUser);
        }
        //creates a new user with a given name
        public async Task<bool> createUser(string username)
        {
            bool created = await _db.createUser(username);
            if (created)
            {
                User user = await _db.getUserByUsername(username);
                setCurrentUser(user.id.ToString());
            }
            return created;
        }
        //checks if username is taken | WIP when creating user and changing name | client side only
        public async Task<bool> checkUsername(string username)
        {
            return await _db.checkUsername(username);
        }
        //creates a new company
        public async Task<bool> createCompany(string name)
        {
            return await _db.createCompany(name);
        }
        //gets a single company by its id
        public async Task<Company> getCompanyByID(string id)
        {
            return await _db.getCompanyByID(Guid.Parse(id));
        }
        //returns a list of all companies registeres
        public async Task<List<Company>> getAllCompanies()
        {
            return await _db.getAllCompanies();
        }
        //deletets the user in the session. cannot delete admin
        public async Task<bool> deleteUser()
        {
            if((await getCurrentUser()).username != adminName){
                bool deleted = await _db.deleteUser(await getCurrentUserID());
                if (deleted)
                {
                    removeCurrentUser();
                }
                return deleted;
            }
            return false;
        }
        //not yet implemented
        public async Task<bool> deleteCompany(string id)
        {
            return false;
            //return await _db.deleteCompany(id);
        }

        private const string SessionKeyCompany = "_currentCompany";
        //sets a new company in session
        public async Task<bool> setCurrentCompany(string id)
        {
            HttpContext.Session.SetString(SessionKeyCompany, id);
            //return await getCurrentCompany() != null;
            return true;
        }
        //gets the company id saved in session
        public string getCurrentCompanyID()
        {
            return HttpContext.Session.GetString(SessionKeyCompany);
        }
        //gets company linked to id in session
        public async Task<Company> getCurrentCompany()
        {
            return await _db.getCompanyByID(Guid.Parse(getCurrentCompanyID()));
        }
        //clears 
        public void removeCurrentCompany()
        {
            HttpContext.Session.SetString(SessionKeyCompany, "");
        }
        private const string SessionKeyUser = "_currentUser";
        //sets a new user to the session
        public bool setCurrentUser(string id)
        {
            HttpContext.Session.SetString(SessionKeyUser, id);
            return true;
        }
        //returns current user id, after setting  a standard in some cases
        public async Task<string> getCurrentUserID()

        {
            string cur = HttpContext.Session.GetString(SessionKeyUser);
            if (cur != null && cur != "")
            {
                return cur;
            }
            //If current user is not set, set it as admin
            else
            {
                User admin = await _db.getUserByUsername(adminName);
                setCurrentUser(admin.id.ToString());
                return admin.id.ToString();
            }
        }
        //returns current user
        public async Task<User> getCurrentUser()
        {
            return await _db.getUserByID(Guid.Parse(await getCurrentUserID()));
        }
        //returns all stocks for certain user
        public async Task<List<StockName>> getStocksForUser(string id)
        {
            List<Stock> dbList = await _db.getStocksWithUserID(await getCurrentUserID());
            List<StockName> stockList = new List<StockName>();
            foreach(Stock stock in dbList)
            {
                stockList.Add(new StockName(stock.companyName, stock.amount));
            }
            return stockList;
        }
        //clears
        public void removeCurrentUser()
        {
            HttpContext.Session.SetString(SessionKeyUser, "");
        }
        //custom session | not used
        public void setCustomSession(string sessionName, string value)
        {
            HttpContext.Session.SetString(sessionName, value);
        }
        public string getCustomSession(string sessionName)
        {
            return HttpContext.Session.GetString(sessionName);
        }

        //tries to buy stock for user and company in session
        public async Task<bool> buyStock(int amount)
        {
            return await _db.tryToBuyStockForUser(await getCurrentUserID(), getCurrentCompanyID(), amount);
        }
        //tries to sell stock for user and company in session
        public async Task<bool> sellStock(int amount)
        {
            return await _db.tryToSellStockForUser(await getCurrentUserID(), getCurrentCompanyID(), amount);
        }
        //gets the current stock from user and company in session
        public async Task<Stock> getCurrentStock()
        {
            return await _db.getStockWithUserAndCompany(await getCurrentUserID(), getCurrentCompanyID());
        }
        //returns amount of shares from current stock
        public async Task<int> getCurrentStockAmount()
        {
            Stock stock = await getCurrentStock();
            if(stock == null)
            {
                return 0;
            }
            return stock.amount;
        }
        public async Task updateValues()
        {
            await _db.updateValues();
        }

    }
}
