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
    public class ColorToImagePathConverter : IValueConverter
    {
        private const string kPath = "Assets/";
        private const string kExtension = ".png";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!GameEngine.Instance.AtoutChoosen)
                return kPath + "color_empty" + kExtension;
            else
            {
                Color color = (Color)value;
                return kPath + color.ToString().ToLower() + kExtension;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
