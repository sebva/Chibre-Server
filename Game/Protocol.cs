using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace Chibre_Server.Game
{
    class Protocol
    {
        #region Incoming

        public static void Hello(JsonObject data, Connection connection)
        {
            string uuid = data.GetNamedString("uuid");
            Guid guid;
            bool ok = Guid.TryParse(uuid, out guid);
            if (ok)
            {
                int playerId = ConnectionManager.Instance.OnHelloReceived(guid, connection);
                if (playerId != -1)
                    HelloReply(connection, playerId);
                else
                    Refusal(connection);
            }
            else
                Debug.WriteLine("Invalid GUID");
        }

        public static void ChooseAtout(JsonObject data, Connection connection)
        {
            string color = data.GetNamedString("color");
            if(color == "chibre")
            {
                connection.Player.ChooseAtoutChibrer();
            }
            else
            {
                StringBuilder colorBuilder = new StringBuilder(color);
                colorBuilder[0] = char.ToUpper(colorBuilder[0]);
                Color atout = (Color)Enum.Parse(typeof(Color), colorBuilder.ToString());
                connection.Player.ChooseAtout(atout);
            }
        }

        public static void PlayCard(JsonObject data, Connection connection)
        {
            Card playedCard = JsonObjectToCard(data.GetNamedObject("card"));
            connection.Player.PlayCard(playedCard);
        }

        #endregion

        #region Outgoing

        public static void HelloReply(Connection connection, int playerId)
        {
            JsonObject helloReply = new JsonObject();
            helloReply.SetNamedValue("action", JsonValue.CreateStringValue("hello_reply"));
            helloReply.SetNamedValue("player_no", JsonValue.CreateNumberValue(playerId - 1));
            connection.SendPayload(helloReply.Stringify());
        }

        public static void Refusal(Connection connection, string reason = "too_much_players")
        {
            JsonObject refusal = new JsonObject();
            refusal.SetNamedValue("action", JsonValue.CreateStringValue("refusal"));
            refusal.SetNamedValue("reason", JsonValue.CreateStringValue(reason));
            connection.SendPayload(refusal.Stringify());
        }

        public static void Distribution(Connection connection, bool atout, List<Card> cards)
        {
            JsonObject distribution = new JsonObject();
            distribution.SetNamedValue("action", JsonValue.CreateStringValue("distribution"));
            distribution.SetNamedValue("atout", JsonValue.CreateBooleanValue(atout));
            JsonArray cardsJson = new JsonArray();
            foreach(Card card in cards)
            {
                JsonObject cardJson = CardToJsonObject(card);
                cardsJson.Add(cardJson);
            }
            distribution.SetNamedValue("cards", cardsJson);
            connection.SendPayload(distribution.Stringify());
        }

        public static void TimeToPlay(Connection connection, List<Card> possibleCards)
        {
            JsonObject timeToPlay = new JsonObject();
            timeToPlay.SetNamedValue("action", JsonValue.CreateStringValue("time_to_play"));
            JsonArray possibleCardsJson = new JsonArray();
            foreach(Card card in possibleCards)
            {
                possibleCardsJson.Add(CardToJsonObject(card));
            }
            timeToPlay.SetNamedValue("possible_cards", possibleCardsJson);

            connection.SendPayload(timeToPlay.Stringify());
        }

        public static void GoodBye(Connection connection)
        {
            JsonObject goodBye = new JsonObject();
            goodBye.SetNamedValue("action", JsonValue.CreateStringValue("goodbye"));

            connection.SendPayload(goodBye.Stringify());
        }

        #endregion

        public static Card JsonObjectToCard(JsonObject data)
        {
            StringBuilder colorBuilder = new StringBuilder(data.GetNamedString("color"));
            colorBuilder[0] = Char.ToUpper(colorBuilder[0]);
            Color color = (Color) Enum.Parse(typeof(Color), colorBuilder.ToString());

            StringBuilder valueBuilder = new StringBuilder(data.GetNamedString("value"));
            valueBuilder[0] = Char.ToUpper(valueBuilder[0]);
            Value value = (Value) Enum.Parse(typeof(Value), valueBuilder.ToString());

            return Card.CardInstance(color, value);
        }

        public static JsonObject CardToJsonObject(Card card)
        {
            string color = card.Color.ToString().ToLower();
            string value = card.Value.ToString().ToLower();

            JsonObject json = new JsonObject();
            json.SetNamedValue("color", JsonValue.CreateStringValue(color));
            json.SetNamedValue("value", JsonValue.CreateStringValue(value));
            return json;
        }
    }
}
