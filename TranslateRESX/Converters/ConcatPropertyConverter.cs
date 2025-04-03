using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace TranslateRESX.Converters
{
    public class ConcatPropertyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            var type = value.GetType();
            var propertyNames = parameter?.ToString()?.Split(',') ?? new string[0];
            var result = new StringBuilder();
            foreach (var propName in propertyNames)
            {
                var trimmedName = propName.Trim();
                if (string.IsNullOrEmpty(trimmedName))
                    continue;

                var property = type.GetProperty(trimmedName);
                if (property != null)
                {
                    var propValue = property.GetValue(value)?.ToString() ?? string.Empty;
                    if (result.Length > 0)
                        result.Append(" ");
                    result.Append(propValue);
                }
            }

            return result.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
