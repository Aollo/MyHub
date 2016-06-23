using System;
using Windows.UI.Xaml.Controls;

namespace MyHub.Controls.NavigationBar
{
    public class ExploreNavigationBarMenuItem : NavigationBarMenuItemBase, INavigationBarMenuItem
    {
        public Type DestPage
        {
            get
            {
                return typeof(Views.ExplorePage);
            }
        }

        public string Label
        {
            get
            {
                return "探索发现";
            }
        }

        public override Symbol Symbol
        {
            get
            {
                return Symbol.Find;
            }
        }

        public bool IsLeft
        {
            get { return true; }
        }
    }
}
