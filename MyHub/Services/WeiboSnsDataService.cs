using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyHub.Models;
using MyHub.Models.Weibo;
using System.Globalization;

namespace MyHub.Services
{
    public class WeiboSnsDataService : ISnsDataService
    {
        private CultureInfo _cultureInfo;
        private string _format;
        private string _snsName;

        public WeiboSnsDataService()
        {
            InitEntities();
            DoDynamicInit();

            _snsName = "新浪微博";
            _cultureInfo = new CultureInfo("en-US");//开始定义新浪微博时间的解析规则
            _format = "ddd MMM d HH:mm:ss zz00 yyyy";

            Lifecycle.AppRuntimeEnvironment.Instance.PropertyChanged += UserAccount_PropertyChanged;
        }

        public Account Account { get; private set; }
        public List<Status> StatusList { get; private set; }
        public List<Comment> CommentList { get; private set; }
        public List<User> UserList { get; private set; }
        public List<Favorite> FavoriteList { get; private set; }
        public List<Location> LocationList { get; private set; }

        /// <summary>
        /// 初始化实体变量
        /// </summary>
        public void InitEntities()
        {
            Account = new Account();
            StatusList = new List<Status>();
            CommentList = new List<Comment>();
            UserList = new List<User>();
            FavoriteList = new List<Favorite>();
            LocationList = new List<Location>();
        }

        private void DoDynamicInit()
        {
            // 初始化当前微博账户的信息，从全局数据中获取
            Account = Lifecycle.AppRuntimeEnvironment.Instance.GetUserAccount(_snsName);
            if (Account == null)// 如果不能从已经登陆的账户中获取本社交网络的账户，就从没有登录的账户中获取
                Account = Lifecycle.AppRuntimeEnvironment.Instance.GetUserAccountUnlogin(_snsName);
        }

        private void UserAccount_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UserAccount")
            {
                DoDynamicInit();
            }
        }

        #region BasicSocialNetworkServices

        public async Task<IList<Status>> GetStatus(string pageNumber, string pageCount, string sinceId)
        {
            //DoDynamicInit();
            StatusList.Clear();
            Status temp;

            var entity = await WeiboSnsDataAccessMethods.GetTimelines("", pageCount, "", "", pageNumber, sinceId);
            if (entity == null || entity.Statuses == null)
                return null;

            foreach(WeiboStatusEntity s in entity.Statuses)
                if((temp = ConvertToStatus(s)) != null)
                    StatusList.Add(temp);

            return StatusList;
        }

        public async Task<Status> PostStatus(Status status)
        {
            //DoDynamicInit();
            return await PostStatus(status.Content, status.LocationInfo.Longitude, status.LocationInfo.Latitude, null);
        }

        /// <summary>
        /// 【存在问题：无法发送图片】
        /// </summary>
        /// <param name="content"></param>
        /// <param name="longtitude"></param>
        /// <param name="lantitude"></param>
        /// <param name="picture"></param>
        /// <returns></returns>
        public async Task<Status> PostStatus(string content, string longtitude, string lantitude, Windows.Storage.StorageFile picture)
        {
            //DoDynamicInit();
            WeiboStatusEntity entity;
            if(picture == null)// 发布普通微博
            {
                entity = await WeiboSnsDataAccessMethods.PostMessage(content, lantitude, longtitude);
            }
            else// 发布图片微博
            {
                entity = await WeiboSnsDataAccessMethods.PostMessageWithPicture(content, picture.Path, longtitude, lantitude);
                //entity = null;
            }

            return ConvertToStatus(entity);
        }

        public async Task<bool?> DeleteStatus(Status status)
        {
            //DoDynamicInit();
            var entity = await WeiboSnsDataAccessMethods.DestroyStatuses(status.StatusId);
            return entity != null;
        }

