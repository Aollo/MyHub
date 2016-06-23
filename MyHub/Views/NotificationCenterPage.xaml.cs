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
    public sealed partial class NotificationCenterPage : BasePage
    {
        private NotificationCenterViewModel _viewModel;

        public NotificationCenterPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _viewModel = new NotificationCenterViewModel();
            DataContext = _viewModel;

            var loadData = e.NavigationMode != NavigationMode.Back;
            if (loadData)
            {
                await _viewModel.LoadState();
            }
        }

        private void snsTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.CurrentSelectedSnsType = e.AddedItems.FirstOrDefault() as string;
        }

        private void rootPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var name = (((sender as Pivot)?.SelectedItem as PivotItem)?.Header as TextBlock)?.Text;

            if (name == null)
                return;
            if (name.Contains("提到"))
                _viewModel.CurrentSelectedMessageType = Models.NotificationMessageType.Mentions;
            else if (name.Contains("评论"))
                _viewModel.CurrentSelectedMessageType = Models.NotificationMessageType.Comments;
            else if (name.Contains("点赞"))
                _viewModel.CurrentSelectedMessageType = Models.NotificationMessageType.Likes;
        }

        private void notificationMessageList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var status = (e.ClickedItem as Models.BasicNotificationMessage).OriginalStatus;
            if (status != null)
                Facade.NavigationFacade.NavigateToStatusDetailPage(status);
        }
    }
}
