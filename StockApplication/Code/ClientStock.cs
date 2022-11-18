using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockApplication.Code
{
    
    public class ClientStock
    {
        [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,20}")]
        public string name { get; set; }
        public int amount { get; set; }
        public float value { get; set; }
        public ClientStock() : this(null, 0, 0)
        {

        }
        public ClientStock(string name, int amount, float value)
        {
            this.name = name;
            this.amount = amount;
            this.value = value;
        }
        public ClientStock(string name, int amount) : this(name, amount, 0)
        {

        }

    }
}
