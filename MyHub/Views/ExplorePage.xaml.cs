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
    public sealed partial class ExplorePage : BasePage
    {
        private ExploreViewModel _viewModel;

        public ExplorePage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _viewModel = new ExploreViewModel();
            DataContext = _viewModel;

            var load = e.NavigationMode != NavigationMode.Back;
            if (load)
                await _viewModel.LoadState();
        }

        private void exploreStatusListview_ItemClick(object sender, ItemClickEventArgs e)
        {
            var status = e.ClickedItem as Models.Status;
            Facade.NavigationFacade.NavigateToStatusDetailPage(status);
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var name = (((sender as Pivot)?.SelectedItem as PivotItem)?.Header) as string;

            if (name == null)
                return;
            if (name.Contains("热门微博"))
                _viewModel.CurrentSelectedExploreType = Lifecycle.MyHubEnums.ExploreType.PublicStatus;
            else if (name.Contains("本地"))
                _viewModel.CurrentSelectedExploreType = Lifecycle.MyHubEnums.ExploreType.LocalStatus;
            else if (name.Contains("话题"))
                _viewModel.CurrentSelectedExploreType = Lifecycle.MyHubEnums.ExploreType.HotTopics;
        }
    }
}
