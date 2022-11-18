using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApplication.Code
{
    //Company without the id
    public class ClientCompany
    {
        public string name { get; set; }
        public float value { get; set; }
        public string values { get; set; }
        public ClientCompany(string Name, float Value, string Values)
        {
            this.name = Name;
            this.value = Value;
            this.values = Values;
        }
    }
}
