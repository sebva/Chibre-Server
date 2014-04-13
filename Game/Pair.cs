using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chibre_Server
{
    public class Pair<T1, T2>
    {
        public Pair(T1 t1, T2 t2)
        {
            First = t1;
            Second = t2;
        }
        public T1 First { get; set; }
        public T2 Second { get; set; }
    }
}
