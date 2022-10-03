using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApplication.Code
{
    public class Stock
    {
        public Guid id { get; set; }
        private int amount { get; set; }
        private Company company { get; set; }
        public Stock() : this(Guid.Empty, 0, 0, null)
        {

        }
        public Stock(Guid id, int amount, float value, Company company)
        {
            this.id = id;
            this.amount = amount;
            this.company = company;
        }
        
    }
}
