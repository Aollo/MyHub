using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MyHub.Models;
using MyHub.Lifecycle;
using MyHub.Services;

namespace MyHub.ViewModels
{
    public class ExploreViewModel : ViewModelBase
    {
        private ObservableCollection<Status> _statusList;
        private ObservableCollection<string> _hotTopicsList;
        private MyHubEnums.ExploreType _currentSelectedExploreType;
        private int _pageNumber;
        private int _pageCount;
        private string _sinceId;

        public ExploreViewModel()
        {
            _statusList = null;
            _hotTopicsList = null;
            InitStatusParameter();

            PropertyChanged += ExploreViewModel_PropertyChanged;
        }

        public ObservableCollection<Status> StatusList
        {
            get
            {
                return _statusList;
            }
            set
            {
                if (_statusList != value)
                {
                    _statusList = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> HotTopicsList
        {
            get
            {
                return _hotTopicsList;
            }
            set
            {
                if (_hotTopicsList != value)
                {
                    _hotTopicsList = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public MyHubEnums.ExploreType CurrentSelectedExploreType
        {
            get
            {
                return _currentSelectedExploreType;
            }
            set
            {
                if (_currentSelectedExploreType != value)
                {
                    _currentSelectedExploreType = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override async Task LoadState()
        {
            await base.LoadState();

            switch(CurrentSelectedExploreType)
            {
                case MyHubEnums.ExploreType.PublicStatus:
                    StatusList = await LoadPublicStatus();
                    break;
                case MyHubEnums.ExploreType.LocalStatus:
                    StatusList = await LoadLocalStatus();
                    break;
                case MyHubEnums.ExploreType.HotTopics:
                    HotTopicsList = await LoadHotTopics();
                    break;
                default:
                    break;
            }
        }

        private async void ExploreViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "CurrentSelectedExploreType")
            {
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

        private async Task<ObservableCollection<Status>> LoadPublicStatus()
        {
            if (_statusList == null)
                _statusList = new ObservableCollection<Status>();
            _statusList.Clear();

            var services =
                Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetAllInstances<ISnsDataService>();

            foreach (ISnsDataService service in services)
            {
                var tempStatusList = await service.GetPublicStatus(_pageNumber.ToString(), _pageCount.ToString());
                if (tempStatusList != null)
                    foreach (Status s in tempStatusList)
                        _statusList.Add(s);
            }
            _statusList = new ObservableCollection<Status>(_statusList.OrderBy(i => i.CreateTime));
            StatusList = new ObservableCollection<Status>(_statusList.Reverse());

            return StatusList;
        }

        private async Task<ObservableCollection<Status>> LoadLocalStatus()
        {
            if (_statusList == null)
                _statusList = new ObservableCollection<Status>();
            _statusList.Clear();

            var services =
                Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetAllInstances<ISnsDataService>();
            var localService =
                Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ILocalDataService>();
            var location = await localService.GetCurrentLocation();

            foreach (ISnsDataService service in services)
            {
                var tempStatusList = await service.GetLocalStatus(location.Latitude, location.Longitude, "", _pageNumber.ToString(), _pageCount.ToString());
                if (tempStatusList != null)
                    foreach (Status s in tempStatusList)
                        _statusList.Add(s);
            }
            _statusList = new ObservableCollection<Status>(_statusList.OrderBy(i => i.CreateTime));
            StatusList = new ObservableCollection<Status>(_statusList.Reverse());

            return StatusList;
        }

        private async Task<ObservableCollection<string>> LoadHotTopics()
        {
            if (_hotTopicsList == null)
                _hotTopicsList = new ObservableCollection<string>();
            _hotTopicsList.Clear();

            var services =
                Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetAllInstances<ISnsDataService>();

            foreach (ISnsDataService service in services)
            {
                var tempTopicsList = await service.GetHotTopics();
                if (tempTopicsList != null)
                    foreach (string s in tempTopicsList)
                        HotTopicsList.Add(s);
            }

            return HotTopicsList;
        }
    }
}
