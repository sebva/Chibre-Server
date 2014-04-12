using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chibre_Server.Game
{
    class GameEngine
    {
        private GameEngine instance = null;

        private GameEngine()
        {
            Teams = new Team[2];
            Teams[0] = new Team();
            Teams[1] = new Team();
            Table = new Table();
        }

        public GameEngine Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameEngine();

                return instance;
            }
        }

        public Color CurrentAtout
        {
            private set;
            get;
        }

        public Table Table
        {
            private set;
            get;
        }

        public Team[] Teams
        {
            private set;
            get;
        }

    }
}
