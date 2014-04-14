using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chibre_Server.Game
{
    class Announce
    {
        #region Comparer
        /// <summary>
        ///  Sort annouce by power, desc order
        /// </summary>
        public class AnnounceComparable : IComparer<Announce>
        {
            private static Card.CardComparer cardComparer;

            static AnnounceComparable()
            {
                cardComparer = new Card.CardComparer();
            }

            public int Compare(Announce a1, Announce a2)
            {
                if (a1.Power < a2.Power)
                    return 1;
                else if (a1.Power > a2.Power)
                    return -1;
                else
                    return cardComparer.Compare(a1.HighestCard, a2.HighestCard);
            }
        }
        #endregion

        private AnnounceType announceType;
        private SortedSet<Card> cards;
        private Player player;
        private int score;
        private int power;

        /// <summary>
        /// Score for each type of announce
        /// </summary>
        private static readonly Dictionary<AnnounceType, int> scoreAnnouce;

        /// <summary>
        /// Power of an annouce. Highest score is the most powerful
        /// </summary>
        private static readonly Dictionary<AnnounceType, int> powerAnnounce;

        static Announce()
        {
            scoreAnnouce = new Dictionary<AnnounceType, int>();
            scoreAnnouce.Add(AnnounceType.TwoHundred, 200);
            scoreAnnouce.Add(AnnounceType.HundredAndFifty, 150);
            scoreAnnouce.Add(AnnounceType.HundredFollow, 100);
            scoreAnnouce.Add(AnnounceType.HundredSame, 100);
            scoreAnnouce.Add(AnnounceType.Fifty, 50);
            scoreAnnouce.Add(AnnounceType.Twenty, 20);

            powerAnnounce = new Dictionary<AnnounceType, int>();
            powerAnnounce.Add(AnnounceType.TwoHundred, 6);
            powerAnnounce.Add(AnnounceType.HundredAndFifty, 5);
            powerAnnounce.Add(AnnounceType.HundredFollow, 4);
            powerAnnounce.Add(AnnounceType.HundredSame, 3);
            powerAnnounce.Add(AnnounceType.Fifty, 2);
            powerAnnounce.Add(AnnounceType.Twenty, 1);
        }

        public Announce(AnnounceType announceType, Player player, List<Card> cards)
        {
            this.announceType = announceType;
            this.player = player;
            this.cards = new SortedSet<Card>(cards, new Card.CardComparer());
            score = scoreAnnouce[announceType];
            power = powerAnnounce[announceType];
        }

        #region Properties
        public Player Player
        {
            get { return player; }
        }

        public int Score
        {
            get { return score; }
        }

        private Card HighestCard
        {
            get { return cards.Last(); }
        }

        private int Power
        {
            get { return power; }
        }
        #endregion
    }
}
