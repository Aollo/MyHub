using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using MyHub.Models;
using MyHub.Services;
using MyHub.Lifecycle;
using MyHub.Commands;
using MyHub.ComponentModel;
using System;

namespace MyHub.ViewModels
{
    public class StatusHomeViewModel : ViewModelBase
    {
        private ObservableCollection<Status> _statusList;
        //private IncrementalLoadingCollection<Status> _statusListSupportIncrementalLoading;
        private ObservableCollection<string> _snsTypes;
        private Account _currentUserAccount;
        private string _currentSelectedSnsType;
        private int _pageNumber;
        private int _pageCount;
        private string _sinceId;
        //private IList<Status> _loadMoreStatusList;
        private readonly string _synthesisModeName = "整合显示";

        public StatusHomeViewModel()
        {
            _statusList = new ObservableCollection<Status>();
            //_statusListSupportIncrementalLoading = new IncrementalLoadingCollection<Status>(LoadMoreStatusSupportIncrementalLoading);
            //_statusListSupportIncrementalLoading.LoadMoreComplated += _statusListSupportIncrementalLoading_LoadMoreComplated;
            //_loadMoreStatusList = new List<Status>();
            SnsTypes = new ObservableCollection<string>();
            var accounts = AppRuntimeEnvironment.Instance.GetAllUserAccount();
            if(accounts.Length > 1)
                SnsTypes.Add(_synthesisModeName);
            foreach(Account a in accounts)
            {
                SnsTypes.Add(a.Sns.Name);
            }

            _currentSelectedSnsType = SnsTypes.FirstOrDefault();
            PropertyChanged += StatusHomeViewModel_PropertyChanged;

            InitStatusParameter();
        }

