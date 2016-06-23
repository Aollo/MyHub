using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MyHub.ViewModels;
using MyHub.Models;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace MyHub.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AccountDetailPage : BasePage
    {
        private AccountDetailViewModel _viewModel;

        public AccountDetailPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Enabled;
            _viewModel = null;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(_viewModel == null)
            {
                _viewModel = new AccountDetailViewModel();
                DataContext = _viewModel;
            }

            if(e.Parameter is UserProfile)
            {
                var userProfile = e.Parameter as UserProfile;
                if (userProfile == null)
                    return;
                _viewModel.UserProfile = userProfile;
            }
            else if(e.Parameter is User)
            {
                var user = e.Parameter as User;
                if (user == null)
                    return;
                _viewModel.User = user;
            }

            var load = e.NavigationMode != NavigationMode.Back;
            if (load)
                await _viewModel.LoadState();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if(e.NavigationMode == NavigationMode.Back)
            {
                NavigationCacheMode = NavigationCacheMode.Disabled;
            }
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var name = (((sender as Pivot)?.SelectedItem as PivotItem)?.Header) as string;

            if (name == null)
                return;
            if (name.Contains("主页"))
                _viewModel.CurrentPivotSelectionType = Lifecycle.MyHubEnums.UserProfilePivotSelectionType.Home;
            else if (name.Contains("新鲜事"))
                _viewModel.CurrentPivotSelectionType = Lifecycle.MyHubEnums.UserProfilePivotSelectionType.Statuses;
            else if (name.Contains("相册"))
                _viewModel.CurrentPivotSelectionType = Lifecycle.MyHubEnums.UserProfilePivotSelectionType.Photos;
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if(e.ClickedItem is User)
            {
                Facade.NavigationFacade.NavigateToAccountDetailPage(e.ClickedItem as User);
            }
            else if(e.ClickedItem is Status)
            {
                Facade.NavigationFacade.NavigateToStatusDetailPage(e.ClickedItem as Status);
            }
        }
    }
}
