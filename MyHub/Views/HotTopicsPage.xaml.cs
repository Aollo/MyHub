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
    public sealed partial class HotTopicsPage : BasePage
    {
        private HotTopicsViewModel _viewModel;

        public HotTopicsPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _viewModel = new HotTopicsViewModel();
            DataContext = _viewModel;

            var load = e.NavigationMode != NavigationMode.Back;
            if (load)
                await _viewModel.LoadState();
        }

        private void hotTopicsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var parameter = string.Format("#{0}#", (e.ClickedItem as string));
            Facade.NavigationFacade.NavigateToPostStatusPage(Lifecycle.MyHubEnums.NavigatedToPostStatusPageType.TransferHotTopic, parameter);
        }
    }
}
