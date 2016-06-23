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
using MyHub.Controls.NavigationBar;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace MyHub.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : BasePage
    {
        public static Frame LeftPartFrame { get; private set; }
        public static Frame RightPartFrame { get; private set; }
        private MainPageViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();

            Lifecycle.AppRuntimeEnvironment.Instance.SetFrame(Lifecycle.MyHubEnums.NavigationFrameType.LeftPart, leftPartFrame);
            Lifecycle.AppRuntimeEnvironment.Instance.SetFrame(Lifecycle.MyHubEnums.NavigationFrameType.RightPart, rightPartFrame);

            _viewModel = new MainPageViewModel();
            DataContext = _viewModel;

            LeftPartFrame = leftPartFrame;
            RightPartFrame = rightPartFrame;
        }

        private void togglePaneButton_Click(object sender, RoutedEventArgs e)
        {
            mainSplitView.IsPaneOpen = !mainSplitView.IsPaneOpen;
        }

        private void navMenuList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var menuItem = e.ClickedItem as INavigationBarMenuItem;
            if(menuItem != null && menuItem.DestPage != null)
            {
                if(menuItem.IsLeft)
                {
                    if(menuItem.Arguments == null)
                        LeftPartFrame.Navigate(menuItem.DestPage);
                    else
                        LeftPartFrame.Navigate(menuItem.DestPage, menuItem.Arguments);
                }
                else
                {
                    if (menuItem.Arguments == null)
                        rightPartFrame.Navigate(menuItem.DestPage);
                    else
                        rightPartFrame.Navigate(menuItem.DestPage, menuItem.Arguments);
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Facade.NavigationFacade.NavigateToStatusHomePage();
            Facade.NavigationFacade.NavigateToMyHubBlankPage();
        }
    }
}
