using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MyHub.Models;
using MyHub.Services;
using System.Collections.Generic;
using MyHub.Commands;

namespace MyHub.ViewModels
{
    public class LocationSelectionViewModel : ViewModelBase
    {
        private ObservableCollection<Location> _locationList;
        private List<string> _suggestionLocation;

        public LocationSelectionViewModel()
        {
            _locationList = null;
            _suggestionLocation = new List<string>()// 存放搜索框的建议
            {
                "加利福尼亚州",
                "旧金山",
                "加州大学伯克利分校",
                "斯坦福大学",
                "卡内基梅隆大学",
                "麻省理工学院"
            };

            CancelCommand = new RelayCommand(OnCancelCommandAct);
        }

        public RelayCommand CancelCommand { get; private set; }

        public ObservableCollection<Location> LocationList
        {
            get { return _locationList; }
            set
            {
                if (_locationList != value)
                {
                    _locationList = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public List<string> SuggestionLocation
        {
            get { return _suggestionLocation; }
        }
        
        public override async Task LoadState()
        {
            await base.LoadState();

            var localDataService = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ILocalDataService>();
            var currentLocation = await localDataService.GetCurrentLocation();
            if (currentLocation == null || currentLocation.Latitude == null || currentLocation.Longitude == null)
                return;

            var snsDataService = 
                Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ISnsDataService>("新浪微博");
            if (snsDataService == null)// 如果用户没有授权登陆新浪微博、新浪微博服务没有注册，将不能使用该服务
                return;

            var tempLocationList = await snsDataService.GetNearbyPositions(currentLocation.Latitude, currentLocation.Longitude, "", "", "", "");
            LocationList = new ObservableCollection<Location>(tempLocationList);
        }

        public async Task SearchLocation(string keyword)
        {
            var service = 
                Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ISnsDataService>("新浪微博");
            if (service == null)// 如果用户没有授权登陆新浪微博、新浪微博服务没有注册，将不能使用该服务
                return;

            var tempLocationList = await service.SearchLocation(keyword, "", "");
            if (tempLocationList == null)
                return;

            LocationList = new ObservableCollection<Location>(tempLocationList);
        }

        public void OnCancelCommandAct()
        {
            Facade.NavigationFacade.GoBack(Lifecycle.MyHubEnums.NavigationFrameType.RightPart);
        }
    }
}
