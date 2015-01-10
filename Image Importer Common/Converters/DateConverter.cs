using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Image_Importer.Common.Converters
{
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return string.Empty;
            DateTimeOffset date = (DateTimeOffset)value;
            return date.ToString("f");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            string strValue = value as string;
            DateTimeOffset resultDateTime;
            if (DateTimeOffset.TryParse(strValue, out resultDateTime))
            {
                return resultDateTime;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}