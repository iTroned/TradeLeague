using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace StockApplication.Code
{
    public class ClientStockComparer : IComparer<ClientStock>
    {
        public int Compare([AllowNull] ClientStock x, [AllowNull] ClientStock y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }
            return y.value.CompareTo(x.value);
        }
    }
}
