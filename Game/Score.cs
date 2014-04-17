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

        public void AddPoints(int points)
        {
            turnTotPoint += points;
        }

        public void ComputeScore()
        {
            Addition(turnTotPoint);
            Reduce();
            NotifyScoreChanged();
            turnTotPoint = 0;
        }

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

        private void Reduce()
        {
            for (int i = categories.Count - 1; i > 0; --i)
            {
                int currentItem = (int)categories[i].First;
                int nextItem = (int)categories[i - 1].First;

                int ratio = currentItem / categories[i - 1].Second;
                nextItem += ratio;
                currentItem -= ratio * categories[i - 1].Second;

                categories[i].First = currentItem;
                categories[i - 1].First = nextItem;
            }
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
        #endregion
    }
}
