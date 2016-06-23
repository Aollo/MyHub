using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MyHub.Controls
{
    public sealed partial class StatusInteractionBottomButton : UserControl
    {
        public StatusInteractionBottomButton()
        {
            this.InitializeComponent();
        }

        private async void OnStatusInteractionBottomButtonClick(object sender, RoutedEventArgs e)
        {
            var name = (sender as AppBarButton).Name;
            Models.Status status;
            if (DataContext is Models.Status)
                status = DataContext as Models.Status;
            else if (DataContext is ViewModels.StatusDetailViewModel)
                status = (DataContext as ViewModels.StatusDetailViewModel).Status;
            else
                status = null;
            Services.ISnsDataService service = null;
            bool? result = null;

            switch(name)
            {
                case "report":
                    Facade.NavigationFacade.NavigateToPostStatusPage(Lifecycle.MyHubEnums.NavigatedToPostStatusPageType.Repost, status);
                    break;
                case "comment":
                    Facade.NavigationFacade.NavigateToPostStatusPage(Lifecycle.MyHubEnums.NavigatedToPostStatusPageType.Comment, status);
                    break;
                case "like":
                    service = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                        .GetInstance<Services.ISnsDataService>(status.Sns.Name);
                    result = await service.LikeStatus(status);
                    if (result == true)
                        like.Opacity = 0.5;
                    else if (result == false)
                        like.Opacity = 1;
                    break;
                case "favorite":
                    service = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                        .GetInstance<Services.ISnsDataService>(status.Sns.Name);
                    result = await service.FavoriteStatus(status);
                    if (result == true)
                        favorite.Opacity = 0.5;
                    else if (result == false)
                        favorite.Opacity = 1;
                    break;
                default:
                    break;
            }
        }
    }
}
