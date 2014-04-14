using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Chibre_Server.Game
{
    class Team
    {
        private Player[] players;
        private Score score;
        private GameEngine gameEngine;

        public Team(GameEngine gameEngine)
        {
            score = new Score();
            players = new Player[2];
            this.gameEngine = gameEngine;
        }

        public void addPlayer(Player player)
        {
            Debug.Assert(players.Length < 2);
            players[players.Length - 1] = player;
        }

        #region Properties
        public int Length
        {
            get { return players.Length; }
        }

        public Score Score
        {
            get { return score; }
        }

        public Player this[int index]
        {
            get { return players[index]; }
        }

        public GameEngine GameEngine
        {
            get { return gameEngine; }
        }
        #endregion
    }
}
