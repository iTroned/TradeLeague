using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace StockApplication.Code
{
    public class StockNameComparer : IComparer<StockName>
    {
        public int Compare([AllowNull] StockName x, [AllowNull] StockName y)
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
