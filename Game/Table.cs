using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Chibre_Server.Game
{
    class Table
    {
        private List<Pair<Card, int>> cards;
        private GameEngine gameEngine;

        public Table(GameEngine gameEngine)
        {
            this.gameEngine = gameEngine;
            cards = new List<Pair<Card, int>>();
        }

        public void AddCard(int playerId, Card card)
        {
            Debug.Assert(cards.Count <= 4);
            cards.Add(new Pair<Card, int>(card, playerId));
        }

        public int Length
        {
            get { return cards.Count; }
        }

        public Color FirstCardColor()
        {
            Debug.Assert(cards.Count > 0);
            return cards[0].First.Color;
        }

        public List<Card> Cards
        {
            get 
            {
                List<Card> output = new List<Card>();
                foreach (Pair<Card, int> pair in cards)
                    output.Add(pair.First);
                return output; 
            }
        }

        public void Clear()
        {
            cards.Clear();
        }

        public List<Pair<Card, int>> CardsByPlayer
        {
            get { return cards; }
        }
    }
}
