using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chibre_Server.Game
{
    class Card
    {
        public class AtoutComparable : IComparer<Card>
        {
            private static readonly List<Value> values;

            static AtoutComparable()
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
                if (values.IndexOf(c1.Value) < values.IndexOf(c2.Value))
                    return 1;
                else if (values.IndexOf(c1.Value) > values.IndexOf(c2.Value))
                    return -1;
                else
                    return 0;
            }
        }

        private static Dictionary<Tuple<Color, Value>, Card> cards;
        private static readonly Dictionary<Value, int> scoreNormalCards;
        private static readonly Dictionary<Value, int> scoreAtoutCards;

        static Card()
        {
            cards = new Dictionary<Tuple<Color, Value>, Card>();
            scoreNormalCards = new Dictionary<Value, int>();
            scoreAtoutCards = new Dictionary<Value, int>();

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
        }

        public static int ScoreCard(Card card, bool isAtout)
        {
            return isAtout ? scoreAtoutCards[card.Value] : scoreNormalCards[card.Value];
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
            this.Color = color;
            this.Value = value;
        }

        public Color Color
        {
            private set;
            get;
        }

        public Value Value
        {
            private set;
            get;
        }
    }
}
