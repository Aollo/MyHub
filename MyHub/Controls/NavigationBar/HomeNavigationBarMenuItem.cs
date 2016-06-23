using System;
using Windows.UI.Xaml.Controls;
using MyHub.Views;

namespace MyHub.Controls.NavigationBar
{
    public class HomeNavigationBarMenuItem : NavigationBarMenuItemBase, INavigationBarMenuItem
    {

        public Type DestPage
        {
            get
            {
                return typeof(StatusHomePage);
            }
        }

        public string Label
        {
            get
            {
                return "主页";
            }
        }

        public override Symbol Symbol
        {
            get
            {
                return Symbol.Home;
            }
        }

        public bool IsLeft
        {
            get { return true; }
        }
    }
}
