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
    public sealed partial class StatusDetailPage : BasePage
    {
        private StatusDetailViewModel _viewModel;

        public StatusDetailPage()
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
                _viewModel = new StatusDetailViewModel();
                DataContext = _viewModel;
            }

            if (e.Parameter is Models.Status)
            {
                _viewModel.Status = e.Parameter as Models.Status;
                await _viewModel.LoadState();
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

        private void interactionInfoListPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var name = (((sender as Pivot)?.SelectedItem as PivotItem)?.Header as TextBlock)?.Text;

            if (name == null)
                return;
            if (name.Contains("转发"))
                _viewModel.CurrentSelectedPivotItemName = "转发";
            else if (name.Contains("评论"))
                _viewModel.CurrentSelectedPivotItemName = "评论";
            else if (name.Contains("点赞"))
                _viewModel.CurrentSelectedPivotItemName = "点赞";
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if(e.ClickedItem is Models.Status)// 点击的是转发的项目
            {
                _viewModel.CurrentSelectedRepostItem = (e.ClickedItem as Models.Status);
            }
            else if(e.ClickedItem is Models.Comment)// 点击的是评论的项目
            {
                _viewModel.CurrentSelectedCommentItem = (e.ClickedItem as Models.Comment);
            }
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }
    }
}
