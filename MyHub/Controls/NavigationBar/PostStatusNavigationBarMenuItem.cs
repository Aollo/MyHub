using System;
using Windows.UI.Xaml.Controls;

namespace MyHub.Controls.NavigationBar
{
    public class PostStatusNavigationBarMenuItem : NavigationBarMenuItemBase, INavigationBarMenuItem
    {
        public Type DestPage
        {
            get
            {
                return typeof(Views.PostStatusPage);
            }
        }

        public object Arguments
        {
            get
            {
                return new ViewModels.PostStatusViewModelArgs()
                {
                    NavigationType = Lifecycle.MyHubEnums.NavigatedToPostStatusPageType.PostNewStatus
                };
            }
        }

        public string Label
        {
            get
            {
                return "发新鲜事";
            }
        }

        public override Symbol Symbol
        {
            get
            {
                return Symbol.Add;
            }
        }

        public bool IsLeft
        {
            get { return false; }
        }
    }
}
