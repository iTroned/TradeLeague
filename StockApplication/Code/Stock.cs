using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApplication.Code
{
    public class Stock
    {
        public Guid id { get; set; }
        public int amount { get; set; }
        public string company { get; set; }
        public string owner { get; set; }
        public Stock() : this(Guid.Empty, 0, null, null)
        {

        }
        public Stock(Guid id, int amount, string owner, string company)
        {
            this.id = id;
            this.amount = amount;
            this.company = company;
        }
        public Stock clone()
        {
            return new Stock(id, amount, owner, company);
        }
        
    }
}
