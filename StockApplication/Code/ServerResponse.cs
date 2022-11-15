using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApplication.Code
{
    public class ServerResponse
    {
        public bool Status { get; set; }

        public string Response { get; set; }
        public ServerResponse(bool Status, string Response)
        {
            this.Status = Status;
            this.Response = Response;
        }
        public ServerResponse(bool Status) : this(Status, "")
        {

        }
    }
    
}
