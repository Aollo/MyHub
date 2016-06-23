using System;
using Windows.UI.Xaml.Data;

namespace MyHub.ValueConverters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var invert = (parameter != null) && System.Convert.ToBoolean(parameter);

            var visibility = value != null;

            if (invert)
                visibility = !visibility;

            if (visibility)
                return Windows.UI.Xaml.Visibility.Visible;
            else
                return Windows.UI.Xaml.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
