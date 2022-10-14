using Microsoft.AspNetCore.Mvc;
using StockApplication.Code.Handlers;
using System;
using StockApplication.Code.DAL;
using Microsoft.EntityFrameworkCore;
using StockApplication.Code;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<bool> setCurrentUser(string id)
        {
            return await _db.setCurrentUser(id);
        }
        public async Task<string> getCurrentUser()
        {
            return await _db.getCurrentUser();
        }

    }
}
