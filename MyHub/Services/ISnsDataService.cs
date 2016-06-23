using System.Collections.Generic;
using System.Threading.Tasks;
using MyHub.Models;

namespace MyHub.Services
{
    /// <summary>
    /// social network site data service interface
    /// </summary>
    public interface ISnsDataService
    {
        #region BasicSocialNetworkServices

        /// <summary>
        /// 获取当前登录用户及其好友的最新动态
        /// </summary>
        /// <param name="pageNumber">从哪一页开始</param>
        /// <param name="pageCount">每一页多少条新状态</param>
        /// <param name="sinceId"></param>
        /// <returns></returns>
        Task<IList<Status>> GetStatus(string pageNumber, string pageCount, string sinceId);

        /// <summary>
        /// 发布一条状态，只需要设置status.content属性即可，其他没有
        /// 这个接口一般没什么用
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<Status> PostStatus(Status status);

        /// <summary>
        /// 发布新状态
        /// </summary>
        /// <param name="content">内容，可以包含文字、表情、标签、地址等信息</param>
        /// <param name="longtitude">无效的参数</param>
        /// <param name="lantitude">无效的参数</param>
        /// <param name="picture">需要发布的图片</param>
        /// <returns>成功发布之后的新状态</returns>
        Task<Status> PostStatus(string content, string longtitude, string lantitude, Windows.Storage.StorageFile picture);

        /// <summary>
        /// 删除一个状态，只能删除自己的状态
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<bool?> DeleteStatus(Status status);

        /// <summary>
        /// 转发一条状态
        /// </summary>
        /// <param name="originalStatus"></param>
        /// <param name="content"></param>
        /// <param name="isComment">是否评论，0或1，只对微博有用</param>
        /// <returns></returns>
        Task<Status> RepostStatus(Status originalStatus, string content, string isComment);

        /// <summary>
        /// 评论一条状态
        /// </summary>
        /// <param name="originalStatus"></param>
        /// <param name="content"></param>
        /// <param name="isComment"></param>
        /// <returns></returns>
        Task<Comment> CommentStatus(Status originalStatus, string content, string isComment);

        /// <summary>
        /// 点赞【只有开心网支持点赞，微博不支持】
        /// </summary>
        /// <param name="originalStatus"></param>
        /// <returns>true点赞成功，false取消点赞成功，null操作失败</returns>
        Task<bool?> LikeStatus(Status originalStatus);

        /// <summary>
        /// 收藏功能【只有微博支持，开心网不支持】
        /// </summary>
        /// <param name="originalStatus"></param>
        /// <returns>true收藏成功，false取消收藏成功，null操作失败</returns>
        Task<bool?> FavoriteStatus(Status originalStatus);

        /// <summary>
        /// 获取一条评论的转发列表
        /// </summary>
        /// <param name="originalStatus"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageCount"></param>
        /// <param name="sinceId"></param>
        /// <returns></returns>
        Task<IList<Status>> GetRepostList(Status originalStatus, string pageNumber, string pageCount, string sinceId);

        /// <summary>
        /// 获取一条状态的评论列表
        /// </summary>
        /// <param name="originalStatus"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageCount"></param>
        /// <param name="sinceId"></param>
        /// <returns></returns>
        Task<IList<Comment>> GetCommentList(Status originalStatus, string pageNumber, string pageCount, string sinceId);

        /// <summary>
        /// 获取一条状态的点赞用户列表【只有开心网支持，微博不支持】
        /// </summary>
        /// <param name="originalStatus"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageCount"></param>
        /// <param name="sinceId"></param>
        /// <returns></returns>
        Task<IList<User>> GetLikeList(Status originalStatus, string pageNumber, string pageCount, string sinceId);

        /// <summary>
        /// 对一条评论进行回复
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Task<Comment> ReplyComment(Comment comment, string content);

        /// <summary>
        /// 删除一条评论，只能删除自己的评论内容
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        Task<bool?> DeleteComment(Comment comment);

        #endregion BasicSocialNetworkServices

        #region NotificationSerivces

        /// <summary>
        /// 获取用户各种通知的未读数量
        /// </summary>
        /// <returns></returns>
        Task<Remind> GetUnreadCount();

        /// <summary>
        /// 清空用户的未读通知数量【目前只有开心网支持改方法】
        /// </summary>
        /// <param name="type">将需要清空的通知类型对应的type对象的属性设置为-1</param>
        /// <returns></returns>
        Task<bool?> ClearUnreadCount(Remind type);

        /// <summary>
        /// 获取我收到的评论列表【开心网可能存在问题】
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageCount"></param>
        /// <param name="sinceId"></param>
        /// <returns></returns>
        Task<IList<Comment>> GetCommentsToMe(string pageNumber, string pageCount, string sinceId);

        /// <summary>
        /// 获取我发出的评论列表【开心网可能存在问题】
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageCount"></param>
        /// <param name="sinceId"></param>
        /// <returns></returns>
        Task<IList<Comment>> GetCommentsFromMe(string pageNumber, string pageCount, string sinceId);

