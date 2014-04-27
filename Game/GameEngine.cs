using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;

namespace Chibre_Server.Game
{
    class GameEngine : INotifyPropertyChanged
    {
        private static GameEngine instance = null;
        private Table table;
        private Team[] teams;
        private Dictionary<int, Player> players;
        private List<Announce> announces;

        private Color atout;
        private int gameNumber;
        private int atoutPlayer;
        private int playerTurn;
        private int turnNumber;
        private int playerTurnNumber;

        public event PropertyChangedEventHandler PropertyChanged;
        private static readonly string[] announceProperties = new string[] { "AnnouncesPlayer1", "AnnouncesPlayer2", "AnnouncesPlayer3", "AnnouncesPlayer4" };
        private GameEngine()
        {
            teams = new Team[2];
            teams[0] = new Team(this);
            teams[1] = new Team(this);
            announces = new List<Announce>();

            table = new Table(this);
            players = new Dictionary<int, Player>();
            gameNumber = 0;
            atoutPlayer = 0;
            playerTurn = 0;
            turnNumber = 0;
            playerTurnNumber = 0;
            AtoutChoosen = false;
        }

        /// <summary>
        /// Singleton, return the instance
        /// </summary>
        public static GameEngine Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameEngine();

                return instance;
            }
        }

        public void ResetInstance()
        {
            foreach(KeyValuePair<int, Player> player in players)
            {
                Protocol.GoodBye(player.Value.Connection);
            }

            instance = null;
        }

        /// <summary>
        /// Start a new turn
        /// </summary>
        public void StartNewTurn()
        {
            turnNumber = 0;
            DistributeCardsLocal();

            if (++gameNumber == 1) // The atout is the player with the 7 of diamonds
            {
                foreach (KeyValuePair<int, Player> entry in players)
                    if (entry.Value.Cards.Contains(Card.CardInstance(Color.Carreau, Value.Seven)))
                        atoutPlayer = entry.Key;
            }
            else
                atoutPlayer = ComputeNextAtoutPlayer();
            playerTurn = atoutPlayer;

            DistributeCardsDevice();
        }

        /// <summary>
        /// Return the id of the next player to choose the atout
        /// </summary>
        /// <returns></returns>
        private int ComputeNextAtoutPlayer()
        {
            return (atoutPlayer + 1) % (teams.Length * teams[0].Length);
        }

        /// <summary>
        /// Return the id of the chibre player
        /// </summary>
        /// <returns></returns>
        private int ComputeChibrePlayer()
        {
            return ((atoutPlayer + players[atoutPlayer].Team.Length) % players.Count);
        }

        #region In-coming
        /// <summary>
        /// Choose the atout
        /// </summary>
        /// <param name="atout"></param>
        public void ChooseAtout(Color atout)
        {
            AtoutChoosen = true;
            this.atout = atout;
            NotifyPropertyChanged("Atout");
            SearchAnnounce();
            players[ComputeChibrePlayer()].SendCards(false);
            SendCards();
        }

        /// <summary>
        /// Add a card to the table from the player
        /// </summary>
        /// <param name="card"></param>
        /// <param name="player"></param>
        public void AddCardTable(Card card, Player player)
        {
            if (player.Id == playerTurn)
            {
                ++playerTurnNumber;
                table.AddCard(playerTurn, card);
                playerTurn = (playerTurn + 1) % players.Count();
                if (playerTurnNumber == 4)
                {
                    playerTurnNumber = 0;
                    FinishTheTurn();
                }
                else
                    SendCards();
            }
        }
        #endregion

        #region out-going
        /// <summary>
        /// Send the cards to the player
        /// </summary>
        private void SendCards()
        {
            players[playerTurn].LegalCards(LegalCards(players[playerTurn]));
        }

        /// <summary>
        /// Chibre
        /// </summary>
        public void Chibrer()
        {
            players[ComputeChibrePlayer()].SendCards(true);
        }
        #endregion

        #region Announce
        /// <summary>
        /// Compute the highest announces
        /// </summary>
        private void ManageAnnounces()
        {
            if(announces.Count == 1)
                announces[0].Player.Team.Score.AddPoints(announces[0].Score, true);
            else if(announces.Count > 1)
            {
                announces.Sort(new Announce.AnnounceComparable()); //Desc order
                //In any case, the kept annouce is the first if equality or the highest and all the annouces of the team
                foreach (Announce announce in new List<Announce>(announces))
                {
                    if (announce.Player.Team == announces[0].Player.Team)
                        announce.Player.Team.Score.AddPoints(announce.Score, true);
                    else
                        announces.Remove(announce);
                }
            }

            NotifyAnnouncesChanged();
        }

        /// <summary>
        /// Search the announces (follow and same)
        /// </summary>
        private void SearchAnnounce()
        {
            List<Pair<Value, AnnounceType>> sameCards = new List<Pair<Value, AnnounceType>>();
            sameCards.Add(new Pair<Value, AnnounceType>(Value.As, AnnounceType.HundredSame));
            sameCards.Add(new Pair<Value, AnnounceType>(Value.Roi, AnnounceType.HundredSame));
            sameCards.Add(new Pair<Value, AnnounceType>(Value.Dame, AnnounceType.HundredSame));
            sameCards.Add(new Pair<Value, AnnounceType>(Value.Ten, AnnounceType.HundredSame));
            sameCards.Add(new Pair<Value, AnnounceType>(Value.Valet, AnnounceType.TwoHundred));
            sameCards.Add(new Pair<Value, AnnounceType>(Value.Nine, AnnounceType.HundredAndFifty));

            List<Pair<int, AnnounceType>> followCards = new List<Pair<int, AnnounceType>>();
            followCards.Add(new Pair<int, AnnounceType>(5, AnnounceType.HundredFollow));
            followCards.Add(new Pair<int, AnnounceType>(4, AnnounceType.Fifty));
            followCards.Add(new Pair<int, AnnounceType>(3, AnnounceType.Twenty));

            IComparer<Card> comparer = new Card.CardComparer();

            foreach(KeyValuePair<int, Player> pair in players)
            {
                List<Card> cards = new List<Card>(pair.Value.Cards);
                cards.Sort(comparer);

                foreach(Pair<Value, AnnounceType> pair2 in sameCards)
                    if(FindSameCards(cards, pair2.First))
                        announces.Add(new Announce(pair2.Second, pair.Value, RemoveSameCard(ref cards, pair2.First)));
                 
                foreach(Pair<int, AnnounceType> pair2 in followCards)
                    if(FindFollowCards(cards, pair2.First))
                        foreach(List<Card> listCards in RemoveFollowCards(ref cards, pair2.First))
                            announces.Add(new Announce(pair2.Second, pair.Value, listCards));
            }
        }

        /// <summary>
        /// Find the cards are followed
        /// </summary>
        /// <param name="cardsOriginal"></param>
        /// <param name="serie"></param>
        /// <returns></returns>
        private bool FindFollowCards(List<Card> cardsOriginal, int serie)
        {
            List<Card> cards = new List<Card>(cardsOriginal);

            for (int i = 0; i <= (cards.Count - serie); ++i)
            {
                bool output = true;
                for (int j = 0; j < serie - 1; ++j)
                    output &= CardsAreFollowed(cards[i + j + 1], cards[i + j]);
                if (output)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check if 2 cards are followed
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        private bool CardsAreFollowed(Card c1, Card c2)
        {
            return c1.Color == c2.Color && (c1.Value - c2.Value) == 1;
        }

        /// <summary>
        /// Remove the follow cards, they cannot be used in another announce
        /// </summary>
        /// <param name="cardsOriginal"></param>
        /// <param name="serie"></param>
        /// <returns></returns>
        private List<List<Card>> RemoveFollowCards(ref List<Card> cardsOriginal, int serie)
        {
            List<Card> cards = new List<Card>(cardsOriginal);

            List<List<Card>> followCards = new List<List<Card>>();
            for (int i = 0; i <= (cards.Count - serie); ++i)
            {
                bool output = true;
                for (int j = 0; j < serie - 1; ++j)
                    output &= CardsAreFollowed(cards[i + j + 1], cards[i + j]);
                if (output)
                {
                    List<Card> series = new List<Card>();
                    for (int j = i; j < i + serie; ++j)
                    {
                        series.Add(cards[j]);
                        cardsOriginal.Remove(cards[j]);
                    }
                    followCards.Add(series);
                }
            }
            return followCards;
        }

        /// <summary>
        /// Find same cards
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool FindSameCards(List<Card> cards, Value value)
        {
            int count = 0;
            foreach (Card card in cards)
                if (card.Value == value)
                    ++count;
            return count == 4;
        }

        /// <summary>
        /// Remove the same cards, they cannot be used in another announce
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private List<Card> RemoveSameCard(ref List<Card> cards, Value value)
        {
            List<Card> announceCards = new List<Card>();
            announceCards.Add(Card.CardInstance(Color.Pique, value));
            announceCards.Add(Card.CardInstance(Color.Coeur, value));
            announceCards.Add(Card.CardInstance(Color.Carreau, value));
            announceCards.Add(Card.CardInstance(Color.Trefle, value));

            cards.Remove(Card.CardInstance(Color.Pique, value));
            cards.Remove(Card.CardInstance(Color.Coeur, value));
            cards.Remove(Card.CardInstance(Color.Carreau, value));
            cards.Remove(Card.CardInstance(Color.Trefle, value));

            return announceCards;
        }
        #endregion

        /// <summary>
        /// Add a player the game
        /// </summary>
        /// <param name="player"></param>
        public void AddPlayer(Player player)
        {
            Team team = teams[player.Id % 2];
            team.addPlayer(player);
            players.Add(player.Id, player);
            player.Team = team;
        }

        /// <summary>
        /// Distribute the card locally (there are not send because the chibre player doesn't receive his card at the same time the other)
        /// </summary>
        private void DistributeCardsLocal()
        {
            List<Card> cards = new List<Card>();
            foreach (Value value in (Value[])Enum.GetValues(typeof(Value)))
                foreach (Color color in (Color[])Enum.GetValues(typeof(Color)))
                    cards.Add(Card.CardInstance(color, value));
            Utils.Shuffle(ref cards);

            int n = cards.Count / 4;
            for(int i = 0; i < n; ++i)
                foreach (KeyValuePair<int, Player> entry in players)
                {
                    entry.Value.AddCard(cards.Last());
                    cards.RemoveAt(cards.Count-1);
                }

            Task task = Task.Delay(600);
            task.Wait();
        }

        /// <summary>
        /// Send the cards to the devices
        /// </summary>
        private void DistributeCardsDevice()
        {
            foreach (KeyValuePair<int, Player> pair in players)
                if (pair.Value.Id != ((atoutPlayer + players[atoutPlayer].Team.Length) % players.Count))
                    pair.Value.SendCards(pair.Value.Id == atoutPlayer);
        }

        /// <summary>
        /// Finish the turn a compute the point and the winner
        /// </summary>
        private void FinishTheTurn()
        {
            List<Card> cards = table.Cards;
            List<Pair<Card, int>> cardsByPlayer = table.CardsByPlayer;

            Card card = WhichCardDoesWin(cards);
            Player winner = null;
            foreach(Pair<Card, int> pair in cardsByPlayer)
                if(pair.First == card)
                    winner = players[pair.Second];

            winner.Team.Score.AddPoints(ComputePointsTurn(cards));
            table.Clear();

            playerTurn = winner.Id;
            if (turnNumber == 0)
                ManageAnnounces();
            else if (turnNumber == 1)
            {
                announces.Clear();
                NotifyAnnouncesChanged();
            }
            if (++turnNumber < 9)
            {
                if (IsGameFinished())
                    NotifyPropertyChanged("Winner");
                else
                    SendCards();
            }
            else
            {
                winner.Team.Score.AddPoints(5); // 5 de der
                if (winner.Team.Score.IsMatch())
                    winner.Team.Score.AddPoints(100); // Add match
                foreach (Team team in teams)
                    team.Score.ComputeScore();

                if (IsGameFinished())
                    NotifyPropertyChanged("Winner");
                else
                    StartNewTurn();
            }
        }

        internal bool IsGameFinished()
        {
            bool gameFinished = false;
            foreach (Team team in teams)
            {
                if (team.Score.TotalPoints >= Settings.GetInstance().PointsCurrent)
                {
                    gameFinished = true;
                }
            }
            return gameFinished;
        }

        /// <summary>
        /// Find the highest card
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        private Card WhichCardDoesWin(List<Card> cards)
        {
            List<Card> atoutCards = new List<Card>();
            List<Card> colorCards = new List<Card>();
            foreach(Card card in cards)
            {
                if (card.Color == atout)
                    atoutCards.Add(card);
                if (card.Color == cards[0].Color)
                    colorCards.Add(card);
            }

            // If all the cards are atout or someone has maybe cut, return the highest score among the atout card
            if (atoutCards.Count > 0)
                return MostPowerfulCard(atoutCards, true);
            // We take the first color and get the highest same color cards
            else 
                return MostPowerfulCard(colorCards, false);
        }

        /// <summary>
        /// Compute the most powerful card
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="isAtout"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Compute the points
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        private int ComputePointsTurn(List<Card> cards)
        {
            int sum = 0;
            foreach (Card card in cards)
                sum += Card.ScoreCard(card, card.Color == atout);
            return sum;
        }

        /// <summary>
        /// Compute the legals cards
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private List<Card> LegalCards(Player player)
        {
            List<Pair<Card, Object>> legalCards = new List<Pair<Card, Object>>();
            foreach (Card card in player.Cards)
                legalCards.Add(new Pair<Card, Object>(card, false));

            // The player is the first player
            if (table.Length == 0)
                foreach (Pair<Card, Object> pair in legalCards)
                    pair.Second = true;
            else
            {
                bool areAllCardsAtout = true;
                foreach (Card card in table.Cards)
                    areAllCardsAtout &= card.Color == atout;

                if(areAllCardsAtout)
                {
                    int count = 0;
                    Card card = null;
                    // Enable all atout cards
                    foreach (Pair<Card, Object> pair in legalCards)
                        if (pair.First.Color == atout)
                        {
                            pair.Second = true;
                            ++count;
                            card = pair.First;
                        }

                    // If we have only the buur, we can bluff or if we don't have any atout, we can use all the cards
                    if (count == 0 || count == 1 && card.Value == Value.Valet)
                        foreach (Pair<Card, Object> pair in legalCards)
                            pair.Second = true;
                }
                else
                {
                    Color color = table.FirstCardColor();

                    int count = 0;
                    // Enable all same color cards
                    foreach (Pair<Card, Object> pair in legalCards)
                        if(pair.First.Color == color)
                        {
                            pair.Second = true;
                            ++count;
                        }

                    // If we don't have any cards of this color, we enable all non-atout cards
                    if (count == 0)
                        foreach (Pair<Card, Object> pair in legalCards)
                            if(pair.First.Color != atout)
                                pair.Second = true;

                    // If some has cut, we have to find the highest cut card and enable all higher atout cards
                    List<Card> atoutCards = new List<Card>();
                    foreach (Card card in table.Cards)
                        if (card.Color == atout)
                            atoutCards.Add(card);

                    // Someone has cut, find the highest card to enable the correct atout cards
                    Card.AtoutComparer atoutComparer = new Card.AtoutComparer();
                    atoutCards.Sort(atoutComparer);

                    // Enable all higher atout cards (or all if nobody has cut)
                    foreach (Pair<Card, Object> pair in legalCards)
                        if (pair.First.Color == atout && (atoutCards.Count == 0 || atoutComparer.Compare(pair.First, atoutCards[0]) < 0))
                            pair.Second = true;

                    // If our last cards are only atout and below the cut card, we have to play with
                    bool canPlay = true;
                    foreach (Pair<Card, Object> pair in legalCards)
                        canPlay |= (bool)pair.Second;

                    if(!canPlay)
                        foreach (Pair<Card, Object> pair in legalCards)
                            if (pair.First.Color == atout)
                                pair.Second = true;
                }
            }

            List<Card> output = new List<Card>();
            foreach (Pair<Card, Object> pair in legalCards)
                if (((bool)pair.Second))
                    output.Add(pair.First);

            return output;
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                GamePage.LatestDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                   () =>
                   {
                       PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                   }).AsTask().Wait();
            }
        }

        private void NotifyAnnouncesChanged()
        {
            foreach(string announceProperty in announceProperties)
                NotifyPropertyChanged(announceProperty);
        }

        private IEnumerable<Announce> AnnouncesForPlayer(int playerId)
        {
            // LINQ !
            return from announce in announces where announce.Player.Id == playerId select announce;
        }

        #region Properties

        public Boolean AtoutChoosen
        {
            get;
            private set;
        }

        public Color Atout
        {
            get { return atout; }
        }

        public int AtoutPlayerId
        {
            get { return atoutPlayer; }
        }

        public Table Table
        {
            get { return table; }
        }

        public Team Team1
        {
            get { return teams[0]; }
        }
        public Team Team2
        {
            get { return teams[1]; }
        }

        public IEnumerable<Announce> AnnouncesPlayer1
        {
            get { return AnnouncesForPlayer(0); }
        }
        public IEnumerable<Announce> AnnouncesPlayer2
        {
            get { return AnnouncesForPlayer(1); }
        }

        public IEnumerable<Announce> AnnouncesPlayer3
        {
            get { return AnnouncesForPlayer(2); }
        }

        public IEnumerable<Announce> AnnouncesPlayer4
        {
            get { return AnnouncesForPlayer(3); }
        }

        public bool ShouldDoublePoints
        {
            get
            {
                return Settings.GetInstance().PiqueDouble && atout == Color.Pique;
            }
        }

        #endregion
    }
}
