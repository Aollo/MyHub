using System;
using System.Collections.Generic;
using MyHub.Views;
using MyHub.ViewModels;
using MyHub.Lifecycle;
using Windows.UI.Xaml.Controls;

namespace MyHub.Facade
{
    public class NavigationFacade
    {
        private static Frame _frame;

        private static void EnsureFrameIsAvailable(MyHubEnums.NavigationFrameType navigationFrameType)
        {
            if (navigationFrameType != MyHubEnums.NavigationFrameType.MainFrame)
                _frame = AppRuntimeEnvironment.Instance.GetFrame(navigationFrameType);
            else// 主页面导航
                _frame = Windows.UI.Xaml.Window.Current.Content as Frame;
        }

        private static void DoNavigate(Type destPage, MyHubEnums.NavigationFrameType navigationFrameType, object parameter = null)
        {
            EnsureFrameIsAvailable(navigationFrameType);

            if (parameter == null)
                _frame.Navigate(destPage);
            else
                _frame.Navigate(destPage, parameter);
        }

        public static void GoBack(MyHubEnums.NavigationFrameType navigationFrameType)
        {
            EnsureFrameIsAvailable(navigationFrameType);
            if (_frame.CanGoBack)
                _frame.GoBack();
        }

        public static void NavigateToAboutPage()
        {
            DoNavigate(typeof(AboutPage), MyHubEnums.NavigationFrameType.RightPart);
        }

        public static void NavigateToAccountDetailPage(Models.User user)
        {
            DoNavigate(typeof(AccountDetailPage), MyHubEnums.NavigationFrameType.RightPart, user);
        }

        public static void NavigateToAccountDetailPage(Models.UserProfile userProfile)
        {
            DoNavigate(typeof(AccountDetailPage), MyHubEnums.NavigationFrameType.RightPart, userProfile);
        }

        public static void NavigateToAccountManagementPage()
        {
            DoNavigate(typeof(AccountManagementPage), MyHubEnums.NavigationFrameType.LeftPart);
        }

        public static void NavigateToExplorePage()
        {
            DoNavigate(typeof(ExplorePage), MyHubEnums.NavigationFrameType.LeftPart);
        }

        public static void NavigateToHotTopicsPage()
        {
            DoNavigate(typeof(HotTopicsPage), MyHubEnums.NavigationFrameType.RightPart);
        }

        public static void NavigateToLocationSelectionPage()
        {
            DoNavigate(typeof(LocationSelectionPage), MyHubEnums.NavigationFrameType.RightPart);
        }

        public static void NavigateToMainPage()
        {
            DoNavigate(typeof(MainPage), MyHubEnums.NavigationFrameType.MainFrame);
        }

        public static void NavigateToMentionUserSelectionPage(List<string> snsTypes)
        {
            DoNavigate(typeof(MentionUserSelectionPage), MyHubEnums.NavigationFrameType.RightPart, snsTypes);
        }

        public static void NavigateToMyHubBlankPage()
        {
            DoNavigate(typeof(MyHubBlankPage), MyHubEnums.NavigationFrameType.RightPart);
        }

        public static void NavigateToNotificationCenterPage()
        {
            DoNavigate(typeof(NotificationCenterPage), MyHubEnums.NavigationFrameType.LeftPart);
        }

        public static void NavigateToPostStatusPage()
        {
            DoNavigate(typeof(PostStatusPage), MyHubEnums.NavigationFrameType.RightPart);
        }

        public static void NavigateToPostStatusPage(MyHubEnums.NavigatedToPostStatusPageType type, object parameter)
        {
            DoNavigate(typeof(PostStatusPage), MyHubEnums.NavigationFrameType.RightPart, 
                new PostStatusViewModelArgs()
                {
                    NavigationType = type,
                    Parameter = parameter
                });
        }

        public static void NavigateToSettingCenterPage()
        {
            DoNavigate(typeof(SettingCenterPage), MyHubEnums.NavigationFrameType.RightPart);
        }

        public static void NavigateToStatusDetailPage(Models.Status status)
        {
            DoNavigate(typeof(StatusDetailPage), MyHubEnums.NavigationFrameType.RightPart, status);
        }

        public static void NavigateToStatusHomePage()
        {
            DoNavigate(typeof(StatusHomePage), MyHubEnums.NavigationFrameType.LeftPart);
        }

        public static void NavigateToWebViewerPage(Uri uri)
        {
            DoNavigate(typeof(WebViewerPage), MyHubEnums.NavigationFrameType.RightPart, uri);
        }

        public static void NavigateToWelcomePage()
        {
            DoNavigate(typeof(WelcomePage), MyHubEnums.NavigationFrameType.MainFrame);
        }
    }
}
