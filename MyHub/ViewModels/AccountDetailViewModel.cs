using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MyHub.Services;
using MyHub.Models;
using MyHub.Commands;

namespace MyHub.ViewModels
{
    public class AccountDetailViewModel : ViewModelBase
    {
        private UserProfile _userProfile;
        private User _user;// 允许传入User类型的参数导航到此页面
        private Lifecycle.MyHubEnums.UserProfilePivotSelectionType _currentPivotSelectionType;
        private ObservableCollection<User> _friendList;
        private ObservableCollection<Status> _homeStatusList;
        private int _pageNumber;
        private int _pageCount;
        private string _sinceId;

        public AccountDetailViewModel()
        {
            _userProfile = null;
            _user = null;
            _friendList = null;
            _homeStatusList = null;
            _currentPivotSelectionType = Lifecycle.MyHubEnums.UserProfilePivotSelectionType.Statuses;// 需要与前台保持一致
            InitStatusParameter();

            BackCommand = new RelayCommand(OnBackAppbarButtonClick);

            PropertyChanged += AccountDetailViewModel_PropertyChanged;
        }

        public RelayCommand BackCommand { get; private set; }

        public UserProfile UserProfile
        {
            get { return _userProfile; }
            set
            {
                if(_userProfile != value)
                {
                    _userProfile = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public User User
        {
            get { return _user; }
            set
            {
                if (_user != value)
                {
                    _user = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Lifecycle.MyHubEnums.UserProfilePivotSelectionType CurrentPivotSelectionType
        {
            get { return _currentPivotSelectionType; }
            set
            {
                if (_currentPivotSelectionType != value)
                {
                    _currentPivotSelectionType = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<User> FriendList
        {
            get { return _friendList; }
            set
            {
                if (_friendList != value)
                {
                    _friendList = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<Status> HomeStatusList
        {
            get { return _homeStatusList; }
            set
            {
                if (_homeStatusList != value)
                {
                    _homeStatusList = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override async Task LoadState()
        {
            await base.LoadState();

            if (UserProfile == null)// 如果是通过User导航过来，暂时不要加载状态，因为UserProfile为空
                return;

            var service = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                .GetInstance<ISnsDataService>(UserProfile.BasicUserInfo.Sns.Name);
            switch(CurrentPivotSelectionType)
            {
                case Lifecycle.MyHubEnums.UserProfilePivotSelectionType.Home:
                    var tempFriendList = await service.GetUserFriends(UserProfile.BasicUserInfo.UserId, UserProfile.BasicUserInfo.NickName);
                    if (tempFriendList == null || tempFriendList.Count <= 0)
                        return;
                    FriendList = new ObservableCollection<User>(tempFriendList);
                    break;
                case Lifecycle.MyHubEnums.UserProfilePivotSelectionType.Statuses:
                    var tempStatusList = await service.GetUserHomeStatus(UserProfile.BasicUserInfo.UserId, UserProfile.BasicUserInfo.NickName, _pageNumber.ToString(), _pageCount.ToString(), _sinceId);
                    if (tempStatusList == null || tempStatusList.Count <= 0)
                        return;
                    HomeStatusList = new ObservableCollection<Status>(tempStatusList);
                    ++_pageNumber;
                    break;
                case Lifecycle.MyHubEnums.UserProfilePivotSelectionType.Photos:
                    break;
                default:
                    break;
            }
        }

        private async void AccountDetailViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "CurrentPivotSelectionType")
            {
                InitStatusParameter();
                await LoadState();
            }
            else if(e.PropertyName == "User")// 获取User对应的UserProfile
            {
                var service = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                .GetInstance<ISnsDataService>(User.Sns.Name);
                UserProfile = await service.GetUserProfile(User.UserId, "");

                InitStatusParameter();
                await LoadState();
            }
        }

        public void InitStatusParameter()
        {
            _pageNumber = 1;
            _pageCount = 5;
            _sinceId = "0";
        }

        private void OnBackAppbarButtonClick()
        {
            Facade.NavigationFacade.GoBack(Lifecycle.MyHubEnums.NavigationFrameType.RightPart);
        }
    }
}
