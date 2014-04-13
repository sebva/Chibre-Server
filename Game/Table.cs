using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chibre_Server.Game
{
    class Table
    {
        private Announce announce;
        private List<Card> cards;
        private GameEngine gameEngine;

        public Table(GameEngine gameEngine)
        {
            this.gameEngine = gameEngine;
            cards = new List<Card>();
        }

        public void AddCard(Card card)
        {
            cards.Add(card);
        }

        public void AddAnnounce(Announce announce)
        {
            this.announce = announce;
            // TODO : Communicate with the GUI
        }

        public int Length
        {
            get { return cards.Count; }
        }

        public List<Card> Cards
        {
            get { return cards; }
        }
    }
}
