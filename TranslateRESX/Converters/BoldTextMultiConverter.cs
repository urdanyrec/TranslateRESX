using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows;

namespace TranslateRESX.Converters
{
    public class BoldTextMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is string text && values[1] is string wordToBold)
            {
                var textBlock = new TextBlock 
                { 
                    TextWrapping = TextWrapping.Wrap 
                };

                int index = text.IndexOf(wordToBold, StringComparison.OrdinalIgnoreCase);
                if (index >= 0)
                {
                    textBlock.Inlines.Add(new Run(text.Substring(0, index)));
                    textBlock.Inlines.Add(new Run(text.Substring(index, wordToBold.Length))
                    {
                        FontWeight = FontWeights.Bold
                    });
                    textBlock.Inlines.Add(new Run(text.Substring(index + wordToBold.Length)));
                }
                else
                {
                    textBlock.Inlines.Add(new Run(text));
                }

                return textBlock;
            }

            return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
