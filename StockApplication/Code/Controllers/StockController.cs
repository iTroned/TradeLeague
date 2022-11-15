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
        private readonly ILogger _log;
        
        public StockController(IStockRepository db, IHostedService listenerService, ILogger<StockController> log)
        {
            _db = db;
            _log = log;
            
            this.listenerService = listenerService;
        }
        //returns a user by its id
        public async Task<ActionResult> GetUserByID(string id)
        {
            User user = await _db.GetUserByID(Guid.Parse(id));
            if(user == null)
            {
                _log.LogInformation("Did not find user");
                return NotFound("Did not find the user");
            }
            return Ok(user);
        }

        //returns a user by a given username
        public async Task<ActionResult> GetUserByUsername(string username)
        {
            User user = await _db.GetUserByUsername(username);
            if(user == null)
            {
                _log.LogInformation("Did not find user");
                return NotFound("Did not find the user");
            }
           
            return Ok(user);
        }

        //returns all registered users with their usernames and passwords
        public async Task<ActionResult> GetAllUsers()
        {
            List<ClientUser> allUsers = await _db.GetAllClientUsers();
            return Ok(allUsers);
        }

        //update a user, does not work for admin
        public async Task<ActionResult> UpdateUser(User editUser)
        {
            if (!GetLoggedInStatus())
            {
                _log.LogInformation("User not logged in");
                return Unauthorized("User not logged in");
            }

            ServerResponse response = await _db.UpdateUser(editUser);
            bool updated = response.Status;

            if (!updated)
            {
                _log.LogInformation(response.Response);
                return BadRequest(response.Response);
            }
            return Ok("User updated");
        }

        //creates a new user with a given name
        public async Task<ActionResult> CreateUser(string username, string password)
        {
            ServerResponse response = await _db.CreateUser(username, password);
            bool created = response.Status;
            if (!created)
            {
                _log.LogInformation(response.Response);
                return BadRequest(response.Response);
            }
            User user = await _db.GetUserByUsername(username);
            SetCurrentUser(user.id.ToString());
            SetLoggedInStatus(true);

            return Ok("User created");
        }

        //checks if username is taken | WIP when creating user and changing name | client side only
        public async Task<ActionResult> CheckUsername(string username)
        {
            bool checkName = await _db.CheckUsername(username);

            if (!checkName)
            {
                _log.LogInformation("Username was taken or invalid");
                return BadRequest("Username was taken or invalid");
            }
            return Ok("Username is valid");
        }

        //creates a new company
        public async Task<ActionResult> CreateCompany(string name)
        {
            if (!GetLoggedInStatus())
            {
                return Unauthorized("User not logged in");
            }

            ServerResponse response = await _db.CreateCompany(name);
            bool created = response.Status;

            if (!created)
            {
                _log.LogInformation(response.Response);
                return BadRequest(response.Response);
            }
            return Ok("Company created");
        }

        //gets a single company by its id
        public async Task<ActionResult> GetCompanyByID(string id)
        {
            Company company = await _db.GetCompanyByID(Guid.Parse(id));
            if(company == null)
            {
                _log.LogInformation("Did not find company");
                return NotFound("Did not find the company");
            }
            return Ok(company);
        }

        //returns a list of all companies registeres
        public async Task<ActionResult> GetAllCompanies()
        {
            List<Company> companyList = await _db.GetAllCompanies();
            return Ok(companyList);
        }

        //deletets the user in the session. cannot delete admin
        public async Task<ActionResult> DeleteUser()
        {
            if (!GetLoggedInStatus())
            {
                _log.LogInformation("User not logged in");
                return Unauthorized("User not logged in");
            }

            ServerResponse response = await _db.DeleteUser(GetCurrentUserID());
            if (!response.Status)
            {
                _log.LogInformation(response.Response);
                return BadRequest(response.Response); //Check
            }
            RemoveCurrentUser();
            return Ok("User deleted");
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
        public async Task<ActionResult> GetCurrentCompany()
        {
            Company company = await _db.GetCompanyByID(Guid.Parse(GetCurrentCompanyID()));
            if(company == null)
            {
                _log.LogInformation("Company not found");
                return NotFound("Company not found");
            }
            return Ok(company);
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
        public async Task<ActionResult> GetCurrentUser()
        {
            if (!GetLoggedInStatus())
            {
                _log.LogInformation("User not logged in");
                return Unauthorized("User not logged in");
            }
            string userID = GetCurrentUserID();
            if(userID == null)
            {
                _log.LogInformation("UserID not found in session");
                return NotFound("UserID not found in session");
            }
            User user = await _db.GetUserByID(Guid.Parse(GetCurrentUserID()));
            if(user == null)
            {
                _log.LogInformation("User not found");
                return NotFound("User not found");
            }
            return Ok(user);
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

        public async Task<ActionResult> LogIn(string username, string password)
        {
            ServerResponse response = await _db.LogIn(username, password);
            if (!response.Status)
            {
                _log.LogInformation(response.Response);
                return BadRequest(response.Response);
            }
            User user = await _db.GetUserByUsername(username);
            SetCurrentUser(user.id.ToString());
            SetLoggedInStatus(true);

            return Ok("Logged in");
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
        public async Task<ActionResult> GetStocksForUser(string id)
        {
            List<Stock> dbList = await _db.GetStocksWithUserID(GetCurrentUserID());
            List<StockName> stockList = new List<StockName>();
            foreach(Stock stock in dbList)
            {
                stockList.Add(new StockName(stock.companyName, stock.amount, await _db.GetStockValue(stock)));
            }
            return Ok(stockList);
        }

        //returns list with total value for every user
        public async Task<ActionResult> GetUsersValue() 
        {
            List<StockName> stocks = await _db.GetAllUsersTotalValue();
            return Ok(stocks);
        }

        //get StockName object with specific user's value
        public async Task<ActionResult> GetUsersValueByID(string id)
        {
            StockName stockName = await _db.GetUsersValueByID(id);
            if (stockName.name == null)
            {
                _log.LogInformation("User not found");
                return NotFound("User not found");
            }
            return Ok(stockName);
        }

       
        

        //tries to buy stock for user and company in session
        public async Task<ActionResult> BuyStock(int amount)
        {
            ServerResponse response = await _db.TryToBuyStockForUser(GetCurrentUserID(), GetCurrentCompanyID(), amount);
            if(!response.Status == true)
            {
                _log.LogInformation(response.Response);
                return BadRequest(response.Response);
            }
            return Ok("Stock sucessfully bought");
        }

        //tries to sell stock for user and company in session
        public async Task<ActionResult> SellStock(int amount)
        {
            ServerResponse response = await _db.TryToSellStockForUser(GetCurrentUserID(), GetCurrentCompanyID(), amount);
            if (!response.Status == true)
            {
                _log.LogInformation(response.Response);
                return BadRequest(response.Response);
            }
            return Ok("Stock sucessfully sold");
            
        }

        //gets the current stock from user and company in session
        public async Task<ActionResult> GetCurrentStock()
        {
            Stock stock = await _db.GetStockWithUserAndCompany(GetCurrentUserID(), GetCurrentCompanyID());
            if(stock == null)
            {
                _log.LogInformation("Stock not found");
                return BadRequest("Stock not found");
            }
            return Ok(stock);
        }





    }
}