        /// <summary>
        /// 获取提到我的评论【只有微博支持该方法，且只能获得到授权本应用的用户的评论】
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageCount"></param>
        /// <param name="sinceId"></param>
        /// <returns></returns>
        Task<IList<Comment>> GetMentionsComments(string pageNumber, string pageCount, string sinceId);

        /// <summary>
        /// 获取提到我的状态【只有微博支持该方法，且只能获得到授权本应用的用户的状态】
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageCount"></param>
        /// <param name="sinceId"></param>
        /// <returns></returns>
        Task<IList<Status>> GetMentionsStatuses(string pageNumber, string pageCount, string sinceId);

        #endregion NotificationSerivces

        #region ExploreAndSearchServices

        /// <summary>
        /// 获得公众状态，开心网和微博都支持该方法
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        Task<IList<Status>> GetPublicStatus(string pageNumber, string pageCount);

        /// <summary>
        /// 获取本地周边状态【只有微博支持】
        /// </summary>
        /// <param name="latitude">从GPS获取的纬度数据</param>
        /// <param name="longitude">从GPS获取的精度数据</param>
        /// <param name="range">可以选择传空字符串表示默认的范围</param>
        /// <param name="pageNumber"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        Task<IList<Status>> GetLocalStatus(string latitude, string longitude, string range, string pageNumber, string pageCount);

        /// <summary>
        /// 获取热门话题【只有开心网支持该方法，微博的无法解析数据】
        /// </summary>
        /// <returns></returns>
        Task<IList<string>> GetHotTopics();

        /// <summary>
        /// 搜索一个话题下对应的所有状态【不支持该功能】
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        Task<IList<Status>> SearchStatusesWithTopic(string topic);

        /// <summary>
        /// 搜索状态【不支持该功能】
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<IList<Status>> SearchStatuses(string key);

        /// <summary>
        /// 搜索用户【不支持该功能】
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<IList<User>> SearchUsers(string key);

        /// <summary>
        /// 搜索话题【不支持该功能】
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        Task<IList<string>> SearchTopics(string topic);

        /// <summary>
        /// 根据传入的经纬度获取附近的位置信息，通过调用微博API实现，开心网不需要实现
        /// </summary>
        /// <param name="latitude">纬度，有效范围：-90.0到+90.0，+表示北纬。</param>
        /// <param name="longitude">经度，有效范围：-180.0到+180.0，+表示东经。</param>
        /// <param name="range">查询范围半径，默认为2000，最大为10000，单位米。可选</param>
        /// <param name="keyword">查询的关键词，必须进行URLencode。可选</param>
        /// <param name="count">单页返回的记录条数，默认为20，最大为50。可选</param>
        /// <param name="page">返回结果的页码，默认为1。可选</param>
        /// <returns></returns>
        Task<IList<Location>> GetNearbyPositions(string latitude, string longitude, string range, string keyword, string count, string page);

        /// <summary>
        /// 根据关键词按地址位置获取POI点的信息，通过调用微博API实现，开心网不需要实现
        /// </summary>
        /// <param name="keyword">查询的关键词，必须进行URLencode</param>
        /// <param name="pageNumber">返回结果的页码，默认为1，最大为40。 </param>
        /// <param name="pageCount">单页返回的记录条数，默认为10，最大为20。 </param>
        /// <returns></returns>
        Task<IList<Location>> SearchLocation(string keyword, string pageNumber, string pageCount);

        /// <summary>
        /// 获取对应长链接的微博短链接，利用微博API实现
        /// </summary>
        /// <param name="originalUrl"></param>
        /// <returns></returns>
        Task<string> ShortenUrl(string originalUrl);

        /// <summary>
        /// 获取微博对应短链接的长连接，利用微博API实现
        /// </summary>
        /// <param name="urlShort"></param>
        /// <returns></returns>
        Task<string> GetShortUrlExpanded(string urlShort);

        #endregion ExploreAndSearchServices

        #region UserHomeServices

        /// <summary>
        /// 获取用户的详细信息，两个参数传入一个即可【微博只能获取授权本应用的详细信息】
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="screenName"></param>
        /// <returns></returns>
        Task<UserProfile> GetUserProfile(string userId, string screenName);

        /// <summary>
        /// 获取用户的好友列表【微博获取的是自己关注的人，但只能返回3条数据】
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="screenName"></param>
        /// <returns></returns>
        Task<IList<User>> GetUserFriends(string userId, string screenName);

        /// <summary>
        /// 获取一个用户发布的状态列表【微博只能获取授权本应用的用户状态】
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="screenName"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageCount"></param>
        /// <param name="sinceId"></param>
        /// <returns></returns>
        Task<IList<Status>> GetUserHomeStatus(string userId, string screenName, string pageNumber, string pageCount, string sinceId);

        /// <summary>
        /// 获取当前用户的收藏列表【只有微博支持】
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        Task<IList<Favorite>> GetUserFavorites(string pageNumber, string pageCount);

        /// <summary>
        /// 获取当前用户的点赞列表【不支持该功能】
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageCount"></param>
        /// <param name="sinceId"></param>
        /// <returns></returns>
        Task<IList<Status>> GetUserLikes(string pageNumber, string pageCount, string sinceId);

        #endregion UserHomeServices
    }
}