using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chibre_Server
{
    /// <summary>
    /// KeyPair doesn't allow to modifie the reference in a foreach. This class allows it !
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    class Pair<T1, T2>
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
