using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chibre_Server.Game
{
    class Score
    {
        public Score()
        {
            this.One = this.Twenty = this.Fifty = this.Hundred = 0;
        }

        public int Twenty
        {
            private set;
            get;
        }

        public int Fifty
        {
            private set;
            get;
        }

        public int Hundred
        {
            private set;
            get;
        }

        public int One
        {
            private set;
            get;
        }

        public void AddPoints(int points)
        {
            // TODO
        }
    }
}
