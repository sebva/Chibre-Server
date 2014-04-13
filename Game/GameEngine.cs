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
        private Table table;
        private Team[] teams;
        private Color atout;

        private GameEngine()
        {
            teams = new Team[2];
            teams[0] = new Team();
            teams[1] = new Team();
            table = new Table();
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
