using Microsoft.AspNetCore.Mvc;
using StockApplication.Code.Handlers;
using System;
using StockApplication.Code.DAL;
using Microsoft.EntityFrameworkCore;
using StockApplication.Code;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace StockApplication.Controllers
{
    [Route("[controller]/[action]")]
    public class StockController : ControllerBase
    {
        private const string adminName = "admin";
        private readonly IStockRepository _db;
        public StockController(IStockRepository db)
        {
            _db = db;
        }
        public async Task<User> getUserByID(string id)
        {
            return await _db.getUserByID(Guid.Parse(id));
        }
        public async Task<User> getUserByUsername(string username)
        {
            return await _db.getUserByUsername(username);
        }
        //returning users with their ids is dangerous but doesnt matter when not using a login system
        public async Task<List<User>> getAllUsers()
        {
            return await _db.getAllUsers();
        }
        public async Task<bool> updateUser(User editUser)
        {
            if(editUser.id == (await getUserByUsername(adminName)).id)
            {
                return false;
            }
            return await _db.updateUser(editUser);
        }
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
        public async Task<bool> checkUsername(string username)
        {
            return await _db.checkUsername(username);
        }
        public async Task<bool> createCompany(string name)
        {
            return await _db.createCompany(name);
        }
        public async Task<Company> getCompanyByID(string id)
        {
            return await _db.getCompanyByID(Guid.Parse(id));
        }
        public async Task<List<Company>> getAllCompanies()
        {
            return await _db.getAllCompanies();
        }
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
        public async Task<bool> setCurrentCompany(string id)
        {
            HttpContext.Session.SetString(SessionKeyCompany, id);
            //return await getCurrentCompany() != null;
            return true;
        }

        public string getCurrentCompanyID()
        {
            return HttpContext.Session.GetString(SessionKeyCompany);
        }
        public async Task<Company> getCurrentCompany()
        {
            return await _db.getCompanyByID(Guid.Parse(getCurrentCompanyID()));
        }
        public void removeCurrentCompany()
        {
            HttpContext.Session.SetString(SessionKeyCompany, "");
        }
        private const string SessionKeyUser = "_currentUser";
        public bool setCurrentUser(string id)
        {
            HttpContext.Session.SetString(SessionKeyUser, id);
            return true;
        }

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
        public async Task<User> getCurrentUser()
        {
            return await _db.getUserByID(Guid.Parse(await getCurrentUserID()));
        }
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
        public void removeCurrentUser()
        {
            HttpContext.Session.SetString(SessionKeyUser, "");
        }
        public void setCustomSession(string sessionName, string value)
        {
            HttpContext.Session.SetString(sessionName, value);
        }
        public string getCustomSession(string sessionName)
        {
            return HttpContext.Session.GetString(sessionName);
        }

        public async Task<bool> buyStock(int amount)
        {
            return await _db.tryToBuyStockForUser(await getCurrentUserID(), getCurrentCompanyID(), amount);
        }
        public async Task<bool> sellStock(int amount)
        {
            return await _db.tryToSellStockForUser(await getCurrentUserID(), getCurrentCompanyID(), amount);
        }
        public async Task<Stock> getCurrentStock()
        {
            return await _db.getStockWithUserAndCompany(await getCurrentUserID(), getCurrentCompanyID());
        }
        public async Task<int> getCurrentStockAmount()
        {
            Stock stock = await getCurrentStock();
            if(stock == null)
            {
                return 0;
            }
            return stock.amount;
        }

    }
}
