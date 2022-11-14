using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApplication.Code.Handlers
{
    public interface IStockRepository
    {
        Task<User> GetUserByID(Guid id);
        Task<User> GetUserByUsername(string username);
        Task<List<User>> GetAllUsers();
        Task<bool> UpdateUser(User editUser);
        Task<bool> CreateUser(string username, string password);
        Task<bool> LogIn(string username, string password);
        Task<bool> CheckUsername(string username);
        Task<bool> CreateCompany(string name);
        Task<Company> GetCompanyByID(Guid id);
        Task<List<Company>> GetAllCompanies();
        Task<bool> DeleteUser(string id);
        Task<bool> DeleteCompany(string id);
        Task<bool> TryToBuyStockForUser(string userID, string companyID, int amount);
        Task<bool> TryToSellStockForUser(string userID, string companyID, int amount);
        Task<float> GetBalanceForUser(Guid id);
        void SetBalanceForUser(User user, float balance);
        Task<Stock> GetStockByID(Guid id);
        Task<bool> DeleteStock(Stock stock, bool save);
        void AddBalanceToUser(User user, float value);
        Task<bool> RemoveBalanceFromUser(User user, float value);
        Task<bool> AddStockToUser(User user, Company company, int amount);
        Task<bool> CreateStock(User user, Company company, int amount);
        Task<bool> RemoveStockFromUser(Stock stock, int amount);
        Task<List<Stock>> GetStocksWithUser(User user);
        Task<List<Stock>> GetStocksWithUserID(string id);
        Task<List<Stock>> GetStocksWithCompany(Company company);
        Task<Stock> GetStockWithUserAndCompany(User user, Company company);
        Task<Stock> GetStockWithUserAndCompany(string uid, string cid);
        Task<bool> UserHasStocksWithCompany(User user, Company company);
        Task<List<StockName>> GetAllUsersTotalValue();
        Task<StockName> GetUsersValueByID(String id);
        Task<float> GetStockValue(Stock stock);
        Task<List<Stock>> GetAllStocks();
        Task UpdateValues();
    }
}
