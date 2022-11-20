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
        private readonly string RESPONSE_companyNotActive = "No company is currently active!";
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
            string id = HttpContext.Session.GetString(SessionKeyUser); //get user-id from session key
            if (string.IsNullOrEmpty(id) && id != user.id.ToString()) //checking if id from sesion key is valid and matches the user.id
            {
                _log.LogInformation(RESPONSE_userNotLoggedIn);
                return Unauthorized(RESPONSE_userNotLoggedIn);
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
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyUser)))
            {
                _log.LogInformation(RESPONSE_userNotLoggedIn);
                return Unauthorized(RESPONSE_userNotLoggedIn);
            }
            string id = HttpContext.Session.GetString(SessionKeyUser);
            ServerResponse response = await _db.UpdateUser(id, username);
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
            if(user == null)
            {
                _log.LogInformation(response.Response);
                return BadRequest(response.Response);
            }
            HttpContext.Session.SetString(SessionKeyUser, user.id.ToString());

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
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyUser)))
            {
                _log.LogInformation(RESPONSE_userNotLoggedIn);
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
        public async Task<ActionResult> GetCompanyByName(string name)
        {
            ClientCompany company = await _db.GetCompanyByName(name);
            if(company == null)
            {
                _log.LogInformation(RESPONSE_companyNotFound);
                return NotFound(RESPONSE_companyNotFound);
            }
            return Ok(company);
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
            string id = HttpContext.Session.GetString(SessionKeyUser);
            if (string.IsNullOrEmpty(id))
            {
                _log.LogInformation(RESPONSE_userNotLoggedIn);
                return Unauthorized(RESPONSE_userNotLoggedIn);
            }

            ServerResponse response = await _db.DeleteUser(id);
            if (!response.Status)
            {
                _log.LogInformation(response.Response);
                return BadRequest(response.Response); //Check
            }
            HttpContext.Session.SetString(SessionKeyUser, "");
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
            string id = HttpContext.Session.GetString(SessionKeyCompany); //get current company id
            if (string.IsNullOrEmpty(id))
            {
                _log.LogInformation(RESPONSE_companyNotActive);
                return Unauthorized(RESPONSE_companyNotActive);
            }

            Company company = await _db.GetCompanyByID(Guid.Parse(id));
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


        //returns current user
        public async Task<ActionResult> GetCurrentUser()
        {
            string id = HttpContext.Session.GetString(SessionKeyUser);
            if (string.IsNullOrEmpty(id))
            {
                _log.LogInformation(RESPONSE_userNotLoggedIn);
                return Unauthorized(RESPONSE_userNotLoggedIn);
            }

            User user = await _db.GetUserByID(Guid.Parse(id));
            if(user == null)
            {
                _log.LogInformation(RESPONSE_userNotFound);
                return NotFound(RESPONSE_userNotFound);
            }
            return Ok(new ClientUser(user.username, user.balance));
        }

        
        private const string _loggedIn = "loggedIn";
        private const string _notLoggedIn = "";

        

       

        public async Task<ActionResult> LogIn(string username, string password)
        {
            ServerResponse response = await _db.LogIn(username, password);
            if (!response.Status)
            {
                _log.LogInformation(response.Response);
                return BadRequest(response.Response);
            }
            User user = await _db.GetUserByUsername(username);
            if(user == null)
            {
                _log.LogInformation(RESPONSE_userNotFound);
                return NotFound(RESPONSE_userNotFound);
            }

            HttpContext.Session.SetString(SessionKeyUser, user.id.ToString());
            return Ok("Logged in");
        }
        public void LogOut()
        {
            HttpContext.Session.SetString(SessionKeyUser, "");
        }

        //returns list of stocks certain user own
        public async Task<ActionResult> GetStocksForUser(string id)
        {
            string userid = HttpContext.Session.GetString(SessionKeyUser);
            if (string.IsNullOrEmpty(userid) && userid != id)
            {
                _log.LogInformation(RESPONSE_userNotLoggedIn);
                return Unauthorized(RESPONSE_userNotLoggedIn);
            }
         
            return Ok(await _db.GetStocksWithUserID(userid));
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
            string userid = HttpContext.Session.GetString(SessionKeyUser);
            if (string.IsNullOrEmpty(userid) && userid != id)
            {
                _log.LogInformation(RESPONSE_userNotLoggedIn);
                return Unauthorized(RESPONSE_userNotLoggedIn);
            }
            ClientStock stockName = await _db.GetUsersValueByID(id);
            if (stockName == null)
            {
                _log.LogInformation(RESPONSE_userNotFound);
                return NotFound(RESPONSE_userNotFound);
            }
            return Ok(stockName);
        }

       
        

        //tries to buy stock for user and company in session
        public async Task<ActionResult> BuyStock(int amount)
        {
            string userid = HttpContext.Session.GetString(SessionKeyUser);
            string companyid = HttpContext.Session.GetString(SessionKeyCompany);
            if (string.IsNullOrEmpty(userid) && string.IsNullOrEmpty(companyid))
            {
                _log.LogInformation("You have to be logged in to perform this action or No company is currently active");
                return Unauthorized("You have to be logged in to perform this action or No company is currently active");
            }
            ServerResponse response = await _db.TryToBuyStockForUser(userid, companyid, amount);
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
            string userid = HttpContext.Session.GetString(SessionKeyUser);
            string companyid = HttpContext.Session.GetString(SessionKeyCompany);
            if (string.IsNullOrEmpty(userid) && string.IsNullOrEmpty(companyid))
            {
                _log.LogInformation("You have to be logged in to perform this action or No company is currently active");
                return Unauthorized("You have to be logged in to perform this action or No company is currently active");
            }
            ServerResponse response = await _db.TryToSellStockForUser(userid, companyid, amount);
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
            string userid = HttpContext.Session.GetString(SessionKeyUser);
            string companyid = HttpContext.Session.GetString(SessionKeyCompany);
            if (string.IsNullOrEmpty(userid) && string.IsNullOrEmpty(companyid))
            {
                _log.LogInformation("You have to be logged in to perform this action or No company is currently active");
                return Unauthorized("You have to be logged in to perform this action or No company is currently active");
            }
            Stock stock = await _db.GetStockWithUserAndCompany(userid, companyid);
            if(stock == null)
            {
                _log.LogInformation(RESPONSE_stockNotFound);
                return BadRequest(RESPONSE_stockNotFound);
            }
            return Ok(new ClientStock(stock.companyName, stock.amount));
        }





    }
}
