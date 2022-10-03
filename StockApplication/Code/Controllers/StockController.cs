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
        public async Task<User> getUserByID(Guid id)
        {
            return await _db.getUserByID(id);
        }
        public async Task<User> getUserByUsername(string username)
        {
            return await _db.getUserByUsername(username);
        }
        public async Task<List<User>> getAllUsers()
        {
            return await _db.getAllUsers();
        }
        public async Task<bool> updateUser(Guid id, string username)
        {
            return await _db.updateUser(id, username);
        }
        public async Task<bool> createUser(string username)
        {
            return await _db.createUser(username);  
        }
        public async Task<bool> checkUsername(string username)
        {
            return await _db.checkUsername(username);
        }

    }
}
