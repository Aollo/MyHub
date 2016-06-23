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
    public sealed partial class StatusHomePage : BasePage
    {
        private StatusHomeViewModel _viewModel;
        private bool _needLoad;
        private bool _needFullLoad;

        public StatusHomePage()
        {
            this.InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;
            Lifecycle.AppRuntimeEnvironment.Instance.PropertyChanged += UserAccount_PropertyChanged;
            _viewModel = null;
            _needLoad = true;// 第一次导航到时加载，后面只有在当前页面并点击主页按钮才加载
            _needFullLoad = true;// 标志是否完全重新导航
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(_viewModel == null || _needFullLoad)
            {
                _viewModel = new StatusHomeViewModel();
                DataContext = _viewModel;
                await _viewModel.LoadState();
            }
            else// 如果不需要完全重新加载页面
            {
                if (e.NavigationMode == NavigationMode.Back)
                    _needLoad = false;

                if (_needLoad)
                    await _viewModel.LoadState();
                else// 如果_needLoad == false
                    _needLoad = true;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if(e.NavigationMode == NavigationMode.Back)
            {
                NavigationCacheMode = NavigationCacheMode.Disabled;
            }
            else if(e.SourcePageType != typeof(StatusHomePage))// 如果不是导航到当前页面
            {
                _needLoad = false;
            }
            _needFullLoad = false;
        }

        private void UserAccount_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // 账户更改之后需要完全重新加载页面
            _needFullLoad = true;
        }

        #region 界面相关事件

        private void snsTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedSnsType = e.AddedItems.FirstOrDefault() as string;
            if (selectedSnsType == null)// 如果获取选中项失败，默认选中第一项
            {
                ComboBox cb = sender as ComboBox;
                if (cb.Items.Count == 0)
                    return;
                cb.SelectedIndex = 0;
                selectedSnsType = cb.SelectedItem as string;
            }

            _viewModel.CurrentSelectedSnsType = selectedSnsType;
        }

        /// <summary>
        /// 导航到相关页面
        /// </summary>
        private void statusList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if(e.ClickedItem is Models.Status)
            {
                Facade.NavigationFacade.NavigateToStatusDetailPage(e.ClickedItem as Models.Status);
            }
        }

        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            scrollViewer.ChangeView(null, (double)Application.Current.Resources["ProgressRingHeight"], null);
        }

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var sv = sender as ScrollViewer;

            // 加载更多
            if(sv.ScrollableHeight == sv.VerticalOffset)
            {
                await _viewModel.LoadMoreStatus();
            }
            
            // 下拉刷新
            if (!e.IsIntermediate)
            {
                if(sv.VerticalOffset == 0.0)
                {
                    progressRing.IsActive = true;

                    //await _viewModel.RefreshStatusSupportIncrementalLoading();
                    await _viewModel.RefreshStatus();
                    scrollViewer.ChangeView(null, (double)Application.Current.Resources["ProgressRingHeight"], null);
                }
                progressRing.IsActive = false;
            }
        }

        #endregion 界面相关事件
    }
}
