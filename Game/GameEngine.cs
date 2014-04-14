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
        }

        public static GameEngine Instance
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
            DistributeCards();
            if (++gameNumber == 1) // The atout is the player with the 7 of diamonds
                foreach (KeyValuePair<int, Player> entry in players)
                    if(entry.Value.Cards.Contains(Card.CardInstance(Color.Carreau, Value.Seven)))
                        atoutPlayer = entry.Key;
            else
                atoutPlayer = (atoutPlayer + 1) % (teams.Length * teams[0].Length);
            playerTurn = atoutPlayer;
        }

        public void ChooseAtout(Color atout)
        {
            this.atout = atout;
            ManageAnnounces();
            SendCards();
        }

        public void AddCardTable(Card card, Player player)
        {
            if (player.Id == playerTurn)
            {
                ++playerTurnNumber;
                table.AddCard(playerTurn, card);
                if (playerTurnNumber == 4)
                {
                    playerTurnNumber = 0;
                    FinishTheTurn();
                }
            }
        }

        private void SendCards()
        {
            players[playerTurn].LegalCards(LegalCards(players[playerTurn]));
            playerTurn = (playerTurn + 1) % (teams.Length * teams[0].Length);
        }

        public void Chibrer()
        {
            players[(atoutPlayer + teams[0].Length) % players.Count].SendCards(true);
        }

        private void ManageAnnounces()
        {
            SearchAnnounce();
            if(announces.Count == 1)
                announces[0].Player.Team.Score.AddPoints(announces[0].Score);
            else if(announces.Count > 1)
            {
                announces.Sort(new Announce.AnnounceComparable()); //Desc order
                //In any case, the kept annouce is the first if equality or the highest and all the annouces of the team
                foreach (Announce announce in announces)
                    if (announce.Player.Team == announces[0].Player.Team)
                        announce.Player.Team.Score.AddPoints(announce.Score);
            }
            announces.Clear();
        }

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

            foreach(KeyValuePair<int, Player> pair in players)
            {
                SortedSet<Card> cards = new SortedSet<Card>(pair.Value.Cards);

                foreach(Pair<Value, AnnounceType> pair2 in sameCards)
                    if(FindSameCards(cards, pair2.First))
                        announces.Add(new Announce(pair2.Second, pair.Value, RemoveSameCard(ref cards, pair2.First)));
                 
                foreach(Pair<int, AnnounceType> pair2 in followCards)
                    if(FindFollowCards(cards, pair2.First))
                        foreach(List<Card> listCards in RemoveFollowCards(ref cards, pair2.First))
                            announces.Add(new Announce(pair2.Second, pair.Value, listCards));
            }
        }

        private bool FindFollowCards(SortedSet<Card> cardsOriginal, int serie)
        {
            List<Card> cards = new List<Card>(new SortedSet<Card>(cardsOriginal, new Card.CardValueComparer()));
            Debug.Assert(cardsOriginal.Count == 9);

            bool output = false;
            for (int i = 0; i <= (cards.Count - serie) && !output; ++i)
            {
                output = true;
                for (int j = i; j < i + serie; ++j)
                    output &= (cards[j + 1].Value - cards[j].Value == 1);
            }
            return output;
        }

        private List<List<Card>> RemoveFollowCards(ref SortedSet<Card> cardsOriginal, int serie)
        {
            List<Card> cards = new List<Card>(new SortedSet<Card>(cardsOriginal, new Card.CardValueComparer()));
            Debug.Assert(cardsOriginal.Count == 9);

            List<List<Card>> followCards = new List<List<Card>>();
            for (int i = 0; i <= (cards.Count - serie); ++i)
            {
                bool output = true;
                for (int j = i; j < i + serie; ++j)
                    output &= (cards[j + 1].Value - cards[j].Value == 1);
                if (output)
                {
                    List<Card> series = new List<Card>();
                    for (int j = i; j < i + serie; ++j)
                    {
                        series.Add(cards[j]);
                        cardsOriginal.Remove(cards[j]);
                    }
                }
            }
            return followCards;
        }

        private bool FindSameCards(SortedSet<Card> cards, Value value)
        {
            int count = 0;
            foreach (Card card in cards)
                if (card.Value == value)
                    ++count;
            return count == 4;
        }

        private List<Card> RemoveSameCard(ref SortedSet<Card> cards, Value value)
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
        public void AddPlayer(Player player)
        {
            Team team = teams[player.Id % 2];
            team.addPlayer(player);
            players.Add(player.Id, player);
            player.Team = team;
        }

        private void DistributeCards()
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

            foreach (KeyValuePair<int, Player> pair in players)
                if (pair.Value.Id != (atoutPlayer + teams[0].Length) % players.Count)
                    pair.Value.SendCards(pair.Value.Id == atoutPlayer);
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

            if(turnNumber == 9)
            {

            }

            ++turnNumber;
        }

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
            if (atoutCards.Count == cards.Count || atoutCards.Count > 0)
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
                    areAllCardsAtout &= card.Color == atout;

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
                        Card.AtoutComparer atoutComparer = new Card.AtoutComparer();
                        atoutCards.Sort(atoutComparer);

                        foreach (Pair<Card, bool> pair in legalCards)
                            if (pair.First.Color == atout && atoutComparer.Compare(pair.First, atoutCards[0]) > 0)
                                pair.Second = true;
                    }
                }
            }

            List<Card> output = new List<Card>();
            foreach (Pair<Card, bool> pair in legalCards)
                if (pair.Second)
                    output.Add(pair.First);

            return output;
        }

        #region Properties

        public Color Atout
        {
            get { return atout; }
        }

        public int AtoutPlayerId
        {
            get { return atoutPlayer; }
        }

        #endregion
    }
}
