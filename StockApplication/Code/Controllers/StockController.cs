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
        public async Task<List<User>> getAllUsers()
        {
            return await _db.getAllUsers();
        }
        public async Task<bool> updateUser(User editUser)
        {
            return await _db.updateUser(editUser);
        }
        public async Task<bool> createUser(string username)
        {
            return await _db.createUser(username);  
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
        public async Task<bool> deleteUser(string id)
        {
            return await _db.deleteUser(id);
        }
        public async Task<bool> deleteCompany(string id)
        {
            return await _db.deleteCompany(id);
        }

       


        private const string SessionKeyUser = "_currentUser";
        public void setCurrentUser(string id)
        {
            HttpContext.Session.SetString(SessionKeyUser, id);
        }

        public string getCurrentUser()
        {
            return HttpContext.Session.GetString(SessionKeyUser);
        }
        public void removeCurrentUser()
        {
            HttpContext.Session.SetString(SessionKeyUser, "");
        }

    }
}
