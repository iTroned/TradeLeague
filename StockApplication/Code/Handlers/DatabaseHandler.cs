using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApplication.Code.Handlers
{
    public class DatabaseHandler
    {
        public DatabaseHandler()
        {

        }
        public User getUserByID(Guid id)
        {
            return new User();
        }
        public bool updateUser(Guid id, User user)
        {
            return true;
        }
        public Guid createUser(User user)
        {
            Guid id = Guid.NewGuid();
            return id;
        }
        public Stock getStockByID(Guid id)
        {
            return new Stock();
        }
        public bool updateStock(Guid id, Stock stock)
        {
            return true;
        }
        public Guid createStock(Stock stock)
        {
            Guid id = Guid.NewGuid();
            return id;
        }
        public Stock getCompanyByID(Guid id)
        {
            return new Stock();
        }
        public bool updateCompany(Guid id, Company company)
        {
            return true;
        }
        public Guid createCompany(Company company)
        {
            Guid id = Guid.NewGuid();
            return id;
        }
        public float getBalanceForUser(User user)
        {
            return 0;
        }
       
        public bool setBalanceForUser(User user, float balance)
        {
            return true;
        }
    }
}
