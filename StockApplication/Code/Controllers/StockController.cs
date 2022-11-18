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

        private readonly string RESPONSE_userNotLoggedIn = "You have to be logged in to perform this action!";
        private readonly string RESPONSE_userNotFound = "User could not be found!";
        private readonly string RESPONSE_companyNotFound = "Company could not be found!";
        private readonly string RESPONSE_stockNotFound = "Stock could not be found!";
        private readonly string RESPONSE_usernameInvalid = "Username was invalid or already taken!";
        public StockController(IStockRepository db, ILogger<StockController> log)
        {
            _db = db;
            _log = log;
        }
        //returns a user by its id || should never be used as it takes in an id
        public async Task<ActionResult> GetUserByID(string id)
        {
            User user = await _db.GetUserByID(Guid.Parse(id));
            if(user == null)
            {
                _log.LogInformation(RESPONSE_userNotFound);
                return NotFound(RESPONSE_userNotFound);
            }
            return Ok(new ClientUser(user.username, user.balance));
        }

        //returns a user by a given username
        public async Task<ActionResult> GetUserByUsername(string username)
        {
            User user = await _db.GetUserByUsername(username);
            if (user == null)
            {
                _log.LogInformation(RESPONSE_userNotFound);
                return NotFound(RESPONSE_userNotFound);
            }
            if (GetCurrentUserID() != user.id.ToString())
            {
                return Unauthorized("That user is not logged in");
            }
            //never pass id to client
            return Ok(new ClientUser(user.username, user.balance));
        }

        //returns all registered users with their usernames and passwords
        public async Task<ActionResult> GetAllUsers()
        {
            List<ClientUser> allUsers = await _db.GetAllClientUsers();
            return Ok(allUsers);
        }

        //update a user, does not work for admin
        public async Task<ActionResult> UpdateUser(string username)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggedIn)))
            {
                _log.LogInformation(RESPONSE_userNotLoggedIn);
                return Unauthorized(RESPONSE_userNotLoggedIn);
            }
            if (ModelState.IsValid)
            {
                ServerResponse response = await _db.UpdateUser(GetCurrentUserID(), username);
                bool updated = response.Status;

                if (!updated)
                {
                    _log.LogInformation(response.Response);
                    return BadRequest(response.Response);
                }
                return Ok("User updated");
            }
            _log.LogInformation("Error in inputvalidation on server");
            return BadRequest("Error in inputvalidation on server");
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
                _log.LogInformation(RESPONSE_usernameInvalid);
                return BadRequest(RESPONSE_usernameInvalid);
            }
            return Ok("Username is valid");
        }

        //creates a new company
        public async Task<ActionResult> CreateCompany(string name)
        {
            if (!GetLoggedInStatus())
            {
                return Unauthorized(RESPONSE_userNotLoggedIn);
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
                _log.LogInformation(RESPONSE_companyNotFound);
                return NotFound(RESPONSE_companyNotFound);
            }
            return Ok(new ClientCompany(company.name, company.value, company.values));
        }

        //returns a list of all companies registeres
        public async Task<ActionResult> GetAllCompanies()
        {
            List<Company> companyList = await _db.GetAllCompanies();
            List<ClientCompany> ret = new List<ClientCompany>();
            foreach(Company company in companyList)
            {
                ret.Add(new ClientCompany(company.name, company.value, company.values));
            }
            return Ok(ret);
        }

        //deletets the user in the session. cannot delete admin
        public async Task<ActionResult> DeleteUser()
        {
            if (!GetLoggedInStatus())
            {
                _log.LogInformation(RESPONSE_userNotLoggedIn);
                return Unauthorized(RESPONSE_userNotLoggedIn);
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
                _log.LogInformation(RESPONSE_companyNotFound);
                return NotFound(RESPONSE_companyNotFound);
            }
            return Ok(new ClientCompany(company.name, company.value, company.values));
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

        //returns current user id, after setting  a standard in some cases //should not be accessible to client
        private string GetCurrentUserID()
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
                _log.LogInformation(RESPONSE_userNotLoggedIn);
                return Unauthorized(RESPONSE_userNotLoggedIn);
            }
            User user = await _db.GetUserByID(Guid.Parse(GetCurrentUserID()));
            if(user == null)
            {
                _log.LogInformation(RESPONSE_userNotFound);
                return NotFound(RESPONSE_userNotFound);
            }
            return Ok(new ClientUser(user.username, user.balance));
        }

        //removes current user
        public void RemoveCurrentUser()
        {
            HttpContext.Session.SetString(SessionKeyUser, "");
        }
        private const string _loggedIn = "loggedIn";
        private const string _notLoggedIn = "";


        //sets a new user to the session
        private void SetLoggedInStatus(bool status)
        {
            if (status)
            {
                HttpContext.Session.SetString(_loggedIn, _loggedIn);
            }
            else
            {
                HttpContext.Session.SetString(_loggedIn, _notLoggedIn);
            }
        }

        public bool GetLoggedInStatus()
        {
            try
            {
                string val = HttpContext.Session.GetString(_loggedIn);
                if(val == _loggedIn)
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

        //returns list of stocks certain user own
        public async Task<ActionResult> GetStocksForUser(string id)
        {
            List<Stock> dbList = await _db.GetStocksWithUserID(GetCurrentUserID());
            List<ClientStock> stockList = new List<ClientStock>();
            foreach(Stock stock in dbList)
            {
                stockList.Add(new ClientStock(stock.companyName, stock.amount, await _db.GetStockValue(stock)));
            }
            return Ok(stockList);
        }

        //returns list with total value for every user
        public async Task<ActionResult> GetUsersValue() 
        {
            List<ClientStock> stocks = await _db.GetAllUsersTotalValue();
            return Ok(stocks);
        }

        //get ClientStock object with specific user's value
        public async Task<ActionResult> GetUsersValueByID(string id)
        {
            ClientStock stockName = await _db.GetUsersValueByID(id);
            if (stockName.name == null)
            {
                _log.LogInformation(RESPONSE_userNotFound);
                return NotFound(RESPONSE_userNotFound);
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
                _log.LogInformation(RESPONSE_stockNotFound);
                return BadRequest(RESPONSE_stockNotFound);
            }
            return Ok(new ClientStock(stock.companyName, stock.amount));
        }





    }
}
