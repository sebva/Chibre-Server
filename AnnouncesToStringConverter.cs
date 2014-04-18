using Chibre_Server.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Text;
using Windows.UI.Xaml.Data;

namespace Chibre_Server
{
    public class AnnouncesToStringConverter : IValueConverter
    {
        private ResourceLoader loader = new ResourceLoader();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            IEnumerable<Announce> announces = value as IEnumerable<Announce>;
            if(value != null)
            {
                loader.GetString("Team1Members");
                string[] announcesString = new string[announces.Count()];
                int i = 0;
                foreach(Announce announce in announces)
                {
                    string announceString = "";
                    switch(announce.AnnounceType)
                    {
                        case AnnounceType.Twenty:
                        case AnnounceType.Fifty:
                        case AnnounceType.HundredFollow:
                            announceString = loader.GetString(announce.AnnounceType.ToString()) +
                                " " +
                                loader.GetString(announce.HighestCard.Value.ToString()) +
                                " " +
                                loader.GetString("Of") +
                                " " +
                                loader.GetString(announce.HighestCard.Color.ToString());
                            break;
                        case AnnounceType.HundredSame:
                        case AnnounceType.HundredAndFifty:
                        case AnnounceType.TwoHundred:
                            announceString = loader.GetString(announce.AnnounceType.ToString()) +
                                " " +
                                loader.GetString(announce.HighestCard.Value.ToString() + "Plural");
                            break;
                    }
                    announcesString[i++] = announceString;
                }
                return string.Join("\n", announcesString);
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
