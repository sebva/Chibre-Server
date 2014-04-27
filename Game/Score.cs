using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;

namespace Chibre_Server.Game
{
    class Score : INotifyPropertyChanged
    {
        private List<Pair<Object, int>> categories;
        public event PropertyChangedEventHandler PropertyChanged;
        private int turnTotPoint;

        public Score()
        {
            turnTotPoint = 0;
            categories = new List<Pair<Object, int>>();
            categories.Add(new Pair<Object, int>(0, 100));
            categories.Add(new Pair<Object, int>(0, 50));
            categories.Add(new Pair<Object, int>(0, 20));
            categories.Add(new Pair<Object, int>(0, 1));
        }

        /// <summary>
        /// Add the points to the score
        /// </summary>
        /// <param name="points"></param>
        public void AddPoints(int points, bool addToGlobalScoreDirectly = false)
        {
            if (GameEngine.Instance.ShouldDoublePoints)
                points *= 2;

            if(addToGlobalScoreDirectly)
            {
                Addition(points);
                Reduce();
                NotifyScoreChanged();
            }
            else
                turnTotPoint += points;
        }

        /// <summary>
        /// Compute the score for the scoreboard
        /// </summary>
        public void ComputeScore()
        {
            Addition(turnTotPoint);
            Reduce();
            NotifyScoreChanged();
            turnTotPoint = 0;
        }

        /// <summary>
        /// Add the points to the different categories
        /// </summary>
        /// <param name="points"></param>
        private void Addition(int points)
        {
            foreach (Pair<Object, int> tuple in categories)
            {
                int item = (int)tuple.First;
                int ratio = points / tuple.Second;
                item += ratio;
                points -= ratio * tuple.Second;
                tuple.First = item;
            }
        }

        /// <summary>
        /// Reduce if the points are higher than the categories
        /// </summary>
        private void Reduce()
        {
            int o = categories.Count - 1;
            int t = o - 1;

            int currentItem = (int)categories[o].First;
            int nextItem = (int)categories[t].First;

            int ratio = currentItem / categories[t].Second;
            nextItem += ratio;
            currentItem -= ratio * categories[t].Second;

            categories[o].First = currentItem;
            categories[t].First = nextItem;
        }
        public void NotifyScoreChanged()
        {
            if (PropertyChanged != null)
            {
                GamePage.LatestDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                   () =>
                   {
                       PropertyChanged(this, new PropertyChangedEventArgs("Score"));
                   }).AsTask().Wait();
            }
        }

        /// <summary>
        /// Check if it's a match
        /// </summary>
        /// <returns></returns>
        public bool IsMatch()
        {
            return turnTotPoint == 157;
        }

        #region Properties
        public int Twenty
        {
            get { return (int)categories[2].First; }
        }

        public int Fifty
        {
            get { return (int)categories[1].First; }
        }

        public int Hundred
        {
            get { return (int)categories[0].First; }
        }

        public int One
        {
            get { return (int)categories[3].First; }
        }

        public int TotalPoints
        {
            get
            {
                return One + Twenty * 20 + Fifty * 50 + Hundred * 100;
            }
        }
        #endregion
    }
}
