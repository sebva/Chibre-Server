using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chibre_Server.Game
{
    class Team
    {
        public Team()
        {
            this.Score = new Score();
        }

        public Score Score
        {
            private set;
            get;
        }
    }
}
