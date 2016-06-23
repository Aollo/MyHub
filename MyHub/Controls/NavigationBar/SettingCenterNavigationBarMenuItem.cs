using System;
using Windows.UI.Xaml.Controls;

namespace MyHub.Controls.NavigationBar
{
    public class SettingCenterNavigationBarMenuItem : NavigationBarMenuItemBase, INavigationBarMenuItem
    {
        public Type DestPage
        {
            get
            {
                return typeof(Views.SettingCenterPage);
            }
        }

        public string Label
        {
            get
            {
                return "设置中心";
            }
        }

        public override Symbol Symbol
        {
            get
            {
                return Symbol.Setting;
            }
        }

        public bool IsLeft
        {
            get { return false; }
        }
    }
}