        public async Task<Status> RepostStatus(Status originalStatus, string content, string isComment)
        {
            //DoDynamicInit();
            var entity = await WeiboSnsDataAccessMethods.ReportStatuses(originalStatus.StatusId, content, isComment, "");

            return ConvertToStatus(entity);
        }

        public async Task<Comment> CommentStatus(Status originalStatus, string content, string isComment)
        {
            //DoDynamicInit();
            var entity = await WeiboSnsDataAccessMethods.CreateComments(originalStatus.StatusId, content, isComment, "");

            return ConvertToComment(entity);
        }

        /// <summary>
        /// 【微博暂未支持】
        /// </summary>
        public async Task<bool?> LikeStatus(Status originalStatus)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalStatus"></param>
        /// <returns>true收藏成功，false取消收藏成功，null操作失败</returns>
        public async Task<bool?> FavoriteStatus(Status originalStatus)
        {
            //DoDynamicInit();
            WeiboFavoriteStatusEntity entity;
            if(!originalStatus.Favorited)// 如果之前没有收藏过
            {
                entity = await WeiboSnsDataAccessMethods.CreateFavorite(originalStatus.StatusId);
                if(entity != null)
                    originalStatus.Favorited = true;
            }
            else
            {
                entity = await WeiboSnsDataAccessMethods.DestroyFavorite(originalStatus.StatusId);
                if (entity != null)
                    originalStatus.Favorited = false;
            }

            if (entity == null)
                return null;
            else
                return originalStatus.Favorited;
        }

        public async Task<IList<Status>> GetRepostList(Status originalStatus, string pageNumber, string pageCount, string sinceId)
        {
            //DoDynamicInit();
            StatusList.Clear();

            var entity = await WeiboSnsDataAccessMethods.GetReportTimeline(originalStatus.StatusId, sinceId, "", pageCount, pageNumber, "");
            if (entity == null || entity.Reposts == null)
                return null;

            foreach(WeiboStatusEntity s in entity.Reposts)
                    StatusList.Add(ConvertToStatus(s));

            return StatusList;
        }

        public async Task<IList<Comment>> GetCommentList(Status originalStatus, string pageNumber, string pageCount, string sinceId)
        {
            //DoDynamicInit();
            CommentList.Clear();

            var entity = await WeiboSnsDataAccessMethods.GetCommentsList(originalStatus.StatusId, sinceId, "", pageCount, pageNumber
                , "");
            if (entity == null || entity.comments == null)
                return null;

            foreach(WeiboCommentEntity c in entity.comments)
                CommentList.Add(ConvertToComment(c));

            return CommentList;
        }

        /// <summary>
        /// 【微博暂未支持】
        /// </summary>
        public async Task<IList<User>> GetLikeList(Status originalStatus, string pageNumber, string pageCount, string sinceId)
        {
            return null;
        }

        public async Task<Comment> ReplyComment(Comment comment, string content)
        {
            //DoDynamicInit();
            var entity = await WeiboSnsDataAccessMethods.ReplyComments(comment.CommentId, content, comment.StatusInfo.StatusId, "", "", "");
            return ConvertToComment(entity);
        }

        public async Task<bool?> DeleteComment(Comment comment)
        {
            //DoDynamicInit();
            var entity = await WeiboSnsDataAccessMethods.DestroyComments(comment.CommentId);
            return entity != null;
        }

        #endregion BasicSocialNetworkServices

        #region NotificationSerivces

        public async Task<Remind> GetUnreadCount()
        {
            //DoDynamicInit();
            var entity = await WeiboSnsDataAccessMethods.GetRemindUnreadCount(Account.UserId);
            return ConvertToRemind(entity);
        }

        /// <summary>
        /// 【微博暂未支持】
        /// </summary>
        public async Task<bool?> ClearUnreadCount(Remind type)
        {
            return null;
        }

