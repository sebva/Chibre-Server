using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chibre_Server.Game
{
    class Card
    {
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
                cards[Tuple.Create<Color, Value>(color, value)] =  card;
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
