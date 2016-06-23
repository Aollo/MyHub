using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyHub.Models;
using Windows.Storage;
using MyHub.Commands;
using MyHub.Services;
using MyHub.Lifecycle;

namespace MyHub.ViewModels
{
    public class PostStatusViewModel : ViewModelBase
    {
        private string _content;
        private Location _location;
        private StorageFile _picture;
        private List<string> _snsTypesList;
        private ObservableCollection<string> _checkedSnsTypeList;// 表示在前台选中了发布到哪个社交网络的名称列表
        private bool _canPublish;// 表示发布按钮是否可用
        private string _locationUrl;// 保存新浪微博位置链接文本
        private int _contentLengthLimit;
        private Lifecycle.MyHubEnums.NavigatedToPostStatusPageType? _navigatedType;// 标志为什么要导航到这个页面，合法值是发布、转发、评论
        private Status _sourceStatus;// 转发或评论的原新鲜事
        private Comment _sourceComment;// 回复评论或转发评论需要用到的评论信息

        public PostStatusViewModel()
        {
            _content = "";
            _location = new Location();
            _snsTypesList = null;
            _checkedSnsTypeList = new ObservableCollection<string>();
            _canPublish = false;
            _locationUrl = "";
            _contentLengthLimit = 140;
            _navigatedType = null;

            PropertyChanged += PostStatusViewModel_PropertyChanged;
            _checkedSnsTypeList.CollectionChanged += _checkedSnsTypeList_CollectionChanged;

            BackCommand = new RelayCommand(OnBackAppbarButtonClick);
            PublishCommand = new RelayCommand(OnPushlishAppbarButtonClick, () => CanPublish);
            InsertTopicCommand = new RelayCommand(OnInsertTopicCommandAct);
            InsertMentionCommand = new RelayCommand(OnInsertMentionCommandAct);
            InsertLocationCommand = new RelayCommand(OnInsertLocationCommandAct);
            InsertPictureLocalCommand = new RelayCommand(OnInsertPictureLocalCommandAct);
            InsertPictureCameraCommand = new RelayCommand(OnInsertPictureCameraCommandAct);
            InsertPictureInkCommand = new RelayCommand(OnInsertPictureInkCommandAct);
        }

        public RelayCommand BackCommand { get; private set; }

        public RelayCommand PublishCommand { get; private set; }

        public RelayCommand InsertTopicCommand { get; private set; }

        public RelayCommand InsertMentionCommand { get; private set; }

        public RelayCommand InsertLocationCommand { get; private set; }

        public RelayCommand InsertPictureLocalCommand { get; private set; }

        public RelayCommand InsertPictureCameraCommand { get; private set; }

        public RelayCommand InsertPictureInkCommand { get; private set; }

        /// <summary>
        /// 导航到该页面的最初页面，用来导航返回
        /// </summary>
        public Type SourcePage { get; set; }