        public async Task<IList<Comment>> GetCommentsToMe(string pageNumber, string pageCount, string sinceId)
        {
            //DoDynamicInit();
            CommentList.Clear();

            var entity = await WeiboSnsDataAccessMethods.GetCommentsToMe(sinceId, "", pageCount, pageNumber, "", "");
            if (entity == null || entity.comments == null)
                return null;
            foreach (WeiboCommentEntity c in entity.comments)
            {
                CommentList.Add(ConvertToComment(c));
            }

            return CommentList;
        }

        public async Task<IList<Comment>> GetCommentsFromMe(string pageNumber, string pageCount, string sinceId)
        {
            //DoDynamicInit();
            CommentList.Clear();

            var entity = await WeiboSnsDataAccessMethods.GetCommentsByMe(sinceId, "", pageCount, pageNumber, "");
            if (entity == null || entity.comments == null)
                return null;
            foreach (WeiboCommentEntity c in entity.comments)
            {
                CommentList.Add(ConvertToComment(c));
            }

            return CommentList;
        }

        public async Task<IList<Comment>> GetMentionsComments(string pageNumber, string pageCount, string sinceId)
        {
            //DoDynamicInit();
            CommentList.Clear();

            var entity = await WeiboSnsDataAccessMethods.GetMentionsComments(sinceId, "", pageCount, pageNumber, "", "");
            if (entity == null || entity.comments == null)
                return null;
            foreach (WeiboCommentEntity c in entity.comments)
            {
                CommentList.Add(ConvertToComment(c));
            }

            return CommentList;
        }

        public async Task<IList<Status>> GetMentionsStatuses(string pageNumber, string pageCount, string sinceId)
        {
            //DoDynamicInit();
            StatusList.Clear();

            var entity = await WeiboSnsDataAccessMethods.GetMentionsStatus(sinceId, "", pageCount, pageNumber, "", "");
            if (entity == null || entity.Statuses == null)
                return null;
            foreach (WeiboStatusEntity s in entity.Statuses)
            {
                StatusList.Add(ConvertToStatus(s));
            }

            return StatusList;
        }

        #endregion NotificationSerivces

        #region ExploreAndSearchServices

        public async Task<IList<Status>> GetPublicStatus(string pageNumber, string pageCount)
        {
            //DoDynamicInit();
            StatusList.Clear();

            var entity = await WeiboSnsDataAccessMethods.GetPulicTimeline("", pageCount, pageNumber);
            if (entity == null || entity.Statuses == null)
                return null;
            foreach (WeiboStatusEntity s in entity.Statuses)
            {
                StatusList.Add(ConvertToStatus(s));
            }

            return StatusList;
        }

        public async Task<IList<Status>> GetLocalStatus(string latitude, string longitude, string range, string pageNumber, string pageCount)
        {
            //DoDynamicInit();
            StatusList.Clear();

            var entity = await WeiboSnsDataAccessMethods.GetLocalPublicTimeline(latitude, longitude, range, pageCount, pageNumber);
            if (entity == null || entity.Statuses == null)
                return null;
            foreach (WeiboStatusEntity s in entity.Statuses)
            {
                StatusList.Add(ConvertToStatus(s));
            }

            return StatusList;
        }

        /// <summary>
        /// 【微博上暂未实现】
        /// </summary>
        /// <returns></returns>
        public async Task<IList<string>> GetHotTopics()
        {
            return null;
        }

        /// <summary>
        /// 【高权限接口，暂未实现】
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        public async Task<IList<Status>> SearchStatusesWithTopic(string topic)
        {
            return null;
        }

        /// <summary>
        /// 【微博暂未支持】
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<IList<Status>> SearchStatuses(string key)
        {
            return null;
        }

        /// <summary>
        /// 【微博暂未支持】
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<IList<User>> SearchUsers(string key)
        {
            return null;
        }

        /// <summary>
        /// 【微博暂未支持】
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        public async Task<IList<string>> SearchTopics(string topic)
        {
            return null;
        }

