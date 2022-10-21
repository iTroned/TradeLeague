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
        Task<bool> updateUser(User editUser);
        Task<bool> createUser(string username);
        Task<bool> checkUsername(string username);
        Task<bool> createCompany(string name);
        Task<Company> getCompanyByID(Guid id);
        Task<List<Company>> getAllCompanies();
        Task<bool> deleteUser(string id);
        Task<bool> deleteCompany(string id);
        Task<bool> tryToBuyStockForUser(string userID, string companyID, int amount);
        Task<bool> tryToSellStockForUser(string userID, string companyID, int amount);
        Task<float> getBalanceForUser(Guid id);
        void setBalanceForUser(User user, float balance);
        Task<Stock> getStockByID(Guid id);
        Task<bool> deleteStock(Guid id);
        void addBalanceToUser(User user, float value);
        Task<bool> removeBalanceFromUser(User user, float value);
        Task<bool> userHasEnoughBalance(User user, float value);
        Task<bool> addStockToUser(User user, Company company, int amount);
        Task<bool> createStock(User user, Company company, int amount);
        Task<bool> removeStockFromUser(User user, Company company, int amount);
        Task<List<Stock>> getStocksWithUser(User user);
        Task<List<Stock>> getStocksWithUserID(string id);
        Task<List<Stock>> getStocksWithCompany(Company company);
        Task<Stock> getStockWithUserAndCompany(User user, Company company);
        Task<Stock> getStockWithUserAndCompany(string uid, string cid);
        Task<bool> userHasStocksWithCompany(User user, Company company);
        Task<bool> updateValues();
        Task<List<Stock>> getAllStocks();
    }
}
