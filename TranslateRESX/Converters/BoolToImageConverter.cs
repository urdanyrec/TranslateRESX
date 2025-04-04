using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TranslateRESX.Converters
{
    public class BoolToImageConverter : IValueConverter
    {
        public BitmapImage TrueImage { get; set; }

        public BitmapImage FalseImage { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? TrueImage : FalseImage;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