        public async Task<IList<Location>> GetNearbyPositions(string latitude, string longitude, string range, string keyword, string count, string page)
        {
            //DoDynamicInit();
            LocationList.Clear();

            var entity = await WeiboSnsDataAccessMethods.GetNearbyPois(latitude, longitude, range, keyword, count, page);
            if (entity == null || entity.pois == null || entity.pois.Count <= 0)
                return null;

            foreach(WeiboPositionItemEntity p in entity.pois)
            {
                LocationList.Add(ConvertToLocation(p));
            }

            return LocationList;
        }

        public async Task<IList<Location>> SearchLocation(string keyword, string pageNumber, string pageCount)
        {
            //DoDynamicInit();
            LocationList.Clear();

            var entity = await WeiboSnsDataAccessMethods.SearchPoisByLocation(keyword, pageNumber, pageCount);
            if (entity == null || entity.pois == null || entity.pois.Count <= 0)
                return null;

            foreach (WeiboPositionItemEntity p in entity.pois)
            {
                LocationList.Add(ConvertToLocation(p));
            }

            return LocationList;
        }

        public async Task<string> ShortenUrl(string originalUrl)
        {
            //DoDynamicInit();
            if (string.IsNullOrWhiteSpace(originalUrl))
                return null;

            var entity = await WeiboSnsDataAccessMethods.ShortenUrl(originalUrl);
            if (entity == null || entity.urls == null || entity.urls.Count <= 0 || !entity.urls[0].result)
                return null;
            return entity.urls[0].url_short;
        }

        public async Task<string> GetShortUrlExpanded(string urlShort)
        {
            //DoDynamicInit();
            if (string.IsNullOrWhiteSpace(urlShort))
                return null;

            var entity = await WeiboSnsDataAccessMethods.GetShortUrlExpanded(urlShort);
            if (entity == null || entity.urls == null || entity.urls.Count <= 0 || !entity.urls[0].result)
                return null;
            return entity.urls[0].url_long;
        }

        #endregion ExploreAndSearchServices

        #region UserHomeServices

        public async Task<UserProfile> GetUserProfile(string userId, string screenName)
        {
            //DoDynamicInit();
            var entity = await WeiboSnsDataAccessMethods.GetUserInfomation(userId, screenName);
            return ConvertToUserProfile(entity);
        }

        public async Task<IList<User>> GetUserFriends(string userId, string screenName)
        {
            //DoDynamicInit();
            UserList.Clear();

            var entity = await WeiboSnsDataAccessMethods.GetFriendsList(userId, "", "", "");
            if (entity == null || entity.users == null)
                return null;
            foreach (WeiboUserEntity u in entity.users)
            {
                UserList.Add(ConvertToUser(u));
            }

            return UserList;
        }

        public async Task<IList<Status>> GetUserHomeStatus(string userId, string screenName, string pageNumber, string pageCount, string sinceId)
        {
            //DoDynamicInit();
            StatusList.Clear();

            var entity = await WeiboSnsDataAccessMethods.GetUserTimeline("", pageCount, "", "", pageNumber, screenName, sinceId, "");
            if (entity == null || entity.Statuses == null)
                return null;
            foreach (WeiboStatusEntity s in entity.Statuses)
            {
                StatusList.Add(ConvertToStatus(s));
            }

            return StatusList;
        }

        public async Task<IList<Favorite>> GetUserFavorites(string pageNumber, string pageCount)
        {
            //DoDynamicInit();
            FavoriteList.Clear();

            var entity = await WeiboSnsDataAccessMethods.GetUserFavoriteList(pageNumber, pageCount);
            if (entity == null || entity.favorites == null)
                return null;
            foreach (WeiboFavoriteStatusEntity f in entity.favorites)
            {
                FavoriteList.Add(ConvertToFavorite(f));
            }

            return FavoriteList;
        }

        /// <summary>
        /// 【微博暂未支持】
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageCount"></param>
        /// <param name="sinceId"></param>
        /// <returns></returns>
        public async Task<IList<Status>> GetUserLikes(string pageNumber, string pageCount, string sinceId)
        {
            return null;
        }

