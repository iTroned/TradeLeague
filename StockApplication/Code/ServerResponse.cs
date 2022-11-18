using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StockApplication.Code
{
    public class ServerResponse
    {
        public bool Status { get; set; }
        [RegularExpression(@"[a-zA-ZæøåÆØÅ. \-]{2,30}")]
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
