using System;
using Windows.UI.Xaml.Data;


namespace MyHub.ValueConverters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 将账户是否登录isAvailable转化为MenuItem是否可见
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool result = false;
            bool invert = (parameter != null) && System.Convert.ToBoolean(parameter);

            if (value is bool)
                result = (bool)value;
            if (invert)
                result = !result;

            if (result)
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
