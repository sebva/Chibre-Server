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
        /// Give the possibility to choose atout
        /// </summary>
        /// <param name="atout"></param>
        public void ChooseAtout(Color atout)
        {
            team.GameEngine.ChooseAtout(atout);
        }

        /// <summary>
        /// Give the possibility to choose atout for the other player
        /// </summary>
        public void ChooseAtoutChibrer()
        {
            team.GameEngine.Chibrer();
        }

        /// <summary>
        /// Add a card to the player
        /// </summary>
        /// <param name="card"></param>
        public void AddCard(Card card)
        {
            cards.Add(card);
        }

        /// <summary>
        /// The the cards to the device
        /// </summary>
        /// <param name="shouldChooseAtout"></param>
        public void SendCards(bool shouldChooseAtout)
        {
            Protocol.Distribution(connection, shouldChooseAtout, new List<Card>(cards));
        }

        /// <summary>
        /// Receive the player card from device
        /// </summary>
        /// <param name="card"></param>
        public void PlayCard(Card card)
        {
            cards.Remove(card);
            team.GameEngine.AddCardTable(card, this);
        }

        /// <summary>
        /// Send the legalcards
        /// </summary>
        /// <param name="cards"></param>
        public void LegalCards(List<Card> cards)
        {
            Protocol.TimeToPlay(connection, cards);
        }

        #region Properties
        public AtoutChoosen AtoutChoosenDelegate
        {
            get;
            set;
        }

        public Connection Connection
        {
            get { return connection; }
        }

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
