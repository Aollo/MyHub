using System;
using System.Collections.Generic;
using MyHub.Lifecycle;

namespace MyHub.Facade
{
    /// <summary>
    /// 封装了页面导航相关
    /// </summary>
    public interface INavigationFacade
    {
        void GoBack(MyHubEnums.NavigationFrameType navigationFrameType);

        void NavigateToAboutPage();

        void NavigateToAccountDetailPage(Models.User user);

        void NavigateToAccountDetailPage(Models.UserProfile userProfile);

        void NavigateToAccountManagementPage();

        void NavigateToExplorePage();

        void NavigateToHotTopicsPage();

        void NavigateToLocationSelectionPage();

        void NavigateToMainPage();

        void NavigateToMentionUserSelectionPage(List<string> snsTypes);

        void NavigateToMyHubBlankPage();

        void NavigateToNotificationCenterPage();

        void NavigateToPostStatusPage();

        void NavigateToPostStatusPage(MyHubEnums.NavigatedToPostStatusPageType type, object parameter);

        void NavigateToSettingCenterPage();

        void NavigateToStatusDetailPage(Models.Status status);

        void NavigateToStatusHomePage();

        void NavigateToWebViewerPage(Uri uri);

        void NavigateToWelcomePage();
    }
}