        #endregion UserHomeServices


        #region ToolFunctions

        private User ConvertToUser(WeiboUserEntity u)
        {
            if (u == null)
                return null;

            User tempUser = new User
            {
                Sns = Account == null ? new SnsType() { Name = _snsName } : Account.Sns,
                UserId = u.idstr,
                NickName = u.screen_name,
                RemarkName = u.remark,
                LogoUrl = u.profile_image_url,
                Following = u.following,
                Verified = u.verified
            };

            return tempUser;
        }

        private Status ConvertToStatus(WeiboStatusEntity s)
        {
            if (s == null)
                return null;

            Status tempStatus = new Status
            {
                Sns = Account == null ? new SnsType() { Name = _snsName } : Account.Sns,
                StatusId = s.idstr,
                Author = ConvertToUser(s.user),
                Content = s.text,
                CreateTime = ConvertToDatetime(s.created_at),
                Source = ConvertToSource(s.source),
                ThumbnailPicUrl = s.thumbnail_pic,
                OriginalPicUrl = s.original_pic,
                Favorited = s.favorited,
                RepostsCount = s.reposts_count,
                CommentsCount = s.comments_count,
                AttitudesCount = s.attitudes_count,
            };

            // init PictureUrls property
            if(s.pic_urls != null && s.pic_urls.Count != 0)
            {
                tempStatus.PictureUrls = new List<string>();
                for (int i = 0; i < s.pic_urls.Count; ++i)
                    tempStatus.PictureUrls.Add(s.pic_urls[i].thumbnail_pic);
            }

            // convert the retweeted_status entity
            if (s.retweeted_status != null)
                tempStatus.RetweetedStatus = ConvertToStatus(s.retweeted_status);

            return tempStatus;
        }

        private Comment ConvertToComment(WeiboCommentEntity c)
        {
            if (c == null)
                return null;

            Comment tempComment = new Comment
            {
                Sns = new SnsType() { Name = _snsName },
                CommentId = c.idstr,
                CreateTime = ConvertToDatetime(c.created_at),
                Content = c.text,
                Source = ConvertToSource(c.source),
                Author = ConvertToUser(c.user),
                StatusInfo = ConvertToStatus(c.status),
                ReplyComment = ConvertToComment(c.reply_comment)
            };

            return tempComment;
        }

        private Remind ConvertToRemind(WeiboRemindEntity r)
        {
            if (r == null)
                return null;

            Remind tempRemind = new Remind
            {
                Sns = Account == null ? new SnsType() { Name = _snsName } : Account.Sns,
                Status = r.status,
                Comment = r.cmt,
                MentionStatus = r.mention_status,
                MentionComment = r.mention_cmt,
                Notice = r.notice
            };

            return tempRemind;
        }

        private UserProfile ConvertToUserProfile(WeiboUserEntity u)
        {
            if (u == null)
                return null;

            UserProfile tempUserProfile = new UserProfile
            {
                BasicUserInfo = ConvertToUser(u),
                LogoLargeUrl = u.avatar_large,
                Gender = u.gender == "m" ? "男" : "女",
                Signature = u.description,
                Location = u.location,
                FollowerCount = u.followers_count,
                FriendsCount = u.friends_count,
                StatusesCount = u.statuses_count
            };

            return tempUserProfile;
        }

        private Favorite ConvertToFavorite(WeiboFavoriteStatusEntity f)
        {
            if (f == null)
                return null;

            Favorite tempFavorite = new Favorite
            {
                StatusInfo = ConvertToStatus(f.status),
                Tags = ConvertToFavoriteTags(f.tags),
                FavoriteTime = ConvertToDatetime(f.favorited_time)
            };

            return tempFavorite;
        }

