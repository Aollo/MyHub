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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace MyHub.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class WebViewerPage : BasePage
    {
        public WebViewerPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Uri)
                webViewControl.Navigate(e.Parameter as Uri);
        }

        private void webViewControl_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            titleBarTextBlock.Text = webViewControl.DocumentTitle;
        }

        private void webViewControl_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            titleBarTextBlock.Text = "打开链接失败，请重试...";
        }

        private void backAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Facade.NavigationFacade.GoBack(Lifecycle.MyHubEnums.NavigationFrameType.RightPart);
        }

        private void refreshAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            webViewControl.Refresh();
        }
    }
}
