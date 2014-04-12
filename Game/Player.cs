using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chibre_Server.Game
{
    class Player
    {
        private Connection connection;
        private HashSet<Card> cards;

        public Player(int id, ref Team team, ref Connection connection)
        {
            this.Id = id;
            this.Team = team;
            this.connection = connection;
            this.cards = new HashSet<Card>();
        }

        public Team Team
        {
            private set;
            get;
        }

        public HashSet<Card> Cards
        {
            get
            {
                return this.cards;
            }
        }

        public int Id
        {
            private set;
            get;
        }

        public void Announce(AnnounceType a)
        {

        }

        public void PlayCard(Card card)
        {

        }
    }
}
