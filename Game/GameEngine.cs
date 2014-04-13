using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Chibre_Server.Game
{
    class GameEngine
    {
        private GameEngine instance = null;
        private Table table;
        private Team[] teams;
        private Dictionary<int, Player> players;

        private Color atout;
        private int gameNumber;
        private int atoutPlayer;
        private int playerTurn;

        private Random random;
        private GameEngine()
        {
            random = new Random();
            teams = new Team[2];
            teams[0] = new Team(this);
            teams[1] = new Team(this);
            
            table = new Table(this);
            players = new Dictionary<int, Player>();
            gameNumber = 0;
            atoutPlayer = 0;
            playerTurn = 0;
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

        public void StartNewTurn()
        {
            ++gameNumber;

            DistributeCards();

            if (gameNumber == 1) // The atout is the player with the 7 of diamonds
            {
                Card sevenDiamonds = Card.CardInstance(Color.Carreau, Value.Seven);
                foreach (KeyValuePair<int, Player> entry in players)
                    if(entry.Value.Cards.Contains(sevenDiamonds))
                    {
                        atoutPlayer = entry.Key;
                        break;
                    }
            }
            else
                atoutPlayer = (atoutPlayer + 1) % (teams.Length * teams[0].Length);
            playerTurn = atoutPlayer;

            for (int i = 0; i < 9; ++i)
                GameProcess();
        }

        public void AddPlayers(Player[] players)
        {
            Debug.Assert(players.Length == 4);

            for (int i = 0; i < players.Length; ++i)
            {
                Team team = teams[i % 2];
                team.addPlayer(players[i]);
                this.players.Add(players[i].Id, players[i]);
            }
        }

        public void AddPlayer(Player player)
        {
            Team team = teams[0];
            if (teams[0].Length == 1 && teams[1].Length == 0 || teams[0].Length == 2 && teams[1].Length == 1)
                team = teams[1];

            team.addPlayer(player);
            players.Add(player.Id, player);
        }

        public void AddCardTable(Card card)
        {
            table.AddCard(playerTurn, card);
            playerTurn = (playerTurn + 1) % (teams.Length * teams[0].Length);
            //TODO : Sync with GameProcess
        }

        private void DistributeCards()
        {
            List<Card> cards = new List<Card>();
            foreach (Value value in (Value[])Enum.GetValues(typeof(Value)))
                foreach (Color color in (Color[])Enum.GetValues(typeof(Color)))
                    cards.Add(Card.CardInstance(color, value));
            Shuffle(cards);

            int n = cards.Count;
            for(int i = 0; i < n; ++i)
                foreach (KeyValuePair<int, Player> entry in players)
                    {
                        entry.Value.AddCard(cards.Last());
                        cards.RemoveAt(cards.Count-1);
                    }
        }

        /// <summary>
        /// Use Fisher-Yates algorithm
        /// </summary>
        /// <param name="list">List to shuffle</param>
        /// <returns>Shuffled list</returns>
        private void Shuffle<T>(List<T> list)
        {
            for(int i = 0; i < list.Count; ++i)
            {
                int j = random.Next(i, list.Count-1);
                T foo = list[i];
                list[i] = list[j];
                list[j] = foo;
            }
        }

        private void GameProcess()
        {
            for(int i = 0; i < 4; ++i)
            {
                players[i].LegalCards(LegalCards(players[i]));
                // TODO : Sync with AddCardTable
            }
            FinishTheTurn();
        }

        private void FinishTheTurn()
        {
            List<Card> cards = table.Cards;
            List<Pair<Card, int>> cardsByPlayer = table.CardsByPlayer;

            Card card = WhichCardDoesWin(cards);
            Player winner = null;
            foreach(Pair<Card, int> pair in cardsByPlayer)
                if(pair.First == card)
                {
                    winner = players[pair.Second];
                    break;
                }
            winner.Team.Score.AddPoints(ComputePointsTurn(cards));
            table.Clear();

            playerTurn = winner.Id;
        }

        private Card WhichCardDoesWin(List<Card> cards)
        {
            bool areAllCardsAtout = true;
            List<Card> atoutCards = new List<Card>();
            List<Card> colorCards = new List<Card>();
            foreach(Card card in cards)
            {
                areAllCardsAtout &= card.Color == atout;
                if (card.Color == atout)
                    atoutCards.Add(card);
                if(card.Color == cards[0].Color)
                    colorCards.Add(card);
            }

            // If all the cards are atout or someone has maybe cut, return the highest score among the atout card
            if (areAllCardsAtout || atoutCards.Count > 0)
                return MostPowerfulCard(atoutCards, true);
            // We take the first color and get the highest same color cards
            else 
                return MostPowerfulCard(colorCards, false);
        }

        private Card MostPowerfulCard(List<Card> cards, bool isAtout)
        {
            Debug.Assert(cards.Count > 0);

            Card maxCard = cards[0];
            int maxScore = Card.PowerCard(maxCard, isAtout);

            foreach (Card card in cards)
            {
                int score = Card.PowerCard(card, isAtout);
                if (score > maxScore)
                {
                    maxScore = score;
                    maxCard = card;
                }
            }
            return maxCard;
        }
        private int ComputePointsTurn(List<Card> cards)
        {
            int sum = 0;
            foreach (Card card in cards)
                sum += Card.ScoreCard(card, card.Color == atout);
            return sum;
        }

        private List<Card> LegalCards(Player player)
        {
            List<Pair<Card, bool>> legalCards = new List<Pair<Card, bool>>();
            List<Card> cardsTable = table.Cards;
            foreach (Card card in player.Cards)
                legalCards.Add(new Pair<Card, bool>(card, false));

            // The player is the first player
            if (table.Length == 0)
                foreach (Pair<Card, bool> pair in legalCards)
                    pair.Second = true;
            else
            {
                bool areAllCardsAtout = true;
                foreach (Card card in cardsTable)
                {
                    areAllCardsAtout = card.Color == atout;
                    if (!areAllCardsAtout)
                        break;
                }

                if(areAllCardsAtout)
                {
                    int count = 0;
                    Card card = null;
                    // Enable all atout cards
                    foreach (Pair<Card, bool> pair in legalCards)
                        if (pair.First.Color == atout)
                        {
                            pair.Second = true;
                            ++count;
                            card = pair.First;
                        }

                    // If we have only the buur, we can bluff or if we don't have any atout, we can use all the cards
                    if (count == 0 || count == 1 && card.Value == Value.Valet)
                        foreach (Pair<Card, bool> pair in legalCards)
                            pair.Second = true;
                }
                else
                {
                    Color color = table.FirstCardColor();

                    int count = 0;
                    // Enable all same color cards
                    foreach (Pair<Card, bool> pair in legalCards)
                        if(pair.First.Color == color)
                        {
                            pair.Second = true;
                            ++count;
                        }

                    // If we don't have any cards of this color, we enable all cards
                    if (count == 0)
                        foreach (Pair<Card, bool> pair in legalCards)
                            pair.Second = true;
                    // We enable only the atout cards
                    else 
                        foreach (Pair<Card, bool> pair in legalCards)
                            pair.Second = pair.First.Color == atout;

                    // If some has cut, we have to find the highest cut card and disable all atout card below its value
                    List<Card> atoutCards = new List<Card>();
                    foreach (Card card in cardsTable)
                        if (card.Color == atout)
                            atoutCards.Add(card);

                    // Someone has cut, find the highest card
                    if(atoutCards.Count > 0)
                    {
                        Card.AtoutComparable atoutComparable = new Card.AtoutComparable();
                        atoutCards.Sort(atoutComparable);

                        foreach (Pair<Card, bool> pair in legalCards)
                            if (pair.First.Color == atout && atoutComparable.Compare(pair.First, atoutCards[0]) == 1)
                                pair.Second = true;
                    }
                }
            }

            List<Card> output = new List<Card>();
            foreach (Pair<Card, bool> pair in legalCards)
                if (pair.Second)
                    output.Add(pair.First);
            if(output.Count <= 0)
                foreach (Pair<Card, bool> pair in legalCards)
                    output.Add(pair.First);
            return output;
        }
    }
}
