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
using Microsoft.Extensions.Logging;

namespace StockApplication.Controllers
{
    [Route("[controller]/[action]")]
    public class StockController : ControllerBase
    {
        private readonly IStockRepository _db;
        private readonly IHostedService listenerService;
        private readonly ILogger _logger;
        public StockController(IStockRepository db, IHostedService listenerService, ILogger<StockController> logger)
        {
            _db = db;
            _logger = logger;
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
        //returns all registered users with their usernames and passwords
        public async Task<List<ClientUser>> GetAllUsers()
        {
            return await _db.GetAllClientUsers();
        }

        //update a user, does not work for admin
        public async Task<ServerResponse> UpdateUser(User editUser)
        {
            if (!GetLoggedInStatus())
            {
                return new ServerResponse(false, "Not logged in!");
            }
            return await _db.UpdateUser(editUser);
        }

        //creates a new user with a given name
        public async Task<ServerResponse> CreateUser(string username, string password)
        {
            ServerResponse response = await _db.CreateUser(username, password);
            bool created = response.Status;
            if (created)
            {
                User user = await _db.GetUserByUsername(username);
                SetCurrentUser(user.id.ToString());
                SetLoggedInStatus(true);
            }
            return response;
        }

        //checks if username is taken | WIP when creating user and changing name | client side only
        public async Task<bool> CheckUsername(string username)
        {
            return await _db.CheckUsername(username);
        }

        //creates a new company
        public async Task<ServerResponse> CreateCompany(string name)
        {
            if (!GetLoggedInStatus())
            {
                return new ServerResponse(false, "Not logged in!");
            }
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
        public async Task<ServerResponse> DeleteUser()
        {
            if (!GetLoggedInStatus())
            {
                return new ServerResponse(false, "Not logged in!");
            }
            ServerResponse response = await _db.DeleteUser(GetCurrentUserID());
            if (response.Status)
            {
                RemoveCurrentUser();
            }
            return response;
        }
        
        public async Task<bool> DeleteCompany(string id) //not yet implemented
        {
            return false;
            //return await _db.DeleteCompany(id);
        }

        private const string SessionKeyCompany = "_currentCompany";
        //sets a new company in session
        public void SetCurrentCompany(string id)
        {
            HttpContext.Session.SetString(SessionKeyCompany, id);
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
        private bool SetCurrentUser(string id)
        {
            HttpContext.Session.SetString(SessionKeyUser, id);
            return true;
        }

        //returns current user id, after setting  a standard in some cases
        public string GetCurrentUserID()
        {
            if (!GetLoggedInStatus())
            {
                return null;
            }
            string cur = HttpContext.Session.GetString(SessionKeyUser);
            if (cur != null && cur != "")
            {
                return cur;
            }
            return null;
        }

        //returns current user
        public async Task<User> GetCurrentUser()
        {
            if (!GetLoggedInStatus())
            {
                return null;
            }
            string userID = GetCurrentUserID();
            if(userID == null)
            {
                return null;
            }
            return await _db.GetUserByID(Guid.Parse(GetCurrentUserID()));
        }

        //removes current user
        public void RemoveCurrentUser()
        {
            HttpContext.Session.SetString(SessionKeyUser, "");
        }
        private const string SessionKeyLoggedIn = "_loggedIn";

        //sets a new user to the session
        private void SetLoggedInStatus(bool status)
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

        public bool GetLoggedInStatus()
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

        public async Task<ServerResponse> LogIn(string username, string password)
        {
            ServerResponse response = await _db.LogIn(username, password);
            if (response.Status)
            {
                User user = await _db.GetUserByUsername(username);
                SetCurrentUser(user.id.ToString());
                SetLoggedInStatus(true);
            }
            return response;
        }
        public void LogOut()
        {
            RemoveCurrentUser();
            SetLoggedInStatus(false);
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
            List<Stock> dbList = await _db.GetStocksWithUserID(GetCurrentUserID());
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
        public async Task<StockName> GetUsersValueByID(string id)
        {
            return await _db.GetUsersValueByID(id);
        }

       
        

        //tries to buy stock for user and company in session
        public async Task<ServerResponse> BuyStock(int amount)
        {
            return await _db.TryToBuyStockForUser(GetCurrentUserID(), GetCurrentCompanyID(), amount);
        }

        //tries to sell stock for user and company in session
        public async Task<ServerResponse> SellStock(int amount)
        {
            return await _db.TryToSellStockForUser(GetCurrentUserID(), GetCurrentCompanyID(), amount);
        }

        //gets the current stock from user and company in session
        public async Task<Stock> GetCurrentStock()
        {
            return await _db.GetStockWithUserAndCompany(GetCurrentUserID(), GetCurrentCompanyID());
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