        public ObservableCollection<string> SnsTypes
        {
            get
            {
                return _snsTypes;
            }
            private set
            {
                if(_snsTypes != value)
                {
                    _snsTypes = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<Status> StatusList
        {
            get
            {
                return _statusList;
            }
            set
            {
                if(_statusList != value)
                {
                    _statusList = value;
                    NotifyPropertyChanged();
                }
            }
        }

        //public IncrementalLoadingCollection<Status> StatusListSupportIcrementalLoading
        //{
        //    get { return _statusListSupportIncrementalLoading; }
        //    private set
        //    {
        //        if (_statusListSupportIncrementalLoading != value)
        //        {
        //            _statusListSupportIncrementalLoading = value;
        //            NotifyPropertyChanged();
        //        }
        //    }
        //}

        public string CurrentSelectedSnsType
        {
            get { return _currentSelectedSnsType; }
            set
            {
                if(_currentSelectedSnsType != value)
                {
                    _currentSelectedSnsType = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Account CurrentUserAccount
        {
            get { return _currentUserAccount; }
            set
            {
                if(_currentUserAccount != value)
                {
                    _currentUserAccount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override async Task LoadState()
        {
            await base.LoadState();

            // 接下来首先获取用户头像
            int index = 0;// 标志从那个下标对应的SnsTypes中的名称开始尝试获取用户头像
            if (_currentSelectedSnsType == _synthesisModeName)
                index = 1;
            for(; index < _snsTypes.Count; ++index)
            {
                CurrentUserAccount = AppRuntimeEnvironment.Instance.GetUserAccount(_snsTypes[index]);
                if (CurrentSelectedSnsType != null)
                    break;
            }

            //int index = 0;// 标志从那个下标对应的SnsTypes中的名称开始尝试获取用户头像
            //if (_currentSelectedSnsType == _synthesisModeName)
            //    index = 1;
            //for(; index < _snsTypes.Count; ++index)
            //{
            //    var service = ServiceLocator.Current.GetInstance<ISnsDataService>(_snsTypes[index]);
            //    CurrentUser = 
            //        (await service.GetUserProfile(AppRuntimeEnvironment.Instance.GetUserAccount(_snsTypes[index]).UserId, ""))?
            //        .BasicUserInfo;
            //    if (CurrentUser != null)
            //        break;
            //}

            await RefreshStatus();
            // await RefreshStatusSupportIncrementalLoading();
        }

        /// <summary>
        /// 根据社交网络名称载入单个社交网络的新鲜事
        /// </summary>
        /// <param name="snsName"></param>
        /// <returns>返回更新后的_statusList，返回值不为void是为了使用级联操作</returns>
        private async Task<ObservableCollection<Status>> LoadSingleSnsStatus(string snsName)
        {
            var service = ServiceLocator.Current.GetInstance<ISnsDataService>(snsName);
            if (service == null)
                return _statusList;
            var tempStatusList = await service.GetStatus(_pageNumber.ToString(), _pageCount.ToString(), _sinceId);
            if (tempStatusList == null || tempStatusList.Count <= 0)
                return _statusList;

            foreach(Status s in tempStatusList)
                _statusList.Add(s);

            return _statusList;
        }

        /// <summary>
        /// 载入所有社交网络的新鲜事，并按照时间线排序
        /// </summary>
        /// <returns>返回更新后的_statusList</returns>
        private async Task<ObservableCollection<Status>> LoadAllSnsStatus()
        {
            for(int i = 1; i < _snsTypes.Count; ++i)// 从第二项开始，因为第一项是"综合显示"
            {
                await LoadSingleSnsStatus(_snsTypes.ElementAt(i));
            }

            QuickSort(_statusList, 0, _statusList.Count - 1);

            return _statusList; 
        }

        public async Task RefreshStatus()
        {
            // 重新初始化
            _statusList.Clear();
            _pageNumber = 1;

            if(_currentSelectedSnsType == _synthesisModeName)
            {
                await LoadAllSnsStatus();
            }
            else
            {
                await LoadSingleSnsStatus(_currentSelectedSnsType);
            }

            ++_pageNumber;
            //NotifyPropertyChanged(nameof(StatusList));// 为了消除刷新因排序产生的闪烁，需要在加载完数据之后统一通知
        }

        public async Task LoadMoreStatus()
        {
            if (_currentSelectedSnsType == _synthesisModeName)
            {
                await LoadAllSnsStatus();
            }
            else
            {
                await LoadSingleSnsStatus(_currentSelectedSnsType);
            }

            ++_pageNumber;
            //NotifyPropertyChanged(nameof(StatusList));// 为了消除刷新因排序产生的闪烁，需要在加载完数据之后统一通知
        }

        #region 支持增量加载的部分

        //private async Task<IList<Status>> LoadSingleSnsStatusSupportIncrementalLoading(string snsName)
        //{
        //    var service = ServiceLocator.Current.GetInstance<ISnsDataService>(snsName);
        //    if (service == null)
        //        return _loadMoreStatusList;

        //    var tempStatusList = await service.GetStatus(_pageNumber.ToString(), _pageCount.ToString(), _sinceId);
        //    if (tempStatusList == null || tempStatusList.Count <= 0)
        //        return _loadMoreStatusList;

        //    //var count = tempStatusList.Count;
        //    //int i, j;
        //    //for(i = 0; i < count; ++i)// 按照时间线降序排序
        //    //{
        //    //    for (j = 0; 
        //    //        j < _loadMoreStatusList.Count 
        //    //        && _loadMoreStatusList[j].CreateTime >= tempStatusList[i].CreateTime; 
        //    //        ++j) ;
        //    //    _loadMoreStatusList.Insert(j, tempStatusList[i]);
        //    //}

        //    foreach (Status s in tempStatusList)
        //        _loadMoreStatusList.Add(s);

        //    return _loadMoreStatusList;
        //}

        //private async Task<IList<Status>> LoadAllSnsStatusSupportIncrementalLoading()
        //{
        //    for (int i = 1; i < _snsTypes.Count; ++i)// 从第二项开始，因为第一项是"综合显示"
        //    {
        //        await LoadSingleSnsStatusSupportIncrementalLoading(_snsTypes.ElementAt(i));
        //    }

        //    return _loadMoreStatusList;
        //}

        //public async Task RefreshStatusSupportIncrementalLoading()
        //{
        //    // 重新初始化
        //    _statusListSupportIncrementalLoading.Clear();
        //    _loadMoreStatusList.Clear();
        //    _pageNumber = 1;

        //    if (_currentSelectedSnsType == _synthesisModeName)
        //    {
        //        await LoadAllSnsStatusSupportIncrementalLoading();

        //        int count = _loadMoreStatusList.Count;
        //        int i, j;// k作为优化排序的变量而存在
        //        for(i = 0; i < count; ++i)// 按照时间线降序排序
        //        {
        //            for (j = 0; 
        //                j < _statusListSupportIncrementalLoading.Count
        //                && _statusListSupportIncrementalLoading[j].CreateTime >= _loadMoreStatusList[i].CreateTime; 
        //                ++j) ;
        //            if (j < _statusListSupportIncrementalLoading.Count)// 位置没有超出当前集合的下标
        //                _statusListSupportIncrementalLoading.Insert(j, _loadMoreStatusList[i]);
        //            else
        //                _statusListSupportIncrementalLoading.Add(_loadMoreStatusList[i]);
        //        }
        //    }
        //    else
        //    {
        //        await LoadSingleSnsStatusSupportIncrementalLoading(_currentSelectedSnsType);

        //        foreach (Status s in _loadMoreStatusList)// 单个社交网络获取到的数据一定是按照时间线降序的，不用重新排序
        //            _statusListSupportIncrementalLoading.Add(s);
        //    }
        //    ++_pageNumber;
        //}

        ///// <summary>
        ///// 支持增量加载的加载更多数据的方法，加载的数量为默认的_pageCount
        ///// </summary>
        ///// <returns></returns>
        //private async Task<Tuple<IList<Status>, bool>> LoadMoreStatusSupportIncrementalLoading()
        //{
        //    _loadMoreStatusList.Clear();

        //    if (_currentSelectedSnsType == _synthesisModeName)
        //    {
        //        await LoadAllSnsStatusSupportIncrementalLoading();
        //    }
        //    else
        //    {
        //        await LoadSingleSnsStatusSupportIncrementalLoading(_currentSelectedSnsType);
        //    }
        //    ++_pageNumber;

        //    // 如果实际加载的数量大于等于默认加载的数量，则默认还有数据
        //    return new Tuple<IList<Status>, bool>(_loadMoreStatusList, _loadMoreStatusList.Count >= _pageCount);
        //}

        //private void _statusListSupportIncrementalLoading_LoadMoreComplated(object sender, EventArgs e)
        //{
        //    // 如果在 “综合显示”模式下增量加载完成，则需要对整个结果结合进行重新排序
        //    if (_currentSelectedSnsType == _synthesisModeName)
        //    {
        //        QuickSort(_statusListSupportIncrementalLoading, 0, _statusListSupportIncrementalLoading.Count - 1);
        //    }
        //}

        #endregion

        private async void StatusHomeViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentSelectedSnsType")
            {
                InitStatusParameter();
                await LoadState();
            }
        }

        private void QuickSort(IList<Status> list, int low, int high)
        {
            if(low < high)
            {
                int pivotpos = Partition(list, low, high);
                QuickSort(list, low, pivotpos - 1);
                QuickSort(list, pivotpos + 1, high);
            }
        }

        private int Partition(IList<Status> list, int low, int high)
        {
            Status pivot = list[low];

            while(low < high)
            {
                while (low < high && list[high].CreateTime <= pivot.CreateTime) --high;
                list[low] = list[high];

                while (low < high && list[low].CreateTime >= pivot.CreateTime) ++low;
                list[high] = list[low];
            }
            list[low] = pivot;
            return low;
        }

        private void InitStatusParameter()
        {
            _pageNumber = 1;
            _pageCount = 5;
            _sinceId = "0";
        }
    }

}
