using System;
using Windows.UI.Xaml.Controls;

namespace MyHub.Controls.NavigationBar
{
    public class AccountCenterNavigationBarMenuItem : NavigationBarMenuItemBase, INavigationBarMenuItem
    {
        public Type DestPage
        {
            get
            {
                return typeof(Views.AccountManagementPage);
            }
        }

        public string Label
        {
            get
            {
                return "账户中心";
            }
        }

        public override Symbol Symbol
        {
            get
            {
                return Symbol.People;
            }
        }

        public bool IsLeft
        {
            get { return true; }
        }
    }
}
