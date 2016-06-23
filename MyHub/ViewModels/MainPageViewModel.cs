using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyHub.Controls.NavigationBar;

namespace MyHub.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public List<INavigationBarMenuItem> TopNavigationBarMenuItems { get; private set; }
        public List<INavigationBarMenuItem> BottomNavigationBarMenuItems { get; private set; }

        public MainPageViewModel()
        {
            TopNavigationBarMenuItems = new List<INavigationBarMenuItem>
            {
                new HomeNavigationBarMenuItem(),
                new NotificationCenterNavigationBarMenuItem(),
                new ExploreNavigationBarMenuItem(),
                new PostStatusNavigationBarMenuItem(),
            };

            BottomNavigationBarMenuItems = new List<INavigationBarMenuItem>()
            {
                new AccountCenterNavigationBarMenuItem(),
                new SettingCenterNavigationBarMenuItem(),
            };
        }

    }
}
