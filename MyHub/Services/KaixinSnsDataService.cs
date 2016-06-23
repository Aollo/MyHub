using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyHub.Models;
using MyHub.Models.Kaixin;
using MyHub.Tools;
using KaixinSDK.Entities;

namespace MyHub.Services
{
    public class KaixinSnsDataService : ISnsDataService
    {
        private string _access_token;
        private int _number;
        private int _count;
        private string _objtype;// 类型一直为记录
        private string _fields;// 获取用户信息时的属性字段
        private string _snsName;

        public KaixinSnsDataService()
        {
            _snsName = "开心网";
            _objtype = "records";
            _fields = "uid,name,gender,city,status,logo50,intro";
            _number = _count = -1;

            Lifecycle.AppRuntimeEnvironment.Instance.PropertyChanged += UserAccount_PropertyChanged;

            InitEntities();
            DoDynamicInit();
        }

        public Account Account { get; private set; }
        public List<Status> StatusList { get; private set; }
        public SortedList<DateTime, Status> SortingStatusList { get; private set; }
        public List<Comment> CommentList { get; private set; }
        public List<User> UserList { get; private set; }
        public List<string> TopicsList { get; private set; }

        /// <summary>
        /// 初始化实体变量
        /// </summary>
        public void InitEntities()
        {
            Account = new Account();
            StatusList = new List<Status>();
            SortingStatusList = new SortedList<DateTime, Status>();
            CommentList = new List<Comment>();
            UserList = new List<User>();
            TopicsList = new List<string>();
        }

        /// <summary>
        /// 做运行时初始化，为了使用单例模式并确保Account在运行中更新时不出问题
        /// </summary>
        private void DoDynamicInit()
        {
            Account = Lifecycle.AppRuntimeEnvironment.Instance.GetUserAccount(_snsName);
            if (Account == null)// 如果不能从已经登陆的账户中获取本社交网络的账户，就从没有登录的账户中获取
                Account = Lifecycle.AppRuntimeEnvironment.Instance.GetUserAccountUnlogin(_snsName);
            _access_token = Account.AccessToken;
        }

