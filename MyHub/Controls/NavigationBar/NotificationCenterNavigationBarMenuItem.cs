using System;
using Windows.UI.Xaml.Controls;

namespace MyHub.Controls.NavigationBar
{
    public class NotificationCenterNavigationBarMenuItem : NavigationBarMenuItemBase, INavigationBarMenuItem
    {
        public Type DestPage
        {
            get
            {
                return typeof(Views.NotificationCenterPage);
            }
        }

        public string Label
        {
            get
            {
                return "通知中心";
            }
        }

        public override Symbol Symbol
        {
            get
            {
                return Symbol.Message;
            }
        }

        public bool IsLeft
        {
            get { return true; }
        }
    }
}