        private List<FavoriteTag> ConvertToFavoriteTags(List<WeiboFavoriteStatusTagEntity> tags)
        {
            if (tags == null)
                return null;

            var tempFavoriteTags = new List<FavoriteTag>();
            foreach(WeiboFavoriteStatusTagEntity t in tags)
            {
                tempFavoriteTags.Add(new FavoriteTag
                {
                    TagId = t.id,
                    TagName = t.tag
                });
            }
            return tempFavoriteTags;
        }

        private DateTime ConvertToDatetime(string timeStr)
        {
            return DateTime.ParseExact(timeStr, _format, _cultureInfo);
        }

        /// <summary>
        /// 将新浪微博中微博来自的客户端解析成简单的文本
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string ConvertToSource(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return null;

            string result = s;

            var temp = s.Split(">".ToCharArray());
            if(temp != null && temp.Length > 1)
            {
                temp = temp[1].Split("<".ToCharArray());
                if (temp != null && temp.Length > 0)
                    result = temp[0];
            }

            return result;
        }

        /// <summary>
        /// 将微博的位置信息实体转化成自己的Location实体
        /// </summary>
        private Location ConvertToLocation(WeiboPositionItemEntity p)
        {
            if (p == null)
                return null;

            Location temp = new Location
            {
                Latitude = p.lat,
                Longitude = p.lon,
                Poiid = p.poiid,
                Title = p.title,
                Province = p.province,
                City = p.city,
                Address = p.address
            };

            return temp;
        }

        #endregion

        #region TestCodeForWeiboDataService

        //private async void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    string pageNumber = "1";
        //    string pageCount = "10";
        //    string screenName = "秋泽Aollo";
        //    string userId = "2325214344";
        //    string content = "MyHub";
        //    string latitude = "37.5193220000";
        //    string longitude = "122.1282450000";

        //    WeiboSnsDataService dataService = new WeiboSnsDataService();
        //    dataService.SetAccount(new Models.Account
        //    {
        //        Sns = new Models.SnsType
        //        {
        //            ID = 1,
        //            Name = _snsName
        //        },
        //        LocalAccountId = 1,
        //        AccessToken = "2.00KT33XCgHO3fBcea2ecb378yTxPUB",
        //        UserId = "2325214344"
        //    });

        //    var entity_0 = await dataService.GetStatus(pageNumber, pageCount, "");
        //    var entity_1 = await dataService.PostStatus(content, "", "", null);
        //    var entity_2 = await dataService.DeleteStatus(entity_1);
        //    var entity_3 = await dataService.RepostStatus(entity_0[0], content, "");
        //    var entity_4 = await dataService.CommentStatus(entity_0[0], content, "");
        //    var entity_5 = await dataService.FavoriteStatus(entity_3);
        //    var entity_6 = await dataService.FavoriteStatus(entity_3);
        //    var entity_7 = await dataService.GetRepostList(entity_0[0], pageNumber, pageCount, "");
        //    var entity_8 = await dataService.GetCommentList(entity_3, pageNumber, pageCount, "");
        //    var entity_9 = await dataService.CommentOnComment(entity_4, content);
        //    var entity_10 = await dataService.DeleteComment(entity_4);
        //    var entity_11 = await dataService.GetUnreadCount();
        //    var entity_12 = await dataService.GetMentionsStatuses(pageNumber, pageCount, "");
        //    var entity_13 = await dataService.GetPublicStatus(pageNumber, pageCount);
        //    var entity_14 = await dataService.GetLocalStatus(latitude, longitude, "", pageNumber, pageCount);
        //    var entity_15 = await dataService.GetUserProfile(screenName);
        //    var entity_16 = await dataService.GetUserFriends(screenName);
        //    var entity_17 = await dataService.GetUserHomeStatus(screenName, pageNumber, pageCount, "");
        //    var entity_18 = await dataService.GetUserFavorites(pageNumber, pageCount);

        //}

        #endregion TestCodeForWeiboDataService

    }
}
