using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chibre_Server.Game
{
    static class Utils
    {
        private static Random random;

        static Utils()
        {
            random = new Random();
        }

        /// <summary>
        /// Use Fisher-Yates algorithm - O(n)
        /// </summary>
        /// <param name="list">List to shuffle</param>
        /// <returns>Shuffled list</returns>
        public static void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                T a = list[i];
                T b = list[random.Next(i, list.Count - 1)];
                Swap<T>(ref a, ref b);
            }
        }
    
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }
}
