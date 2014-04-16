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
        private int one;
        private int twenty;
        private int fifty;
        private int hundred;
        public event PropertyChangedEventHandler PropertyChanged;

        public Score()
        {
            this.one = this.twenty = this.fifty = this.hundred = 0;
            categories = new List<Pair<Object, int>>();
            categories.Add(new Pair<Object, int>(hundred, 100));
            categories.Add(new Pair<Object, int>(fifty, 50));
            categories.Add(new Pair<Object, int>(twenty, 20));
            categories.Add(new Pair<Object, int>(one, 1));
        }

        public void AddPoints(int points)
        {
            Addition(points);
            Reduce();
            NotifyScoreChanged();
        }

        private void Addition(int points)
        {
            foreach (Pair<Object, int> tuple in categories)
            {
                int item = (int)tuple.First;
                item += points / tuple.Second;
                points -= points / tuple.Second;
                tuple.First = item;
            }
        }

        private void Reduce()
        {
            for (int i = categories.Count - 1; i > 0; --i)
            {
                int currentItem = (int)categories[i].First;
                int nextItem = (int)categories[i - 1].First;

                int value = currentItem / categories[i - 1].Second;
                nextItem += value;
                currentItem -= value;

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
            get { return twenty; }
        }

        public int Fifty
        {
            get { return fifty; }
        }

        public int Hundred
        {
            get { return hundred; }
        }

        public int One
        {
            get { return one; }
        }
        #endregion
    }
}
