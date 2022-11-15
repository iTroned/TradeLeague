using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApplication.Code.DAL
{
    public interface IStockRepository
    {
        Task<User> GetUserByID(Guid id);
        Task<User> GetUserByUsername(string username);
        Task<List<User>> GetAllUsers();
        Task<List<ClientUser>> GetAllClientUsers();
        Task<ServerResponse> UpdateUser(User editUser);
        Task<ServerResponse> CreateUser(string username, string password);
        Task<ServerResponse> LogIn(string username, string password);
        Task<bool> CheckUsername(string username);
        Task<ServerResponse> CreateCompany(string name);
        Task<Company> GetCompanyByID(Guid id);
        Task<List<Company>> GetAllCompanies();
        Task<ServerResponse> DeleteUser(string id);
        Task<ServerResponse> DeleteCompany(string id);
        Task<ServerResponse> TryToBuyStockForUser(string userID, string companyID, int amount);
        Task<ServerResponse> TryToSellStockForUser(string userID, string companyID, int amount);
        Task<float> GetBalanceForUser(Guid id);
        Task<Stock> GetStockByID(Guid id);
        //Task<bool> DeleteStock(Stock stock, bool save); //should not be public in this case
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