        /// <summary>
        /// 要发布状态的文本内容，不包括位置信息、图片等，需要与前台绑定
        /// </summary>
        public string Content
        {
            get { return _content; }
            set
            {
                if(_content != value)
                {
                    _content = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Location Location
        {
            get { return _location; }
            set
            {
                if(_location != value)
                {
                    _location = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public List<string> SnsTypeList
        {
            get { return _snsTypesList; }
            private set { _snsTypesList = value; }
        }

        public ObservableCollection<string> CheckedSnsTypeList
        {
            get { return _checkedSnsTypeList; }
            set
            {
                if(_checkedSnsTypeList != value)
                {
                    _checkedSnsTypeList = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool CanPublish
        {
            get { return _canPublish; }
            set
            {
                if(_canPublish != value)
                {
                    _canPublish = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public MyHubEnums.NavigatedToPostStatusPageType NavigatedType
        {
            get { return (MyHubEnums.NavigatedToPostStatusPageType)_navigatedType; }
            set
            {
                if(_navigatedType != value)
                {
                    _navigatedType = value;
                }
            }
        }

        public Status SourceStatus
        {
            get { return _sourceStatus; }
            set
            {
                if(_sourceStatus != value)
                {
                    _sourceStatus = value;
                }
            }
        }

        public Comment SourceComment
        {
            get { return _sourceComment; }
            set
            {
                if (_sourceComment != value)
                {
                    _sourceComment = value;
                }
            }
        }

        public StorageFile Picture
        {
            get { return _picture; }
            set { _picture = value; }
        }

        public override async Task LoadState()
        {
            await base.LoadState();

            // 载入SnsTypeList信息，用于前台更新页面
            if (_snsTypesList == null)
                _snsTypesList = new List<string>();
            _snsTypesList.Clear();

            Account[] accounts = Lifecycle.AppRuntimeEnvironment.Instance.GetAllUserAccount();
            foreach(Account account in accounts)
            {
                _snsTypesList.Add(account.Sns.Name);
            }
        }

        private async void PostStatusViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Content")
            {
                DoPublishCheck();
            }
            else if(e.PropertyName == "Location")
            {
                _locationUrl = await GenerateWeiboLocationShortUrl(_location);
                DoPublishCheck();
            }
            else if(e.PropertyName == "CheckedSnsTypeList")
            {
                DoPublishCheck();
            }
        }

        private void _checkedSnsTypeList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DoPublishCheck();
        }

        private void OnBackAppbarButtonClick()
        {
            Facade.NavigationFacade.GoBack(MyHubEnums.NavigationFrameType.RightPart);
        }

        /// <summary>
        /// 根据用户选择的社交网络类型、用户输入的内容发布信息
        /// </summary>
        private async void OnPushlishAppbarButtonClick()
        {
            if (_navigatedType == null)
                return;
            DoPublishCheck();// 漏网之鱼
            if (!CanPublish)
                return;

            switch(_navigatedType)
            {
                case MyHubEnums.NavigatedToPostStatusPageType.PostNewStatus:
                    await PostNewStatus();
                    break;
                case MyHubEnums.NavigatedToPostStatusPageType.Repost:
                    await RepostStatus();
                    break;
                case MyHubEnums.NavigatedToPostStatusPageType.Comment:
                    await CommentStatus();
                    break;
                case MyHubEnums.NavigatedToPostStatusPageType.ReplyComment:
                    await ReplyComment();
                    break;
                case MyHubEnums.NavigatedToPostStatusPageType.RepostComment:
                    await RepostComment();
                    break;
                default:
                    break;
            }
            DoInitAfterPublish();

            // TODO: 将发布成功后的消息加入当前消息列表

            // 回退到SourcePage
            //AppRuntimeEnvironment.Instance.GetFrame(Views.NavigationFrameType.RightPart)
            //    .Navigate(SourcePage);
            Facade.NavigationFacade.NavigateToMyHubBlankPage();
        }

        private async Task<List<Status>> PostNewStatus()// 发布一条新鲜事到多个社交网络
        {
            List<Status> publishedStatus = new List<Status>(CheckedSnsTypeList.Count);

            foreach(string snsType in CheckedSnsTypeList)
            {
                var service = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ISnsDataService>(snsType);
                if (service == null)
                    continue;
                var status = await service.PostStatus(_content + _locationUrl, _location?.Longitude, _location?.Latitude, _picture);
                if (status != null)
                    publishedStatus.Add(status);
            }
            
            return publishedStatus;
        }

        private async Task<List<Status>> RepostStatus()
        {
            if (_sourceStatus == null)
                return null;

            List<Status> repostedStatus = new List<Status>(CheckedSnsTypeList.Count);

            foreach (string snsType in CheckedSnsTypeList)
            {
                var service = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ISnsDataService>(snsType);
                if (service == null)
                    continue;
                if(_sourceStatus.Sns.Name == snsType)// 本社交网络平台内采用转发
                {
                    var status = await service.RepostStatus(_sourceStatus, _content, "");
                    if (status != null)
                        repostedStatus.Add(status);
                }
                else// 跨社交网络平台重新发送新鲜事
                {
                    var text = _content + "//@" + _sourceStatus.Author.NickName + ":" + _sourceStatus.Content
                                + "[转发自" + _sourceStatus.Sns.Name + "]";
                    if(text.Length > _contentLengthLimit)
                        text = text.Take(_contentLengthLimit).ToString();
                    var status = await service.PostStatus(text, "", "", null);
                    if (status != null)
                        repostedStatus.Add(status);
                }
            }

            return repostedStatus;
        }

        /// <summary>
        /// 只能评论该社交网络内的新鲜事
        /// </summary>
        /// <returns></returns>
        private async Task<Comment> CommentStatus()
        {
            if (_sourceStatus == null)
                return null;

            var service = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ISnsDataService>(_sourceStatus.Sns.Name);
            var comment = await service.CommentStatus(_sourceStatus, _content, "");
            return comment;
        }

        private async Task<Comment> ReplyComment()
        {
            if (_sourceComment == null)
                return null;

            var service = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ISnsDataService>(_sourceComment.Sns.Name);
            var comment = await service.ReplyComment(_sourceComment, _content);
            return comment;
        }

        private async Task<List<Status>> RepostComment()
        {
            if (_sourceComment == null)
                return null;

            _sourceStatus = _sourceComment.StatusInfo;// 利用这种方式进行代码复用，复用转发新鲜事的方法
            if (_sourceStatus != null)
                return await RepostStatus();
            else
                return await PostNewStatus();
        }

        /// <summary>
        /// 根据Location中的信息生成新浪微博定位信息的短链接
        /// </summary>
        private async Task<string> GenerateWeiboLocationShortUrl(Location location)
        {
            if (location == null || string.IsNullOrWhiteSpace(location.Poiid))
                return "";

            string url = "http://weibo.com/p/100101" + location.Poiid;
            var service = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ISnsDataService>("新浪微博");
            if (service == null)
                return url;
            string shortUrl = await service.ShortenUrl(url);
            return shortUrl;
        }

        /// <summary>
        /// 做发布之前的检查，主要检查文本长度、选择要发布的社交网络类型是否符合规则
        /// </summary>
        private void DoPublishCheck()
        {
            bool result = true;

            if (Content.Length + _locationUrl.Length <= 0 || Content.Length + _locationUrl.Length > _contentLengthLimit)
                result = false;
            if (CheckedSnsTypeList == null || CheckedSnsTypeList.Count == 0)
                result = false;

            CanPublish = result;
        }

        private void DoInitAfterPublish()
        {
            Content = "";
            Location = null;
        }

        private void OnInsertTopicCommandAct()
        {
            Facade.NavigationFacade.NavigateToHotTopicsPage();
        }

        private void OnInsertLocationCommandAct()
        {
            Facade.NavigationFacade.NavigateToLocationSelectionPage();
        }

        private void OnInsertMentionCommandAct()
        {
            List<string> parameter = new List<string>();
            foreach (string s in CheckedSnsTypeList)
            {
                parameter.Add(s);
            }

            Facade.NavigationFacade.NavigateToMentionUserSelectionPage(parameter);
        }

        private void OnInsertPictureLocalCommandAct()
        {
            //Windows.Storage.Pickers.FileOpenPicker fileOpenPicker = new Windows.Storage.Pickers.FileOpenPicker();
            //fileOpenPicker.FileTypeFilter.Add(".jpg");
            //fileOpenPicker.FileTypeFilter.Add(".jpeg");
            //fileOpenPicker.FileTypeFilter.Add(".gif");
            //fileOpenPicker.FileTypeFilter.Add(".png");

            //StorageFile file = await fileOpenPicker.PickSingleFileAsync();
            //if (file != null)
            //{
            //    _viewModel.Picture = await CopyToIso(file);
            //}
        }

        private void OnInsertPictureCameraCommandAct()
        {

        }

        private void OnInsertPictureInkCommandAct()
        {

        }
    }
}
