using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFT585_TP3
{
    static class IFT585Helper
    {
        public static IEnumerable<T> Flatten<T>(params IEnumerable<T>[] xss)
        {
            return xss.Aggregate((acc, x) => acc.Concat(x));
        }
    }
}
