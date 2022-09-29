using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApplication.Code
{
    public class Company
    {
        private string name { get; set; }
        private Guid id { get; set; }
        private int value { get; set; }
        public Company() : this(null, Guid.Empty, 0)
        {
            
        }
        public Company(string name, Guid id, int value)
        {
            this.name = name;
            this.id = id;
            this.value = value;
        }

    }
}
