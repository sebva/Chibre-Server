using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chibre_Server.Game
{
    class Team
    {
        private Player[] players;
        private Score score;
        public Team()
        {
            this.score = new Score();
            players = new Player[2];
        }

        public Score Score
        {
            private set;
            get;
        }
    }
}
