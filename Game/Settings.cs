using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Chibre_Server.Game
{
    /// <summary>
    /// Global settings class
    /// </summary>
    class Settings : INotifyPropertyChanged
    {
        private static Settings instance = null;
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _piqueDouble;
        private int _pointsSimple;
        private int _pointsDouble;

        private Settings()
        {
            // Restore settings from persistent storage
            ApplicationDataContainer data = ApplicationData.Current.LocalSettings;
            object piqueDouble = data.Values["PiqueDouble"];
            if (piqueDouble is bool)
                PiqueDouble = (bool)piqueDouble;
            else
                PiqueDouble = true;

            object pointsSimple = data.Values["PointsSimple"];
            if (pointsSimple is int)
                PointsSimple = (int)pointsSimple;
            else
                PointsSimple = 1000;

            object pointsDouble = data.Values["PointsDouble"];
            if (pointsDouble is int)
                PointsDouble = (int)pointsDouble;
            else
                PointsDouble = 1500;

        }

        ~Settings()
        {
            SaveState();
        }

        public void SaveState()
        {
            ApplicationDataContainer data = ApplicationData.Current.LocalSettings;
            data.Values["PiqueDouble"] = PiqueDouble;
            data.Values["PointsSimple"] = PointsSimple;
            data.Values["PointsDouble"] = PointsDouble;
        }

        public static Settings GetInstance()
        {
            if (instance == null)
                instance = new Settings();

            return instance;
        }

        public bool PiqueDouble
        {
            get { return _piqueDouble; }
            set
            {
                _piqueDouble = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("PiqueDouble"));
            }
        }

        public int PointsSimple
        {
            get { return _pointsSimple; }
            set
            {
                _pointsSimple = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("PointsSimple"));
            }
        }

        public int PointsDouble
        {
            get { return _pointsDouble; }
            set
            {
                _pointsDouble = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("PointsDouble"));
            }
        }

        public int PointsCurrent
        {
            get
            {
                return PiqueDouble ? PointsDouble : PointsSimple;
            }
        }
    }
}
