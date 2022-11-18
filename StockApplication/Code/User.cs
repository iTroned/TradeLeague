using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockApplication.Code
{
    public class User
    {
        public Guid id { get; set; }
        [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,20}")]
        public string username { get; set; }
        public string password { get; set; }
        public string salt { get; set; }
        public float balance { get; set; }
        public virtual ICollection<Stock> stocks { get; private set; }
        public User() : this(Guid.Empty, null, null, null, 0)
        {

        }
        public User(Guid id, string username, string password, string salt, float balance)
        {
            this.id = id;
            this.username = username;
            this.password = password;
            this.salt = salt;
            this.balance = balance;
        }
        public User Clone()
        {
            return new User(id, username, password, salt, balance);
        }
    }
}
