using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyHub.Models;
using MyHub.Lifecycle;
using System.Collections.ObjectModel;
using Microsoft.Practices.ServiceLocation;
using MyHub.Services;

namespace MyHub.ViewModels
{
    public class NotificationCenterViewModel : ViewModelBase
    {
        private ObservableCollection<BasicNotificationMessage> _messageList;
        private ObservableCollection<string> _snsTypes;
        private string _currentSelectedSnsType;
        private NotificationMessageType _currentSelectedMessageType;
        private int _pageNumber;
        private int _pageCount;
        private string _sinceId;

        public NotificationCenterViewModel()
        {
            SnsTypes = new ObservableCollection<string>();
            var accounts = AppRuntimeEnvironment.Instance.GetAllUserAccount();
            if (accounts.Length > 1)
                SnsTypes.Add("整合显示");
            foreach (Account a in accounts)
                SnsTypes.Add(a.Sns.Name);

            _messageList = null;
            _currentSelectedSnsType = SnsTypes.FirstOrDefault();
            _currentSelectedMessageType = NotificationMessageType.Comments;// 默认是评论的，需要和界面保持一致
            InitStatusParameter();

            PropertyChanged += NotificationCenterViewModel_PropertyChanged;
        }

        public ObservableCollection<string> SnsTypes
        {
            get
            {
                return _snsTypes;
            }
            private set
            {
                if (_snsTypes != value)
                {
                    _snsTypes = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<BasicNotificationMessage> MessageList
        {
            get { return _messageList; }
            private set
            {
                if (_messageList != value)
                {
                    _messageList = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string CurrentSelectedSnsType
        {
            get { return _currentSelectedSnsType; }
            set
            {
                if (_currentSelectedSnsType != value)
                {
                    _currentSelectedSnsType = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public NotificationMessageType CurrentSelectedMessageType
        {
            get { return _currentSelectedMessageType; }
            set
            {
                if(_currentSelectedMessageType != value)
                {
                    _currentSelectedMessageType = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public override async Task LoadState()
        {
            await base.LoadState();

            if (_messageList == null) _messageList = new ObservableCollection<BasicNotificationMessage>();
            _messageList.Clear();

            if(CurrentSelectedSnsType == "整合显示")
            {
                var services = ServiceLocator.Current.GetAllInstances<ISnsDataService>();
                foreach(ISnsDataService service in services)
                    MessageList = await LoadNotificationMessage(service, CurrentSelectedMessageType);
            }
            else// 获取单个社交网络的通知信息
            {
                var service = ServiceLocator.Current.GetInstance<ISnsDataService>(CurrentSelectedSnsType);
                MessageList = await LoadNotificationMessage(service, CurrentSelectedMessageType);
            }
            ++_pageNumber;
        }

        private async void NotificationCenterViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentSelectedSnsType" 
                || e.PropertyName == "CurrentSelectedMessageType")
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

        /// <summary>
        /// 获取指定网络指定类型的信息
        /// </summary>
        /// <param name="service"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private async Task<ObservableCollection<BasicNotificationMessage>> LoadNotificationMessage(ISnsDataService service, NotificationMessageType type)
        {
            switch (type)
            {
                case NotificationMessageType.Mentions:
                    var tempMentionsStatusList = await service.GetMentionsStatuses(_pageNumber.ToString(), _pageCount.ToString(), _sinceId);
                    if (tempMentionsStatusList != null)
                        foreach (Status s in tempMentionsStatusList)
                            _messageList.Add(ConvertToBasicNotificationMessage(s));
                    var tempMentionsCommentsList = await service.GetMentionsComments(_pageNumber.ToString(), _pageCount.ToString(), _sinceId);
                    if (tempMentionsCommentsList != null)
                        foreach (Comment c in tempMentionsCommentsList)
                            _messageList.Add(ConvertToBasicNotificationMessage(c));
                    _messageList = new ObservableCollection<BasicNotificationMessage>(_messageList.OrderBy(i => i.CreateTime));
                    MessageList = new ObservableCollection<BasicNotificationMessage>(_messageList.Reverse());// 使用公共访问器是为了引发通知
                    break;
                case NotificationMessageType.Comments:
                    var tempCommentsToMeList = await service.GetCommentsToMe(_pageNumber.ToString(), _pageCount.ToString(), _sinceId);
                    if (tempCommentsToMeList != null)
                        foreach (Comment c in tempCommentsToMeList)
                            _messageList.Add(ConvertToBasicNotificationMessage(c));
                    var tempCommentsFromMeList = await service.GetCommentsFromMe(_pageNumber.ToString(), _pageCount.ToString(), _sinceId);
                    if (tempCommentsFromMeList != null)
                        foreach (Comment c in tempCommentsFromMeList)
                            _messageList.Add(ConvertToBasicNotificationMessage(c));
                    _messageList = new ObservableCollection<BasicNotificationMessage>(_messageList.OrderBy(i => i.CreateTime));
                    MessageList = new ObservableCollection<BasicNotificationMessage>(_messageList.Reverse());// 使用公共访问器是为了引发通知
                    break;
                case NotificationMessageType.Likes:
                    MessageList = null;// 目前所有社交网络都不支持获取点赞通知消息
                    break;
                default:
                    break;
            }

            return MessageList;
        }

        private BasicNotificationMessage ConvertToBasicNotificationMessage(Status s)
        {
            if (s == null)
                return null;

            BasicNotificationMessage temp = new BasicNotificationMessage
            {
                Sns = s.Sns,
                MessageType = CurrentSelectedMessageType,
                MessageId = s.StatusId,
                CreateTime = s.CreateTime,
                Content = s.Content,
                Author = s.Author,
                Source = s.Source,
                OriginalStatus = s.RetweetedStatus
            };

            return temp;
        }

        private BasicNotificationMessage ConvertToBasicNotificationMessage(Comment c)
        {
            if (c == null)
                return null;

            BasicNotificationMessage temp = new BasicNotificationMessage
            {
                Sns = c.StatusInfo.Sns,
                MessageType = CurrentSelectedMessageType,
                MessageId = c.CommentId,
                CreateTime = c.CreateTime,
                Content = c.Content,
                Author = c.Author,
                Source = c.Source,
                OriginalStatus = c.StatusInfo
            };

            return temp;
        }
    }
}
