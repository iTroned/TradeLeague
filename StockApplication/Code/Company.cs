using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApplication.Code
{
    public class Company
    {
        public string name { get; set; }
        public Guid id { get; set; }
        public float value { get; set; }
        public Company() : this(Guid.Empty, null, 0)
        {
            
        }
        public Company(Guid id, string name, float value)
        {
            this.name = name;
            this.id = id;
            this.value = value;
        }
        public Company clone()
        {
            return new Company(id, name, value);
        }

    }
}
