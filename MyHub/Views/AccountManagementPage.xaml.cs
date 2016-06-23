using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MyHub.ViewModels;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace MyHub.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AccountManagementPage : BasePage
    {
        private AccountManagementViewModel _viewModel;

        public AccountManagementPage()
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
                _viewModel = new AccountManagementViewModel();
                DataContext = _viewModel;
                await _viewModel.LoadState();// 只有在首次导航的时候才创建实例并载入状态，否则保持不变，因账户更改时会自动调用LoadState()更新数据
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if(e.NavigationMode == NavigationMode.Back)
            {
                NavigationCacheMode = NavigationCacheMode.Disabled;
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var userAccountProfile = e.ClickedItem as Tuple<Models.Account, Models.UserProfile>;
            if (userAccountProfile == null || !userAccountProfile.Item1.isAvailable)// 如果账户不可用，不进行导航
                return;

            Facade.NavigationFacade.NavigateToAccountDetailPage(userAccountProfile.Item2);
        }
    }
}
