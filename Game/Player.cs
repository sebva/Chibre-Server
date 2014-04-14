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
        public delegate void AtoutChoosen();

        private Connection connection;
        private SortedSet<Card> cards;
        private Team team;
        private int id;

        public Player(int id, ref Connection connection)
        {
            this.id = id;
            this.connection = connection;
            this.cards = new SortedSet<Card>(new Card.CardComparer());
        }

        /// <summary>
        /// Give the possibity to choose atout
        /// </summary>
        /// <param name="atout">Things</param>
        public void ChooseAtout(Color atout)
        {
            team.GameEngine.ChooseAtout(atout);
        }

        public AtoutChoosen AtoutChoosenDelegate
        {
            get;
            set;
        }

        public void ChooseAtoutChibrer()
        {
            team.GameEngine.Chibrer();
        }

        public Connection Connection
        {
            get { return connection; }
        }

        public void AddCard(Card card)
        {
            Debug.Assert(cards.Count <= 9);
            cards.Add(card);
        }

        public void SendCards(bool shouldChooseAtout)
        {
            Debug.WriteLine("Sending cards to device");
            Protocol.Distribution(connection, shouldChooseAtout, new List<Card>(cards));
        }

        public void PlayCard(Card card)
        {
            cards.Remove(card);
            team.GameEngine.AddCardTable(card);
        }

        public void LegalCards(List<Card> cards)
        {
            Protocol.TimeToPlay(connection, cards);
        }

        #region Properties

        public Team Team
        {
            get { return team; }
            set { team = value; }
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
