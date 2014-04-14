using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chibre_Server.Game
{
    class Card
    {
        #region AtoutComparer
        /// <summary>
        /// Sort atout cards by power, asc order
        /// </summary>
        public class AtoutComparer : IComparer<Card>
        {
            private static readonly List<Value> values;

            static AtoutComparer()
            {
                values = new List<Value>();
                values.Add(Value.Valet);
                values.Add(Value.Nine);
                values.Add(Value.As);
                values.Add(Value.Roi);
                values.Add(Value.Dame);
                values.Add(Value.Ten);
                values.Add(Value.Eight);
                values.Add(Value.Seven);
                values.Add(Value.Six);
            }

            public int Compare(Card c1, Card c2)
            {
                return -values.IndexOf(c1.Value).CompareTo(values.IndexOf(c2.Value));
            }
        }
        #endregion

        #region CardComparer
        /// <summary>
        /// Sort card by color and value, asc order
        /// </summary>
        public class CardComparer : IComparer<Card>
        {
            public static readonly List<Value> values;
            private static readonly List<Color> colors;

            static CardComparer()
            {
                values = new List<Value>();
                values.Add(Value.As);
                values.Add(Value.Roi);
                values.Add(Value.Dame);
                values.Add(Value.Valet);
                values.Add(Value.Ten);
                values.Add(Value.Nine);
                values.Add(Value.Eight);
                values.Add(Value.Seven);
                values.Add(Value.Six);

                colors = new List<Color>();
                colors.Add(Color.Coeur);
                colors.Add(Color.Pique);
                colors.Add(Color.Carreau);
                colors.Add(Color.Trefle);
            }

            public int Compare(Card c1, Card c2)
            {
                int color = colors.IndexOf(c1.Color).CompareTo(colors.IndexOf(c2.Color));

                if (color != 0)
                    return color;
                else
                    return -values.IndexOf(c1.Value).CompareTo(values.IndexOf(c2.Value));
            }
        }
        #endregion

        #region CardValueComparer
        /// <summary>
        /// Sort card by value, asc order
        /// </summary>
        public class CardValueComparer : IComparer<Card>
        {
            public int Compare(Card c1, Card c2)
            {
                return -CardComparer.values.IndexOf(c1.Value).CompareTo(CardComparer.values.IndexOf(c2.Value));
            }
        }
        #endregion

        /// <summary>
        /// MultiSeton (Singleton Idiom)
        /// </summary>
        private static Dictionary<Tuple<Color, Value>, Card> cards;

        /// <summary>
        /// (Value Card, isAtout) => Power
        /// </summary>
        private static readonly Dictionary<Tuple<Value, bool>, int> powerCards;

        private static readonly Dictionary<Value, int> scoreNormalCards;
        private static readonly Dictionary<Value, int> scoreAtoutCards;

        private Color color;
        private Value value;

        static Card()
        {
            cards = new Dictionary<Tuple<Color, Value>, Card>();
            scoreNormalCards = new Dictionary<Value, int>();
            scoreAtoutCards = new Dictionary<Value, int>();
            powerCards = new Dictionary<Tuple<Value,bool>,int>();

            scoreNormalCards.Add(Value.Six, 0);
            scoreNormalCards.Add(Value.Seven, 0);
            scoreNormalCards.Add(Value.Eight, 0);
            scoreNormalCards.Add(Value.Nine, 0);
            scoreNormalCards.Add(Value.Ten, 10);
            scoreNormalCards.Add(Value.Valet, 2);
            scoreNormalCards.Add(Value.Dame, 3);
            scoreNormalCards.Add(Value.Roi, 4);
            scoreNormalCards.Add(Value.As, 11);

            scoreAtoutCards.Add(Value.Six, scoreNormalCards[Value.Six]);
            scoreAtoutCards.Add(Value.Seven, scoreNormalCards[Value.Seven]);
            scoreAtoutCards.Add(Value.Eight, scoreNormalCards[Value.Eight]);
            scoreAtoutCards.Add(Value.Nine, 14);
            scoreAtoutCards.Add(Value.Ten, scoreNormalCards[Value.Ten]);
            scoreAtoutCards.Add(Value.Valet, 20);
            scoreAtoutCards.Add(Value.Dame, scoreNormalCards[Value.Dame]);
            scoreAtoutCards.Add(Value.Roi, scoreNormalCards[Value.Roi]);
            scoreAtoutCards.Add(Value.As, scoreNormalCards[Value.As]);

            powerCards.Add(new Tuple<Value,bool>(Value.Valet, true), 18);
            powerCards.Add(new Tuple<Value,bool>(Value.Nine, true), 17);
            powerCards.Add(new Tuple<Value,bool>(Value.As, true), 16);
            powerCards.Add(new Tuple<Value,bool>(Value.Roi, true), 15);
            powerCards.Add(new Tuple<Value,bool>(Value.Dame, true), 14);
            powerCards.Add(new Tuple<Value,bool>(Value.Ten, true), 13);
            powerCards.Add(new Tuple<Value,bool>(Value.Eight, true), 12);
            powerCards.Add(new Tuple<Value,bool>(Value.Seven, true), 11);
            powerCards.Add(new Tuple<Value,bool>(Value.Six, true), 10);

            powerCards.Add(new Tuple<Value,bool>(Value.As, false), 9);
            powerCards.Add(new Tuple<Value,bool>(Value.Roi, false), 8);
            powerCards.Add(new Tuple<Value,bool>(Value.Dame, false), 7);
            powerCards.Add(new Tuple<Value,bool>(Value.Valet, false), 6);
            powerCards.Add(new Tuple<Value,bool>(Value.Ten, false), 5);
            powerCards.Add(new Tuple<Value,bool>(Value.Nine, false), 4);
            powerCards.Add(new Tuple<Value,bool>(Value.Eight, false), 3);
            powerCards.Add(new Tuple<Value,bool>(Value.Seven, false), 2);
            powerCards.Add(new Tuple<Value,bool>(Value.Six, false), 1);
        }

        public static int ScoreCard(Card card, bool isAtout)
        {
            return isAtout ? scoreAtoutCards[card.Value] : scoreNormalCards[card.Value];
        }

        public static int PowerCard(Card card, bool isAtout)
        {
            return powerCards[Tuple.Create(card.Value, isAtout)];
        }

        public static Card CardInstance(Color color, Value value)
        {
            Tuple<Color, Value> tuple = Tuple.Create<Color, Value>(color, value);
            if(!cards.ContainsKey(tuple))
            {
                Card card = new Card(color, value);
                cards[tuple] =  card;
            }

            return cards[tuple];
        }

        private Card(Color color, Value value)
        {
            this.color = color;
            this.value = value;
        }

        #region Properties
        public Color Color
        {
            get { return color; }
        }

        public Value Value
        {
            get { return value; }
        }
        #endregion
    }
}
