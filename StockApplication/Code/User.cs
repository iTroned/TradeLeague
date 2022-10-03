using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApplication.Code
{
    public class User
    {
        public Guid id { get; set; }
        public string username { get; set; }

        public Stock[] ownedStocks { get; set; }
        public float balance { get; set; }
        public User() : this(Guid.Empty, null, null, 0)
        {

        }
        public User(Guid id, string username, Stock[] ownedStocks, float balance)
        {
            this.id = id;
            this.username = username;
            if(ownedStocks != null)
            {
                this.ownedStocks = ownedStocks;
            }
            else
            {
                this.ownedStocks = new Stock[0];
            }
            this.balance = balance;
        }
        public User clone()
        {
            return new User(id, username, ownedStocks, balance);
        }
    }
}
