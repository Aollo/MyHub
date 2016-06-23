using MyHub.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyHub.Services;
using MyHub.Commands;

namespace MyHub.ViewModels
{
    public class MentionUserSelectionViewModel : ViewModelBase
    {
        private ObservableCollection<User> _friendsList;

        public MentionUserSelectionViewModel()
        {
            _friendsList = null;
            SnsTypes = null;

            CancelCommand = new RelayCommand(OnCancelCommandAct);
        }

        public List<string> SnsTypes { get; set; }

        public RelayCommand CancelCommand { get; private set; }

        public ObservableCollection<User> FriendsList
        {
            get { return _friendsList; }
            set
            {
                if (_friendsList != value)
                {
                    _friendsList = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override async Task LoadState()
        {
            await base.LoadState();
            if (_friendsList == null)
                _friendsList = new ObservableCollection<User>();
            _friendsList.Clear();

            Account[] accounts = Lifecycle.AppRuntimeEnvironment.Instance.GetAllUserAccount();
            foreach(Account account in accounts)
            {
                if (SnsTypes != null && !SnsTypes.Contains(account.Sns.Name))
                    continue;

                var service = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ISnsDataService>(account.Sns.Name);
                var tempFriendsList = await service.GetUserFriends(account.UserId, account.UserName);
                if (tempFriendsList != null && tempFriendsList.Count > 0)
                    foreach (User u in tempFriendsList)
                        _friendsList.Add(u);
            }
            NotifyPropertyChanged(nameof(FriendsList));
        }

        public void OnCancelCommandAct()
        {
            Facade.NavigationFacade.GoBack(Lifecycle.MyHubEnums.NavigationFrameType.RightPart);
        }
    }
}