        private void UserAccount_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "UserAccount")
            {
                DoDynamicInit();
            }
        }

        #region BasicSocialNetworkServices

        /// <summary>
        /// 获取用户好友最新的与用户自己发布的最新的记录中pageNumber与pageCount指定的条数
        /// </summary>
        public async Task<IList<Status>> GetStatus(string pageNumber, string pageCount, string sinceId)
        {
            //DoDynamicInit();
            StatusList.Clear();
            SortingStatusList.Clear();
            Status _status;

            // 获取好友的最新动态
            var friendEntity = await KaixinSnsDataAccessMethods.GetFriendsRecords(_access_token, ConvertToRecordStartNumber(pageNumber, pageCount), _count, null);

            if (friendEntity.Error_Code != 0)// 错误处理
                return null;

            foreach (FriendsRecordsEntity r in friendEntity.FriendsRecordsList)
            {
                _status = ConvertToStatus(r);
                if (SortingStatusList.ContainsKey(_status.CreateTime)) continue;// 首先检查是否有重复的key
                SortingStatusList.Add(_status.CreateTime, _status);
            }

            // 获取自己的最新动态
            var myEntity = await KaixinSnsDataAccessMethods.GetUserHomeRecords(_access_token, null, ConvertToRecordStartNumber(pageNumber, pageCount), _count, null);

            if (myEntity.Error_Code != 0)// 错误处理，如果获取自己的信息出错，返回之前获取到的好友的状态
                goto END;

            foreach (FriendsRecordsEntity r in myEntity.FriendsRecordsList)
            {
                _status = ConvertToStatus(r);
                if (SortingStatusList.ContainsKey(_status.CreateTime)) continue;// 首先检查是否有重复的key
                SortingStatusList.Add(_status.CreateTime, _status);
            }
            
            // 将排序好的所有记录转移到StatusList中，方便统一获取
            END:
            for(int i = SortingStatusList.Count - 1; i >= 0 && _count > 0; --i)// 只提取前_count条数据
            {
                StatusList.Add(SortingStatusList.Values[i]);
                --_count;
            }

            return StatusList;
        }

        ///// <summary>
        ///// 只是获取用户好友最新的记录中pageNumber与pageCount指定的条数
        ///// </summary>
        //public async Task<IList<Status>> GetStatus(string pageNumber, string pageCount, string sinceId)
        //{
        //    StatusList.Clear();

        //    var friendEntity = await KaixinSnsDataAccessMethods.GetFriendsRecords(_access_token, ConvertToRecordStartNumber(pageNumber, pageCount), _count, null);

        //    if (friendEntity.Error_Code != 0)// 错误处理
        //        return null;

        //    foreach (FriendsRecordsEntity r in friendEntity.FriendsRecordsList)
        //    {
        //        StatusList.Add(ConvertToStatus(r));
        //    }

        //    return StatusList;
        //}

        public async Task<Status> PostStatus(Status status)
        {
            //DoDynamicInit();
            return await PostStatus(status.Content, status.LocationInfo.Longitude, status.LocationInfo.Latitude, null);
        }

        public async Task<Status> PostStatus(string content, string longtitude, string lantitude, Windows.Storage.StorageFile picture)
        {
            //DoDynamicInit();
            Status result;

            var entity = await KaixinSnsDataAccessMethods.PostRecord(_access_token, content, "", null, null, 1, null, picture);
            if (entity.Error_Code == 0)// 记录发布成功
            {
                var recordEntity = await KaixinSnsDataAccessMethods.GetUserHomeRecords(_access_token, null, 0, 1, null);// 将用户刚发布的记录获取过来

                if (recordEntity.Error_Code != 0)
                    return null;

                if (recordEntity.FriendsRecordsList[0].Rid == entity.Rid)
                    result = ConvertToStatus(recordEntity.FriendsRecordsList[0]);
                else// 获取的记录错误
                    result = null;
            }
            else
                result = null;

            return result;
        }

        public async Task<bool?> DeleteStatus(Status status)
        {
            //DoDynamicInit();
            var entity = await KaixinSnsDataAccessMethods.DeleteRecord(_access_token, status.StatusId);
            return (entity.result == "1");
        }

        public async Task<Status> RepostStatus(Status originalStatus, string content, string isComment)
        {
            //DoDynamicInit();
            Status result;

            var entity = await KaixinSnsDataAccessMethods.CreateForwardForSource(_access_token, _objtype, originalStatus.StatusId, originalStatus.Author.UserId, content);
            if(entity.Fid != null)
            {
                var recordEntity = await KaixinSnsDataAccessMethods.GetUserHomeRecords(_access_token, null, 0, 1, null);// 将用户刚发布的记录获取过来

                if(recordEntity.FriendsRecordsList[0].Rid == entity.Fid)
                    result = ConvertToStatus(recordEntity.FriendsRecordsList[0]);
                else// 获取的记录错误
                    result = null;
            }
            else
            {
                result = null;
            }

            return result;
        }

        public async Task<Comment> CommentStatus(Status originalStatus, string content, string isComment)
        {
            //DoDynamicInit();
            Comment result;

            var entity = await KaixinSnsDataAccessMethods.CreateCommentForSource(_access_token, originalStatus.StatusId, originalStatus.Author.UserId, content, _objtype, null);
            if(entity.Error_Code == 0)
            {
                // 获取我发出的评论
                var commentEntity = await KaixinSnsDataAccessMethods.GetMyRepliedCommentList(_access_token, 0, 1);
                if (commentEntity.Data[0].Thread_cid == entity.Data.Thread_cid)// 取出最新的一条评论
                {
                    result = ConvertToComment(commentEntity.Data[0]);
                    
                    // 设置发表评论的用户的信息
                    var userEntity = await KaixinSnsDataAccessMethods.GetUserInfoList(_access_token, commentEntity.Data[0].Uid, _fields);
                    result.Author = ConvertToUser(userEntity.FriendList[0]);
                    // 设置被评论资源的信息
                    result.StatusInfo = originalStatus;
                }
                else
                    result = null;
            }
            else
            {
                result = null;
            }

            return result;
        }

        /// <summary>
        /// 点赞方法
        /// </summary>
        /// <param name="originalStatus"></param>
        /// <returns>true点赞成功，false取消点赞成功，null操作失败</returns>
        public async Task<bool?> LikeStatus(Status originalStatus)
        {
            //DoDynamicInit();
            bool? result = null;

            var entity = await KaixinSnsDataAccessMethods.CheckLikeForSource(_access_token, _objtype, originalStatus.StatusId, originalStatus.Author.UserId, Account.UserId);
            if (entity == null)
                return null;
            if (entity.data[0].liked == "0")// 没有点赞，进行点赞
            {
                var entityLike = await KaixinSnsDataAccessMethods.CreateLikeForSource(_access_token, _objtype, originalStatus.StatusId, originalStatus.Author.UserId);
                if (entity == null)
                    return null;
                result = entityLike.Data.Result == "1" ? true : result;
            }
            else// 取消点赞
            {
                var entityUnlike = await KaixinSnsDataAccessMethods.CancelLikeForSource(_access_token, _objtype, originalStatus.StatusId, originalStatus.Author.UserId);
                if (entity == null)
                    return null;
                result = entityUnlike.Data.Result == "1" ? false : result;
            }

            return result;
        }

        /// <summary>
        /// 【开心网不支持】
        /// </summary>
        /// <param name="originalStatus"></param>
        /// <returns></returns>
        public async Task<bool?> FavoriteStatus(Status originalStatus)
        {
            return null;
        }

        public async Task<IList<Status>> GetRepostList(Status originalStatus, string pageNumber, string pageCount, string sinceId)
        {
            //DoDynamicInit();
            StatusList.Clear();

            var entity = await KaixinSnsDataAccessMethods.GetForwardList(_access_token, _objtype, originalStatus.StatusId, originalStatus.Author.UserId, ConvertToRecordStartNumber(pageNumber, pageCount), _count);

            if (entity.Error_Code != 0)// 错误处理
                return null;

            foreach (ForwardRecordEntity r in entity.Forwardlist)
            {
                var entityStatus = ConvertToStatus(r);
                entityStatus.RetweetedStatus = originalStatus;

                // 获取该转发记录对应用户发布的所有记录，通过CTime找到对应转发的那条记录，将Rid设置到StatusId
                var entityUserRecord = await KaixinSnsDataAccessMethods.GetUserHomeRecords(_access_token, Convert.ToInt32(entityStatus.Author.UserId), 0, 20, null);
                entityStatus.StatusId = "";// 默认为不可用状态
                foreach(FriendsRecordsEntity fr in entityUserRecord.FriendsRecordsList)
                {
                    if (fr.Ctime == r.Ctime)
                    {
                        entityStatus.StatusId = fr.Rid;
                        break;
                    }
                }

                StatusList.Add(entityStatus);
            }

            return StatusList;
        }

        public async Task<IList<Comment>> GetCommentList(Status originalStatus, string pageNumber, string pageCount, string sinceId)
        {
            //DoDynamicInit();
            CommentList.Clear();

            var entity = await KaixinSnsDataAccessMethods.GetCommentList(_access_token, originalStatus.StatusId, originalStatus.Author.UserId, _objtype, ConvertToRecordStartNumber(pageNumber, pageCount), _count);

            if (entity.Error_Code != 0)// 错误处理
                return null;

            foreach (CommentListDataEntity c in entity.Data)
            {
                CommentList.Add(ConvertToComment(c));
            }

            return CommentList;
        }

        public async Task<IList<User>> GetLikeList(Status originalStatus, string pageNumber, string pageCount, string sinceId)
        {
            //DoDynamicInit();
            UserList.Clear();

            var entity = await KaixinSnsDataAccessMethods.GetLikeUserList(_access_token, _objtype, originalStatus.StatusId, originalStatus.Author.UserId, ConvertToRecordStartNumber(pageNumber, pageCount), _count);

            if (entity.Error_Code != 0)// 错误处理
                return null;

            foreach (ShowLikeUserDataEntity u in entity.Data)
            {
                UserList.Add(ConvertToUser(u));
            }

            return UserList;
        }

        public async Task<Comment> ReplyComment(Comment comment, string content)
        {
            //DoDynamicInit();
            Comment result;

            var entity = await KaixinSnsDataAccessMethods.ReplyCommentForSource(
                                _access_token, 
                                comment.StatusInfo.StatusId, 
                                comment.Author.UserId, 
                                comment.CommentId, 
                                comment.StatusInfo.Author.UserId, 
                                content, 
                                _objtype);

            if(entity.Error_Code == 0)
            {
                result = new Comment
                {
                    CommentId = entity.Data.Cid,
                    CreateTime = DateTime.Now.Date + DateTime.Now.TimeOfDay,
                    Content = content,
                    Source = _snsName,
                    Author = new User(),
                    StatusInfo = comment.StatusInfo,
                    ReplyComment = comment
                };

                // 设置当前登陆用户的用户信息
                var userEntity = await KaixinSnsDataAccessMethods.GetUserInfo(_access_token, _fields);
                result.Author = ConvertToUser(userEntity);
            }
            else
            {
                result = null;
            }

            return result;
        }

        public async Task<bool?> DeleteComment(Comment comment)
        {
            //DoDynamicInit();
            var entity = await KaixinSnsDataAccessMethods.DeleteCommentForSource(_access_token, comment.StatusInfo.Author.UserId, comment.CommentId);

            return (entity.Data.Result == "1");
        }

        #endregion BasicSocialNetworkServices

        #region NotificationSerivces

        public async Task<Remind> GetUnreadCount()
        {
            //DoDynamicInit();
            var entity = await KaixinSnsDataAccessMethods.GetUnreadMessageCount(_access_token);
            return ConvertToRemind(entity);
        }

        public async Task<bool?> ClearUnreadCount(Remind type)
        {
            //DoDynamicInit();
            var entity = await KaixinSnsDataAccessMethods.ClearUnreadMessageCount(_access_token, ConvertToClearRemindType(type));
            return (entity.result == "1");
        }

        /// <summary>
        /// 【由于开心网返回的信息与自己的数据结构差距太大，所以此方法存在问题】
        /// </summary>
        public async Task<IList<Comment>> GetCommentsToMe(string pageNumber, string pageCount, string sinceId)
        {
            //DoDynamicInit();
            CommentList.Clear();

            var entity = await KaixinSnsDataAccessMethods.GetCommentListForMe(_access_token, ConvertToRecordStartNumber(pageNumber, pageCount), _count);

            if (entity.Error_Code != 0)// 错误处理
                return null;

            foreach (ReceivedCommentListDataEntity c in entity.Data)
            {
                CommentList.Add(ConvertToComment(c));
            }

            return CommentList;
        }

        /// <summary>
        /// 【由于开心网返回的信息与自己的数据结构差距太大，所以此方法存在问题】
        /// </summary>
        public async Task<IList<Comment>> GetCommentsFromMe(string pageNumber, string pageCount, string sinceId)
        {
            //DoDynamicInit();
            CommentList.Clear();

            var entity = await KaixinSnsDataAccessMethods.GetMyRepliedCommentList(_access_token, ConvertToRecordStartNumber(pageNumber, pageCount), _count);

            if (entity.Error_Code != 0)// 错误处理
                return null;

            foreach (RepliedCommentListDataEntity c in entity.Data)
            {
                var comment = ConvertToComment(c);
                // 设置用户的基本信息
                var userEntity = await KaixinSnsDataAccessMethods.GetUserInfo(_access_token, _fields);
                comment.Author = ConvertToUser(userEntity);
                // 设置被评论的资源的基本信息
                comment.StatusInfo.Sns = Account.Sns;
                comment.StatusInfo.Content = c.Source.Title;
                comment.StatusInfo.Author.UserId = c.Source.Uid;
                comment.StatusInfo.Author.NickName = c.Source.Name;

                CommentList.Add(comment);
            }

            return CommentList;
        }

        /// <summary>
        /// 【开心网不支持】
        /// </summary>
        public async Task<IList<Comment>> GetMentionsComments(string pageNumber, string pageCount, string sinceId)
        {
            return null;
        }

        /// <summary>
        /// 【开心网不支持】
        /// </summary>
        public async Task<IList<Status>> GetMentionsStatuses(string pageNumber, string pageCount, string sinceId)
        {
            return null;
        }

        #endregion NotificationSerivces

        #region ExploreAndSearchServices

        public async Task<IList<Status>> GetPublicStatus(string pageNumber, string pageCount)
        {
            //DoDynamicInit();
            StatusList.Clear();

            var entity = await KaixinSnsDataAccessMethods.GetPublicRecords(_access_token, ConvertToRecordStartNumber(pageNumber, pageCount), _count);

            if (entity.Error_Code != 0)// 错误处理
                return null;

            foreach(FriendsRecordsEntity r in entity.FriendsRecordsList)
            {
                StatusList.Add(ConvertToStatus(r));
            }

            return StatusList;
        }

        /// <summary>
        /// 【开心网不支持】
        /// </summary>
        public async Task<IList<Status>> GetLocalStatus(string latitude, string longitude, string range, string pageNumber, string pageCount)
        {
            return null;
        }

        public async Task<IList<string>> GetHotTopics()
        {
            //DoDynamicInit();
            TopicsList.Clear();

            var entity = await KaixinSnsDataAccessMethods.GetHotTopics(_access_token);
            foreach (KaixinHotTopicEntity t in entity)
            {
                TopicsList.Add(t.title);
            }

            return TopicsList;
        }

        /// <summary>
        /// 【开心网不支持】
        /// </summary>
        public async Task<IList<Status>> SearchStatusesWithTopic(string topic)
        {
            return null;
        }

        /// <summary>
        /// 【开心网不支持】
        /// </summary>
        public async Task<IList<Status>> SearchStatuses(string key)
        {
            return null;
        }

        /// <summary>
        /// 【开心网不支持】
        /// </summary>
        public async Task<IList<User>> SearchUsers(string key)
        {
            return null;
        }

        /// <summary>
        /// 【开心网不支持】
        /// </summary>
        public async Task<IList<string>> SearchTopics(string topic)
        {
            return null;
        }

        /// <summary>
        /// 【开心网不用实现】
        /// </summary>
        public async Task<IList<Location>> GetNearbyPositions(string latitude, string longitude, string range, string keyword, string count, string page)
        {
            return null;
        }

        /// <summary>
        /// 【开心网不用实现】
        /// </summary>
        public async Task<IList<Location>> SearchLocation(string keyword, string pageNumber, string pageCount)
        {
            return null;
        }

        /// <summary>
        /// 【开心网不用实现】
        /// </summary>
        public Task<string> ShortenUrl(string originalUrl)
        {
            return null;
        }

        /// <summary>
        /// 【开心网不用实现】
        /// </summary>
        public Task<string> GetShortUrlExpanded(string urlShort)
        {
            return null;
        }

        #endregion ExploreAndSearchServices

        #region UserHomeServices

        public async Task<UserProfile> GetUserProfile(string userId, string screenName)
        {
            //DoDynamicInit();
            if (string.IsNullOrWhiteSpace(userId))// 默认获取当前登录用户的资料
            {
                var entity = await KaixinSnsDataAccessMethods.GetUserInfo(_access_token, _fields);
                return ConvertToUserProfile(entity);
            }
            else// 如果指定了uid则会获取指定的用户资料
            {
                var entity = await KaixinSnsDataAccessMethods.GetUserInfoList(_access_token, userId, _fields);
                return ConvertToUserProfile(entity.FriendList[0]);
            }
        }

        /// <summary>
        /// 只能获取当前登录用户的好友资料，其他人无法获取
        /// </summary>
        public async Task<IList<User>> GetUserFriends(string userId, string screenName)
        {
            //DoDynamicInit();
            UserList.Clear();

            var entity = await KaixinSnsDataAccessMethods.GetFriendList(_access_token, _fields, 0, 50);

            if (entity.Error_Code != 0)// 错误处理
                return null;

            foreach (FriendEntity u in entity.FriendList)
            {
                UserList.Add(ConvertToUser(u));
            }

            return UserList;
        }

        public async Task<IList<Status>> GetUserHomeStatus(string userId, string screenName, string pageNumber, string pageCount, string sinceId)
        {
            //DoDynamicInit();
            StatusList.Clear();

            int? _uid = null;
            if (!string.IsNullOrWhiteSpace(userId))
                _uid = Convert.ToInt32(userId);

            var entity = await KaixinSnsDataAccessMethods.GetUserHomeRecords(_access_token, _uid, ConvertToRecordStartNumber(pageNumber, pageCount), _count, null);

            if (entity.Error_Code != 0)// 错误处理
                return null;

            foreach (FriendsRecordsEntity r in entity.FriendsRecordsList)
            {
                StatusList.Add(ConvertToStatus(r));
            }

            return StatusList;
        }

        /// <summary>
        /// 【开心网不支持】
        /// </summary>
        public async Task<IList<Favorite>> GetUserFavorites(string pageNumber, string pageCount)
        {
            return null;
        }

        /// <summary>
        /// 开心网不支持
        /// </summary>
        public async Task<IList<Status>> GetUserLikes(string pageNumber, string pageCount, string sinceId)
        {
            return null;
        }

        #endregion UserHomeServices


        #region ToolFunctions

        private Status ConvertToStatus(FriendsRecordsEntity r)
        {
            if (r == null || r.Ctime == null)
                return null;

            Status temp = new Status
            {
                Sns = Account == null ? new SnsType() { Name = _snsName } : Account.Sns,
                StatusId = r.Rid,
                Author = ConvertToUser(r.User),
                Content = r.Main.Content,
                CreateTime = TimeConverter.ConvertToDateTime(r.Ctime),
                Source = _snsName,
                ThumbnailPicUrl = r.Main.Pics.Src,
                OriginalPicUrl = r.Main.Pics.Src,
                PictureUrls = r.Main.Pics.Src == null ? null : new List<string>() { r.Main.Pics.Src },
                Favorited = false,
                RepostsCount = 0,
                CommentsCount = 0,
                AttitudesCount = 0,
                LocationInfo = null,
                RetweetedStatus = ConvertToRetweetedStatus(r.Source)
            };

            return temp;
        }

        /// <summary>
        /// 将开心网的 转发记录列表的一条转发记录实体ForwardRecordEntity转化成Status实体
        /// RetweetedStatus字段没有设置，在调用方设置
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private Status ConvertToStatus(ForwardRecordEntity r)
        {
            if (r == null || r.Ctime == null)
                return null;

            Status temp = new Status
            {
                Sns = Account == null ? new SnsType() { Name = _snsName } : Account.Sns,
                //StatusId = 
                Author = ConvertToUser(r),
                Content = r.Content,
                CreateTime = TimeConverter.ConvertToDateTime(r.Ctime),
                Source = _snsName,
                ThumbnailPicUrl = null,
                OriginalPicUrl = null,
                PictureUrls = null,
                Favorited = false,
                RepostsCount = -1,
                CommentsCount = -1,
                AttitudesCount = -1,
                LocationInfo = null,
                RetweetedStatus = new Status()
            };

            return temp;
        }

        /// <summary>
        /// 将开心网记录中的Source字段转化成Status实体中的RetweetedStatus字段
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private Status ConvertToRetweetedStatus(FriendsRecords_Source_Entity s)
        {
            if (s == null || s.Ctime == null)
                return null;

            Status temp = new Status
            {
                Sns = Account == null ? new SnsType() { Name = _snsName } : Account.Sns,
                StatusId = s.Rid,
                Author = ConvertToUser(s.User),
                Content = s.Main.Content,
                CreateTime = TimeConverter.ConvertToDateTime(s.Ctime),
                Source = _snsName,
                ThumbnailPicUrl = s.Main.Pics.Src,
                OriginalPicUrl = s.Main.Pics.Src,
                PictureUrls = s.Main.Pics.Src == null ? null : new List<string>() { s.Main.Pics.Src },
                Favorited = false,
                RepostsCount = Convert.ToInt32(s.Rnum),
                CommentsCount = Convert.ToInt32(s.Cnum),
                AttitudesCount = 0,
                LocationInfo = null,
                RetweetedStatus = null
            };

            return temp;
        }

        private User ConvertToUser(FriendsRecords_User_Entity u)
        {
            if (u == null || u.Uid == null)
                return null;

            User temp = new User
            {
                Sns = Account == null ? new SnsType() { Name = _snsName } : Account.Sns,
                UserId = u.Uid,
                NickName = u.Name,
                RemarkName = "",
                LogoUrl = u.Logo50,
                Verified = false,
                Following = true
            };

            return temp;
        }

        private User ConvertToUser(ShowLikeUserDataEntity u)
        {
            if (u == null || u.Uid == null)
                return null;

            User temp = new User
            {
                Sns = Account == null ? new SnsType() { Name = _snsName } : Account.Sns,
                UserId = u.Uid,
                NickName = u.Name,
                RemarkName = "",
                LogoUrl = u.Logo50,
                Verified = false,
                Following = true
            };

            return temp;
        }

        private User ConvertToUser(FriendEntity f)
        {
            if (f == null || f.Name == null)
                return null;

            User temp = new User
            {
                Sns = Account == null ? new SnsType() { Name = _snsName } : Account.Sns,
                UserId = f.Uid.ToString(),
                NickName = f.Name,
                RemarkName = "",
                LogoUrl = f.Logo50,
                Verified = false,
                Following = true
            };

            return temp;
        }

        private User ConvertToUser(ForwardRecordEntity r)
        {
            if (r == null || r.Uid == null)
                return null;

            User temp = new User
            {
                Sns = Account == null ? new SnsType() { Name = _snsName } : Account.Sns,
                UserId = r.Uid,
                NickName = r.Name,
                RemarkName = "",
                LogoUrl = r.Logo50,
                Verified = false,
                Following = true,
            };

            return temp;
        }

        private User ConvertToUser(ReceivedCommentListDataEntity c)
        {
            if (c == null || c.Uid == null)
                return null;

            User temp = new User
            {
                Sns = Account == null ? new SnsType() { Name = _snsName } : Account.Sns,
                UserId = c.Uid,
                NickName = c.Name,
                RemarkName = "",
                LogoUrl = c.Logo50,
                Verified = false,
                Following = true
            };

            return temp;
        }

        private User ConvertToUser(CommentListDataEntity c)
        {
            if (c == null || c.Uid == null)
                return null;

            User temp = new User
            {
                Sns = Account == null ? new SnsType() { Name = _snsName } : Account.Sns,
                UserId = c.Uid,
                NickName = c.Real_name,
                RemarkName = "",
                LogoUrl = c.Logo50,
                Verified = false,
                Following = true
            };

            return temp;
        }
        

        private UserProfile ConvertToUserProfile(FriendEntity f)
        {
            if (f == null || f.Name == null)
                return null;

            UserProfile temp = new UserProfile
            {
                BasicUserInfo = ConvertToUser(f),
                LogoLargeUrl = f.Logo120,
                Gender = f.Gender == 0 ? "男" : "女",
                Signature = f.Intro,
                Description = "",
                Location = f.City,
                FollowerCount = -1,
                FriendsCount = -1,
                StatusesCount = f.Status
            };

            if (temp.LogoLargeUrl == null)
                temp.LogoLargeUrl = temp.BasicUserInfo.LogoUrl;

            return temp;
        }

        private Comment ConvertToComment(CommentListDataEntity c)
        {
            if (c == null || c.Uid == null)
                return null;

            Comment temp = new Comment
            {
                Sns = new SnsType() { Name = _snsName },
                CommentId = c.Thread_cid,
                CreateTime = TimeConverter.ConvertToDateTime(c.Ctime),
                Content = c.Content,
                Source = _snsName,
                Author = ConvertToUser(c),
                StatusInfo = null,
                ReplyComment = ConvertToComment(c.Replys)
            };

            return temp;
        }

        /// <summary>
        /// 将开心网获取评论列表中列表项CommentListDataEntity中的CommentEntity类型的Replys字段转换成Comment类型的ReplyComment属性
        /// </summary>
        private Comment ConvertToComment(CommentEntity c)
        {
            if (c == null || c.Uid == null)
                return null;

            Comment temp = new Comment
            {
                Sns = new SnsType() { Name = _snsName },
                CommentId = c.Cid,
                CreateTime = TimeConverter.ConvertToDateTime(c.Ctime),
                Content = c.Content,
                Source = _snsName,
                Author = new User()
                {
                    Sns = Account.Sns,
                    UserId = c.Uid,
                    NickName = c.Name,
                    RemarkName = "",
                    LogoUrl = c.Logo50,
                    Following = true,
                    Verified = false
                },
                StatusInfo = null,
                ReplyComment = null
            };

            return temp;
        }

        /// <summary>
        /// 将开心网的评论实体RepliedCommentListDataEntity转化成Comment实体
        /// 注意：用户信息Author和被评论的记录的信息StatusInfo没有设置，在调用方设置
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private Comment ConvertToComment(RepliedCommentListDataEntity c)
        {
            if (c == null || c.Uid == null)
                return null;

            Comment temp = new Comment
            {
                Sns = new SnsType() { Name = _snsName },
                CommentId = c.Thread_cid,
                CreateTime = TimeConverter.ConvertToDateTime(c.Ctime),
                Content = c.Content,
                Source = _snsName,
                Author = new User(),
                StatusInfo = new Status
                {
                    Sns = new SnsType(),
                    Author = new User(),
                    RetweetedStatus = new Status()
                },
                ReplyComment = null
            };
            
            return temp;
        }

        /// <summary>
        /// 将用户收到的评论列表项ReceivedCommentListDataEntity实体转换成Comment实体
        /// 注意：此方法存在一些问题
        /// </summary>
        private Comment ConvertToComment(ReceivedCommentListDataEntity c)
        {
            if (c == null || c.Uid == null)
                return null;

            Comment temp = new Comment
            {
                Sns = new SnsType() { Name = _snsName },
                CommentId = c.Thread_cid,
                CreateTime = TimeConverter.ConvertToDateTime(c.Ctime),
                Content = c.Content,
                Source = _snsName,
                Author = ConvertToUser(c),
                StatusInfo = new Status(),
                ReplyComment = null
            };

            // 设置StatusInfo信息
            temp.StatusInfo.Sns = Account.Sns;
            temp.StatusInfo.StatusId = c.Source.Objid;
            temp.StatusInfo.Content = c.Source.Title;

            if(c.Replys.Cid != null)// 设置ReplyComment信息
            {
                temp.ReplyComment = new Comment
                {
                    Author = new User
                    {
                        Sns = Account.Sns,
                        UserId = c.Replys.Uid,
                        NickName = c.Replys.Name,
                        RemarkName = "",
                        LogoUrl = c.Replys.Logo25,
                        Verified = false,
                        Following = true
                    },
                    CommentId = c.Replys.Cid,
                    Content = c.Content,
                    CreateTime = TimeConverter.ConvertToDateTime(c.Ctime),
                    Source = _snsName,
                    StatusInfo = null,
                    ReplyComment = null
                };
            }

            return temp;
        }

        private Remind ConvertToRemind(KaixinMessageSummaryEntity m)
        {
            if (m == null)
                return null;

            Remind temp = new Remind
            {
                Sns = Account == null ? new SnsType() { Name = _snsName } : Account.Sns,
                Status = Convert.ToInt32(m.message),
                Comment = Convert.ToInt32(m.comment),
                MentionStatus = Convert.ToInt32(m.sysmsg_forward),
                MentionComment = Convert.ToInt32(m.reply),
                Notice = Convert.ToInt32(m.sysmsg_notice)
            };

            return temp;
        }

        /// <summary>
        /// 将Remind类型的type转化成开心网的msg_type字符串
        /// </summary>
        private string ConvertToClearRemindType(Remind type)
        {
            if (type == null)
                return null;

            string _type = "";
            if (type.Status == -1) _type += "message";
            if (type.Comment == -1) _type += ",comment";
            if (type.MentionStatus == -1) _type += ",sysmsg_forward";
            if (type.MentionComment == -1) _type += ",reply";
            if (type.Notice == -1) _type += ",notice";

            return _type;
        }

        /// <summary>
        /// 将参数pageNumber转化为开心网的记录开始展示的条数start
        /// </summary>
        /// <param name="pageNumber"></param>
        private int ConvertToRecordStartNumber(string pageNumber, string pageCount)
        {
            _number = Convert.ToInt32(pageNumber);
            _count = Convert.ToInt32(pageCount);
            return (_number - 1) * _count;
        }

        #endregion ToolFunctions


        #region 测试语句

        //private async void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    string number = "1";
        //    string count = "10";
        //    string content = "MyHub test content";
        //    //string latitude = "37.5193220000";
        //    //string longitude = "122.1282450000";
        //    //string accessTokenKaixin = kaixinClientOAuth.Access_Token;
        //    string accessTokenKaixin = "159068599_e7836224f6f3c018b8f69ae78163167d";
        //    string uid = "";
        //    //string objtype = "records";

        //    var entity_test = await KaixinSnsDataAccessMethods.GetUserInfo(accessTokenKaixin, "");
        //    uid = entity_test.Uid.ToString();


        //    KaixinSnsDataService dataService = new KaixinSnsDataService();
        //    dataService.SetAccount(new Account
        //    {
        //        Sns = new SnsType
        //        {
        //            ID = 2,
        //            Name = _snsName
        //        },
        //        LocalAccountId = 2,
        //        AccessToken = accessTokenKaixin,
        //        UserId = uid
        //    });

        //    var entity_0 = await dataService.GetStatus(number, count, "");
        //    var entity_1 = await dataService.PostStatus(content, "", "", null);
        //    var entity_2 = await dataService.DeleteStatus(entity_1);
        //    var entity_3 = await dataService.RepostStatus(entity_0[0], content, "");
        //    var entity_4 = await dataService.CommentStatus(entity_3, content, "");
        //    var entity_5 = await dataService.LikeStatus(entity_3);
        //    var entity_6 = await dataService.GetRepostList(entity_0[0], number, count, "");
        //    var entity_7 = await dataService.GetCommentList(entity_0[0], number, count, "");
        //    var entity_8 = await dataService.GetLikeList(entity_3, number, count, "");
        //    var entity_9 = await dataService.ReplyComment(entity_4, content);
        //    var entity_10 = await dataService.DeleteComment(entity_9);
        //    var entity_11 = await dataService.GetUnreadCount();
        //    var entity_12 = await dataService.ClearUnreadCount(new Remind { Comment = -1 });
        //    var entity_13 = await dataService.GetCommentsToMe(number, count, "");
        //    var entity_14 = await dataService.GetCommentsFromMe(number, count, "");
        //    var entity_15 = await dataService.GetPublicStatus(number, count);
        //    var entity_16 = await dataService.GetHotTopics();
        //    var entity_17 = await dataService.GetUserProfile("", "");
        //    var entity_18 = await dataService.GetUserFriends("", "");
        //    var entity_19 = await dataService.GetUserHomeStatus("", "", number, count, "");
        //}

        #endregion
    }
}
