using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApplication.Code
{
    public class User
    {
        private Guid id { get; set; }
        private string username { get; set; }

        private Stock[] ownedStocks { get; set; }
        private float balance { get; set; }
        public User() : this(Guid.Empty, null, null, 0)
        {

        }
        public User(Guid id, string username, Stock[] ownedStocks, float balance)
        {
            this.id = id;
            this.username = username;
            this.ownedStocks = ownedStocks;
            this.balance = balance;
        }
    }
}
