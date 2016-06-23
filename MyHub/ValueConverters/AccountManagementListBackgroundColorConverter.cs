using System;
using Windows.UI.Xaml.Data;

namespace MyHub.ValueConverters
{
    public class AccountManagementListBackgroundColorConverter : IValueConverter
    {
        /// <summary>
        /// 将账户是否登录isAvailable转化为背景颜色
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool result = false;

            if (value is bool)
                result = (bool)value;

            if (result)
                return "White";
            else
                return "LightGray";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
