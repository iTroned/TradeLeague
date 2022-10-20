using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApplication.Code
{
    
    public class StockName
    {
        public string name { get; set; }
        public int amount { get; set; }
        public StockName() : this(null, 0)
        {

        }
        public StockName(string name, int amount)
        {
            this.name = name;
            this.amount = amount;
        }
    }
}
