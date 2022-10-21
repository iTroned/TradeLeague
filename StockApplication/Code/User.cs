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
        public float balance { get; set; }
        public virtual ICollection<Stock> stocks { get; private set; }
        public User() : this(Guid.Empty, null, 0)
        {

        }
        public User(Guid id, string username, float balance)
        {
            this.id = id;
            this.username = username;
            this.balance = balance;
        }
        public User clone()
        {
            return new User(id, username, balance);
        }
    }
}
