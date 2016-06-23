using System;
using Windows.UI.Xaml.Data;

namespace MyHub.ValueConverters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool result = false;

            if (value is bool)
                result = (bool)value;

            if (result)
                return "Orange";
            else
                return "Black";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
