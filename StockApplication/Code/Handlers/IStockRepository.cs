using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApplication.Code.Handlers
{
    public interface IStockRepository
    {
        Task<User> getUserByID(Guid id);
        Task<User> getUserByUsername(string username);
        Task<List<User>> getAllUsers();
        Task<bool> updateUser(Guid id, string username);
        Task<bool> createUser(string username);
        Task<bool> checkUsername(string username);
        Task<bool> createCompany(string name);
        Task<Company> getCompanyByID(Guid id);
        Task<List<Company>> getAllCompanies();
        Task<bool> deleteUser(string id);
        Task<bool> deleteCompany(string id);
    }
}
