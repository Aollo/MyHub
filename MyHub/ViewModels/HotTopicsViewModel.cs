using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MyHub.Services;
using MyHub.Commands;

namespace MyHub.ViewModels
{
    public class HotTopicsViewModel : ViewModelBase
    {
        private ObservableCollection<string> _hotTopicsList;

        public HotTopicsViewModel()
        {
            _hotTopicsList = null;

            CancelCommand = new RelayCommand(OnCancelCommandAct);
        }

        public RelayCommand CancelCommand { get; private set; }

        public ObservableCollection<string> HotTopicsList
        {
            get { return _hotTopicsList; }
            set
            {
                if(_hotTopicsList != value)
                {
                    _hotTopicsList = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override async Task LoadState()
        {
            await base.LoadState();

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
            NotifyPropertyChanged(nameof(HotTopicsList));
        }

        public void OnCancelCommandAct()
        {
            Facade.NavigationFacade.GoBack( Lifecycle.MyHubEnums.NavigationFrameType.RightPart);
        }
    }
}
