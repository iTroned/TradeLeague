using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApplication.Code
{
    public class ClientUser
    {
        public string username { get; set; }
        public float balance { get; set; }
        public ClientUser(string username, float balance)
        {
            this.username = username;
            this.balance = balance;
        }
    }
}
