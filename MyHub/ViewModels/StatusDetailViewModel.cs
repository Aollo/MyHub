using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using MyHub.Models;
using MyHub.Services;
using MyHub.Commands;
using Windows.UI.Popups;

namespace MyHub.ViewModels
{
    public class StatusDetailViewModel : ViewModelBase
    {
        private Status _status;
        private ObservableCollection<Status> _repostList;
        private ObservableCollection<Comment> _commentList;
        private ObservableCollection<User> _likeList;
        private int _pageNumber;
        private int _pageCount;
        private string _sinceId;
        private string _currentSelectedPivotItemName;// 当前选中的PivotItem的标题名：转发、评论、点赞
        private Status _currentSelectedRepostItem;// 当前选中的评论，用于同前台弹出菜单进行数据绑定
        private Comment _currentSelectedCommentItem;

        public StatusDetailViewModel()
        {
            CurrentSelectedPivotItemName = "评论";// 必须与前台保持一致！
            _repostList = null;
            _commentList = null;
            _likeList = null;
            InitStatusParameter();
            _currentSelectedRepostItem = null;
            _currentSelectedCommentItem = null;

            BackCommand = new RelayCommand(OnBackAppbarButtonClick);
            DeleteStatusCommand = new RelayCommand<Status>(OnDeleteStatusButtonClick, () => _status.Author.UserId == Lifecycle.AppRuntimeEnvironment.Instance.GetUserAccount(_status.Sns.Name).UserId);
            ShowStatusDetailCommand = new RelayCommand<Status>(OnShowStatusDetailCommandAct);
            RepostStatusCommand = new RelayCommand<Status>(OnRepostStatusCommandAct);
            CommentOnStatusCommand = new RelayCommand<Status>(OnCommentOnStatusCommandAct);
            FavoriteStatusCommand = new RelayCommand<Status>(OnFavoriteStatusCommandAct);
            ReplyCommentCommand = new RelayCommand<Comment>(OnReplyCommentCommandAct);
            RepostCommentCommand = new RelayCommand<Comment>(OnRepostCommentCommandAct);
            DeleteCommentCommand = new RelayCommand<Comment>(OnDeleteCommentButtonClick, () =>
            {
                var uid = Lifecycle.AppRuntimeEnvironment.Instance.GetUserAccount(_status.Sns.Name).UserId;
                return (uid == _status.Author.UserId) || (uid == _currentSelectedCommentItem.Author.UserId);
            });

            PropertyChanged += StatusDetailViewModel_PropertyChanged;
        }

        public RelayCommand BackCommand { get; private set; }

        public RelayCommand<Status> DeleteStatusCommand { get; private set; }

        public RelayCommand<Status> ShowStatusDetailCommand { get; private set; }

        public RelayCommand<Status> RepostStatusCommand { get; private set; }

        public RelayCommand<Status> CommentOnStatusCommand { get; private set; }

        public RelayCommand<Status> FavoriteStatusCommand { get; private set; }

        public RelayCommand<Comment> ReplyCommentCommand { get; private set; }

        public RelayCommand<Comment> RepostCommentCommand { get; private set; }

        public RelayCommand<Comment> DeleteCommentCommand { get; private set; }

