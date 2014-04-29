using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;

namespace Chibre_Server.Game
{
    class Table : INotifyPropertyChanged
    {
        private List<Pair<Card, int>> cards;
        private GameEngine gameEngine;
        public event PropertyChangedEventHandler PropertyChanged;
        private string[] propertiesToNotify = { "Player1Card", "Player2Card", "Player3Card", "Player4Card" };

        public Table(GameEngine gameEngine)
        {
            this.gameEngine = gameEngine;
            cards = new List<Pair<Card, int>>();
        }

        /// <summary>
        /// Add the card of the player to the table
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="card"></param>
        public void AddCard(int playerId, Card card)
        {
            Debug.Assert(cards.Count <= 4);
            cards.Add(new Pair<Card, int>(card, playerId));
            NotifyCardsChanged();
        }

        /// <summary>
        /// Return the color of the first card in the table
        /// </summary>
        /// <returns>Color</returns>
        public Color FirstCardColor()
        {
            return cards[0].First.Color;
        }

        /// <summary>
        /// Clear the table
        /// </summary>
        public void Clear()
        {
            cards.Clear();
        }

        /// <summary>
        /// Return the card of the player
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns>Card</returns>
        private Card CardForPlayer(int playerId)
        {
            foreach(Pair<Card, int> card in cards)
            {
                if (card.Second == playerId)
                    return card.First;
            }
            return null;
        }

        public void NotifyCardsChanged()
        {
            if (PropertyChanged != null)
            {
                GamePage.LatestDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                   () =>
                   {
                       foreach (string propertyName in propertiesToNotify)
                           PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                   }).AsTask().Wait();
            }
        }

        #region Properties
        public int Length
        {
            get { return cards.Count; }
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

        public List<Pair<Card, int>> CardsByPlayer
        {
            get { return cards; }
        }

        // For UI binding
        public Card Player1Card
        {
            get { return CardForPlayer(0); }
        }

        public Card Player2Card
        {
            get { return CardForPlayer(1); }
        }

        public Card Player3Card
        {
            get { return CardForPlayer(2); }
        }

        public Card Player4Card
        {
            get { return CardForPlayer(3); }
        }
        
        #endregion
    }
}
