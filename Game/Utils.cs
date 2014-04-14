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
        public static void Shuffle<T>(ref List<T> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                int a = i;
                int b = random.Next(i, list.Count - 1);
                T temp = list[a];
                list[a] = list[b];
                list[b] = temp;
            }
        }
    }
}