        public Status Status
        {
            get
            {
                return _status;
            }
            set
            {
                if(_status != value)
                {
                    _status = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<Status> RepostList
        {
            get { return _repostList; }
            set
            {
                if(_repostList != value)
                {
                    _repostList = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<Comment> CommentList
        {
            get { return _commentList; }
            set
            {
                if (_commentList != value)
                {
                    _commentList = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<User> LikeList
        {
            get { return _likeList; }
            set
            {
                if (_likeList != value)
                {
                    _likeList = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string CurrentSelectedPivotItemName
        {
            get { return _currentSelectedPivotItemName; }
            set
            {
                if (_currentSelectedPivotItemName != value)
                {
                    _currentSelectedPivotItemName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Status CurrentSelectedRepostItem
        {
            get { return _currentSelectedRepostItem; }
            set
            {
                if (_currentSelectedRepostItem != value)
                {
                    _currentSelectedRepostItem = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Comment CurrentSelectedCommentItem
        {
            get { return _currentSelectedCommentItem; }
            set
            {
                if (_currentSelectedCommentItem != value)
                {
                    _currentSelectedCommentItem = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override async Task LoadState()
        {
            await base.LoadState();

            var service = ServiceLocator.Current.GetInstance<ISnsDataService>(Status.Sns.Name);
            switch (CurrentSelectedPivotItemName)
            {
                case "转发":
                    var tempRepostList = await service.GetRepostList(Status, _pageNumber.ToString(), _pageCount.ToString(), _sinceId);
                    if(tempRepostList != null)
                        RepostList = new ObservableCollection<Status>(tempRepostList);
                    if (Status.Sns.Name == "开心网")
                    {
                        Status.RepostsCount = RepostList.Count;
                    }
                    break;
                case "评论":
                    var tempCommentList = await service.GetCommentList(Status, _pageNumber.ToString(), _pageCount.ToString(), _sinceId);
                    if(tempCommentList != null)
                        CommentList = new ObservableCollection<Comment>(tempCommentList);
                    if (Status.Sns.Name == "开心网")
                    {
                        Status.CommentsCount = CommentList.Count;
                    }
                    break;
                case "点赞":
                    var tempLikeList = await service.GetLikeList(Status, _pageNumber.ToString(), _pageCount.ToString(), _sinceId);
                    if (tempLikeList != null)
                        LikeList = new ObservableCollection<User>(tempLikeList);
                    if (Status.Sns.Name == "开心网")
                    {
                        Status.AttitudesCount = LikeList.Count;
                    }
                    break;
                default:
                    break;
            }
            ++_pageNumber;
        }

        private async void StatusDetailViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "CurrentSelectedPivotItemName")
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

        private void OnBackAppbarButtonClick()
        {
            Facade.NavigationFacade.GoBack(Lifecycle.MyHubEnums.NavigationFrameType.RightPart);
        }

        private async void OnDeleteStatusButtonClick(Status status)
        {
            // 显示提示
            MessageDialog dialog = new MessageDialog("您确定要删除这条新鲜事么？", "提示");
            UICommand cmdOk = new UICommand("确定", OnDeleteStatusCommandAct, 1);
            UICommand cmdCancel = new UICommand("取消", OnDeleteStatusCommandAct, 2);
            dialog.Commands.Add(cmdOk);
            dialog.Commands.Add(cmdCancel);
            await dialog.ShowAsync();
        }

        private async void OnDeleteStatusCommandAct(IUICommand cmd)
        {
            int cmdId = (int)cmd.Id;
            if(cmdId == 1)
            {
                var service = ServiceLocator.Current.GetInstance<ISnsDataService>(_status.Sns.Name);
                await service.DeleteStatus(_status);
                OnBackAppbarButtonClick();
            }
            else
            {
                return;
            }
        }

        private void OnShowStatusDetailCommandAct(Status status)
        {
            Facade.NavigationFacade.NavigateToStatusDetailPage(status);
        }

        private void OnRepostStatusCommandAct(Status status)
        {
            Facade.NavigationFacade.NavigateToPostStatusPage(Lifecycle.MyHubEnums.NavigatedToPostStatusPageType.Repost, status);
        }


        private void OnCommentOnStatusCommandAct(Status status)
        {
            Facade.NavigationFacade.NavigateToPostStatusPage(Lifecycle.MyHubEnums.NavigatedToPostStatusPageType.Comment, status);
        }

        private async void OnFavoriteStatusCommandAct(Status status)
        {
            var service = ServiceLocator.Current.GetInstance<ISnsDataService>(status.Sns.Name);
            var result = await service.FavoriteStatus(status);
        }

        private void OnReplyCommentCommandAct(Comment comment)
        {
            if (comment.StatusInfo == null)
                comment.StatusInfo = _status;
            Facade.NavigationFacade.NavigateToPostStatusPage(Lifecycle.MyHubEnums.NavigatedToPostStatusPageType.ReplyComment, comment);
        }

        private void OnRepostCommentCommandAct(Comment comment)
        {
            if (comment.StatusInfo == null)
                comment.StatusInfo = _status;
            Facade.NavigationFacade.NavigateToPostStatusPage(Lifecycle.MyHubEnums.NavigatedToPostStatusPageType.RepostComment, comment);
        }

        private async void OnDeleteCommentButtonClick(Comment comment)
        {
            // 显示提示
            MessageDialog dialog = new MessageDialog("您确定要删除这条评论么？", "提示");
            UICommand cmdOk = new UICommand("确定", OnDeleteStatusCommandAct, 1);
            UICommand cmdCancel = new UICommand("取消", OnDeleteStatusCommandAct, 2);
            dialog.Commands.Add(cmdOk);
            dialog.Commands.Add(cmdCancel);
            await dialog.ShowAsync();
        }

        private async void OnDeleteCommentCommandAct(IUICommand cmd)
        {
            int cmdId = (int)cmd.Id;
            if (cmdId == 1)
            {
                var service = ServiceLocator.Current.GetInstance<ISnsDataService>(_currentSelectedCommentItem.StatusInfo.Sns.Name);
                var result = await service.DeleteComment(_currentSelectedCommentItem);
                if (result == true)
                    CommentList.Remove(_currentSelectedCommentItem);
            }
            else
            {
                return;
            }
        }
    }
}
