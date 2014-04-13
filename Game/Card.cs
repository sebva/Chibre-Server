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

        static Card()
        {
            cards = new Dictionary<Tuple<Color, Value>, Card>();
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
