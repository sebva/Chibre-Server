using Chibre_Server.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml.Data;

namespace Chibre_Server
{
    public class CardToImagePathConverter : IValueConverter
    {
        private const string kPath = "Assets/";
        private const string kSeparator = "_";
        private const string kExtension = ".png";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Card card = value as Card;
            if(card != null)
            {
                string cardColor = card.Color.ToString().ToLower();
                string cardValue = card.Value.ToString().ToLower();
                return kPath + cardColor + kSeparator + cardValue + kExtension;
            }
            else
            {
                return kPath + "card_empty" + kExtension;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
