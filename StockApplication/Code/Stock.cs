using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockApplication.Code
{
    public class Stock
    {
        public Guid id { get; set; }
        public int amount { get; set; }
        
        public Guid Userid { get; set; }
        public Guid Companyid { get; set; }
        [RegularExpression(@"[a-zA-ZÊ¯Â∆ÿ≈. \-]{2,20}")]
        public string companyName { get; set; }

        public virtual User User { get; set; }
        public virtual Company Company { get; set; }
        public Stock() : this(Guid.Empty, 0, Guid.Empty, Guid.Empty, null)
        {

        }
        public Stock(Guid id, int amount, Guid Userid, Guid Companyid, string companyName)
        {
            this.id = id;
            this.amount = amount;
            this.Userid = Userid;
            this.Companyid = Companyid;
            this.companyName = companyName;
        }
        public Stock Clone()
        {
            return new Stock(id, amount, Userid, Companyid, companyName);
        }

    }
}
