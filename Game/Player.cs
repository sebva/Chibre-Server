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

        public Player(int id, ref Team team, ref Connection connection)
        {
            this.id = id;
            this.team = team;
            this.connection = connection;
            this.cards = new SortedSet<Card>(new Card.CardComparer());
            this.team.GameEngine.AddPlayer(this);
        }

        /// <summary>
        /// Give the possibity to choose atout
        /// </summary>
        /// <returns>True : has chosen, False : has "chibré" </returns>
        public bool ChooseAtout()
        {
            //TODO : Choose atout
            //team.GameEngine.ChooseAtout(...);
            return true;
        }

        public void ChooseAtoutChiber()
        {
            //TODO : Choose atout;
            //team.GameEngine.ChooseAtout(...);
        }

        public void Announce(Announce annouce)
        {
            team.GameEngine.AddAnnounce(annouce);
            //TODO : Call this method whith the communication
        }

        public void AddCard(Card card)
        {
            Debug.Assert(cards.Count <= 9);
            cards.Add(card);
            //TODO : Send to the device
        }

        public void PlayCard(Card card)
        {
            cards.Remove(card);
            team.GameEngine.AddCardTable(card);
        }

        public void LegalCards(List<Card> cards)
        {
            //TODO : Send to the device
        }

        #region Properties

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
        #endregion
    }
}
