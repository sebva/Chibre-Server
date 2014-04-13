using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Chibre_Server.Game
{
    class Player
    {
        private Connection connection;
        private SortedSet<Card> cards;
        private Team team;
        private int id;

        private class CardComparer : IComparer<Card>
        {
            public int Compare(Card c1, Card c2)
            {
                if (c1.Color < c2.Color)
                    return -1;
                else if (c1.Color > c2.Color)
                    return 1;
                else if (c1.Value < c2.Value)
                    return -1;
                else if (c1.Value > c2.Value)
                    return 1;
                else
                    return 0;
            }
        }

        public Player(int id, ref Team team, ref Connection connection)
        {
            this.id = id;
            this.team = team;
            this.connection = connection;
            this.cards = new SortedSet<Card>(new CardComparer());
        }

        public Team Team
        {
            get { return team; }
        }

        public SortedSet<Card> Cards
        {
            get { return this.cards; }
        }

        public int Id
        {
            get { return id; }
        }

        public void Announce(AnnounceType a)
        {

        }

        public void AddCard(Card card)
        {
            Debug.Assert(cards.Count <= 9);
            cards.Add(card);
        }

        public void PlayCard(Card card)
        {
            cards.Remove(card);
            //team.GameEngine.
        }

        public void LegalCards(List<Card> cards)
        {
            //TODO : Send to the device
        }
    }
}
