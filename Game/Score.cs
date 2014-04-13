using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chibre_Server.Game
{
    class Score
    {
        private int one;
        private int twenty;
        private int fifty;
        private int hundred;

        public Score()
        {
            this.one = this.twenty = this.fifty = this.hundred = 0;
        }

        public int Twenty
        {
            get { return twenty; }
        }

        public int Fifty
        {
            get { return fifty; }
        }

        public int Hundred
        {
            get { return hundred; }
        }

        public int One
        {
            get { return one; }
        }

        public void AddPoints(int points)
        {
            hundred += points / 100;
            points -= points / 100;

            fifty += points / 50;
            points -= points / 50;

            twenty += points / 20;
            points -= points / 20;

            one += points;
        }
    }
}
