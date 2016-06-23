using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.Storage;
using KaixinSDK;
using KaixinSDK.Commons;
using KaixinSDK.Entities;
using MyHub.Models.Kaixin;
using MyHub.Lifecycle;


namespace MyHub.Services
{
    /// <summary>
    /// 获取开心网信息的相关方法
    /// </summary>
    public static class KaixinSnsDataAccessMethods
    {
        /// <summary>
        /// 获取当前登录用户的所有好友的最新消息 
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="start">展示起始条数，默认为0。不是必填</param>
        /// <param name="num">展示条数，默认为20。不是必填</param>
        /// <param name="category">分类条件：0：全部，1：原创，2：转发，3：签名。不是必填的</param>
        /// <returns></returns>
        public static async Task<FriendsRecordsListEntity> GetFriendsRecords(string access_token, int? start, int? num, int? category)
        {
            var entity = await API.Instance.RequestFriendsRecords(access_token, start, num, category);
            return entity;
        }

        /// <summary>
        /// 单一用户的记录列表 GET
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="uid">用户id，默认为调用接口用户</param>
        /// <param name="start">展示起始条数，默认为0</param>
        /// <param name="num">展示条数，默认为20</param>
        /// <param name="category">分类条件：0：全部，1：原创，2：转发，3：签名。不是必填的</param>
        /// <returns></returns>
        public static async Task<FriendsRecordsListEntity> GetUserHomeRecords(string access_token, int? uid, int? start, int? num, int? category)
        {
            var parameters = new Dictionary<string, string>();

            parameters.Add("access_token", access_token);
            if(uid != null)
            {
                parameters.Add("uid", uid.ToString());
            }
            if (start != null)
            {
                parameters.Add("start", start.ToString());
            }
            if (num != null)
            {
                parameters.Add("num", num.ToString());
            }
            if (category != null)
            {
                if (category == 0 || category == 1 || category == 2 || category == 3)//判断category的值是否是合法的数值
                {
                    parameters.Add("category", category.ToString());
                }
            }

            return await API.Instance.DownloadString<FriendsRecordsListEntity>(API.Instance.GenerateRequestUri(KaixinConstant.METHODS.RecordsUser, parameters));
        }

        /// <summary>
        /// 随便看看记录列表
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="start">展示起始条数，默认为0</param>
        /// <param name="num">展示条数，默认为20 </param>
        /// <returns></returns>
        public static async Task<FriendsRecordsListEntity> GetPublicRecords(string access_token, int? start, int? num)
        {
            var parameters = new Dictionary<string, string>();

            parameters.Add("access_token", access_token);
            if (start != null)
            {
                parameters.Add("start", start.ToString());
            }
            if (num != null)
            {
                parameters.Add("num", num.ToString());
            }

            return await API.Instance.DownloadString<FriendsRecordsListEntity>(API.Instance.GenerateRequestUri(KaixinConstant.METHODS.RecordsPublic, parameters));
        }

        /// <summary>
        /// 发表记录。
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="content">发记录的内容（最多140个汉字或280个英文字母字符），不支持 {_USER_} 等。</param>
        /// <param name="location">记录的地理位置（目前仅在“我的记录”列表中显示）。</param>
        /// <param name="latitude">纬度 -90.0 到 +90.0，+表示北纬（目前暂不能显示）。</param>
        /// <param name="longitude">经度 -180.0 到 +180.0，+表示东经（目前暂不能显示）。</param>
        /// <param name="sync_status">是否同步签名，0/1/2 - 无任何操作/同步/不同步，默认为0无任何操作。</param>
        /// <param name="privacy">权限设置，0/1/2/3 - 任何人可见/好友可见/仅自己可见/好友及好友的好友可见，默认为0任何人可见。</param>
        /// <param name="file">发记录上传的图片，图片在 10M 以内。</param>
        /// <returns></returns>
        public static async Task<PostRecordEntity> PostRecord(string access_token, string content, string location, double? latitude, double? longitude, int? sync_status, int? privacy, StorageFile file)
        {
            var entity = await API.Instance.PostRecord(access_token, content, location, latitude, longitude, sync_status, privacy, file);
            return entity;
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="rid">记录ID</param>
        /// <returns></returns>
        public static async Task<KaixinDeleteRecordEntity> DeleteRecord(string access_token, string rid)
        {
            var parameters = new List<NamedValuePair>();

            parameters.Add(new NamedValuePair("access_token", access_token));
            parameters.Add(new NamedValuePair("rid", rid));

            return await API.Instance.Submit<KaixinDeleteRecordEntity>(KaixinConstant.METHODS.DeleteRecord, parameters);
        }

        /// <summary>
        /// 对某一资源进行转发，HTTP请求方式: POST 
        /// </summary>
        /// <param name="access_token">授权的access token</param>
        /// <param name="objtype">被转发资源的类型，目前支持的资源有：photo, album, records, diary</param>
        /// <param name="objid">被转发资源的ID</param>
        /// <param name="ouid">被转发资源所有者的UID</param>
        /// <param name="content">转发附言内容</param>
        /// <returns></returns>
        /// 可用的测试内容: objtype = "records", objid = "1461805161468308", ouid = "65589562"
        /// content = "This is test content for method CreateForwardForSource. -- Form MyHub"
        public static async Task<CreateForward> CreateForwardForSource(string access_token, string objtype, string objid, string ouid, string content)
        {
            var entity = await API.Instance.CreateForwardForSource(access_token, objtype, objid, ouid, content);
            return entity;
        }

        /// <summary>
        /// 添加对某一资源的评论
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="objid">被评论资源的ID</param>
        /// <param name="ouid">被评论资源所有者的UID</param>
        /// <param name="content">评论内容</param>
        /// <param name="objtype">被评论资源的类型，目前支持的资源有：photo, album, records, diary, repaste。如不提供objtype则评论的类型为当前应用的appid，需要通知开放平台管理员开通应用的评论权限</param>
        /// <param name="secret">是否悄悄话。0：不是悄悄话，默认为0；1：是悄悄话</param>
        /// <returns></returns>
        /// 可用的测试内容：objid = "1461652675778745", ouid = "78276544", content = "This is a test comment for method CreateCommentForSource."
        /// objtype = "records", secret = 0
        public static async Task<CreateCommentEntity> CreateCommentForSource(string access_token, string objid, string ouid, string content, string objtype, int? secret)
        {
            var entity = await API.Instance.CreateCommentForSource(access_token, objid, ouid, content, objtype, secret);
            return entity;
        }

        /// <summary>
        /// 表达对某一资源的赞. HTTP请求方式: GET. 注意：记录的转发仍使用records作为objtype，而不是forward。居然能够多次对同一个资源点赞！！
        /// </summary>
        /// <param name="access_token">授权的access token</param>
        /// <param name="objtype">被赞资源的类型，目前支持的资源有：photo, album, records, diary, repaste </param>
        /// <param name="objid">被赞资源的ID</param>
        /// <param name="ouid">被赞资源所有者的UID</param>
        /// <returns></returns>
        /// 可用的测试内容: objtype = "records", objid = "1461652675778745", ouid = "78276544"
        public static async Task<CreateLikeEntity> CreateLikeForSource(string access_token, string objtype, string objid, string ouid)
        {
            var entity = await API.Instance.CreateLikeForSource(access_token, objtype, objid, ouid);
            return entity;
        }

        /// <summary>
        /// 取消对某一资源的赞。HTTP请求方式：GET 。注意：记录的转发仍使用records作为objtype，而不是forward 
        /// </summary>
        /// <param name="access_token">授权的access token</param>
        /// <param name="objtype">被赞资源的类型，目前支持的资源有：photo, album, records, diary, repaste </param>
        /// <param name="objid">被赞资源的ID</param>
        /// <param name="ouid">被赞资源所有者的UID</param>
        /// <returns></returns>
        /// 可用的测试内容： objtype = "records"， objid = "1461652675778745"， ouid = "78276544"
        public static async Task<CancelLikeEntity> CancelLikeForSource(string access_token, string objtype, string objid, string ouid)
        {
            var entity = await API.Instance.CancelLikeForSource(access_token, objtype, objid, ouid);
            return entity;
        }


        /// <summary>
        /// 判断用户是否赞过某一资源
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="objtype">被赞资源的类型，目前支持的资源有：photo(pid), album(albumid), records(rid), diary(did), repaste(urpid), where(wid)。注意：记录的转发仍使用records作为objtype，而不是forward</param>
        /// <param name="objid"></param>
        /// <param name="ouid"></param>
        /// <param name="uids">待判断的用户ID，多个uid用英文半角逗号分隔</param>
        /// <returns></returns>
        public static async Task<KaixinLikeCheckListEntity> CheckLikeForSource(string access_token, string objtype, string objid, string ouid, string uids)
        {
            var parameters = new Dictionary<string, string>();

            parameters.Add("access_token", access_token);
            parameters.Add("objtype", objtype);
            parameters.Add("objid", objid);
            parameters.Add("ouid", ouid);
            parameters.Add("uids", uids);

            return await API.Instance.DownloadString<KaixinLikeCheckListEntity>(API.Instance.GenerateRequestUri(KaixinConstant.METHODS.LikeCheck, parameters));
        }

        /// <summary>
        /// 获取某一资源的所有转发. HTTP请求方式: GET
        /// </summary>
        /// <param name="access_token">授权的access token</param>
        /// <param name="objtype">被转发资源的类型，目前支持的资源有：photo, album, records, diary</param>
        /// <param name="objid">被转发资源的ID</param>
        /// <param name="ouid">被转发资源所有者的UID</param>
        /// <param name="start">起始位置，默认0。不是必须参数</param>
        /// <param name="num">返回数量，默认10。不是必须参数</param>
        /// <returns></returns>
        /// 可用的测试内容： objtype = "records", objid = "1461805161468308", ouid = "65589562", start = 0, num = 20
        public static async Task<ForwardListEntity> GetForwardList(string access_token, string objtype, string objid, string ouid, int? start, int? num)
        {
            var entity = await API.Instance.RequestForwardList(access_token, objtype, objid, ouid, start, num);
            return entity;
        }

        /// <summary>
        /// 获取某条记录的评论列表
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="objid">被评论资源的ID</param>
        /// <param name="ouid">被评论资源所有者的UID</param>
        /// <param name="objtype">被评论资源的类型，目前支持的资源有：photo, album, records, diary, repaste.如不提供objtype则评论的类型为当前应用的appid，需要通知开放平台管理员开通应用的评论权限</param>
        /// <param name="start">起始位置，默认0</param>
        /// <param name="num">返回数量，默认10</param>
        /// <returns></returns>
        public static async Task<CommentListEntity> GetCommentList(string access_token, string objid, string ouid, string objtype, int? start, int? num)
        {
            var entity = await API.Instance.RequestCommentList(access_token, objid, ouid, objtype, start, num);
            return entity;
        }

        /// <summary>
        /// 获取对某一资源赞过的用户列表。HTTP请求方式：GET。注意：记录的转发仍使用records作为objtype，而不是forward 
        /// </summary>
        /// <param name="access_token">授权的access token</param>
        /// <param name="objtype">被赞资源的类型，目前支持的资源有：photo, album, records, diary, repaste </param>
        /// <param name="objid">被赞资源的ID</param>
        /// <param name="ouid">被赞资源所有者的UID</param>
        /// <param name="start">起始位置，默认是0。非必须参数</param>
        /// <param name="num">返回数量，默认是10。非必须参数</param>
        /// <returns></returns>
        /// 可用的测试内容： objtype = "records"， objid = "1461652675778745"， ouid = "78276544"， start = 0， num = 0
        public static async Task<ShowLikeUserListEntity> GetLikeUserList(string access_token, string objtype, string objid, string ouid, int? start, int? num)
        {
            var entity = await API.Instance.ShowLikeUserList(access_token, objtype, objid, ouid, start, num);
            return entity;
        }

        /// <summary>
        /// 对某一资源的某条评论进行回复
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="objid">被评论资源的ID</param>
        /// <param name="ouid">评论所有者的UID</param>
        /// <param name="thread_cid">评论的ID</param>
        /// <param name="owner">被评论资源所有者的UID</param>
        /// <param name="content">回复内容</param>
        /// <param name="objtype">被评论资源的类型，目前支持的资源有：photo, album, records, diary, repaste。如不提供objtype则评论的类型为当前应用的appid，需要通知开放平台管理员开通应用的评论权限</param>
        /// <returns></returns>
        /// 可用的测试内容: objid = "1461652675778745", ouid = "159068599", thread_cid = "1278256389", owner = "78276544"
        /// content = "This is a test reply for method ReplyCommentForSource. -- From MyHub", objtype = "records"
        public static async Task<ReplyCommentEntity> ReplyCommentForSource(string access_token, string objid, string ouid, string thread_cid, string owner, string content, string objtype)
        {
            var entity = await API.Instance.ReplyCommentForSource(access_token, objid, ouid, thread_cid, owner, content, objtype);
            return entity;
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="uid">评论资源所有者的uid</param>
        /// <param name="cid">评论id（cid）或评论主线id（thread_cid）</param>
        /// <returns></returns>
        /// 可用的测试内容：uid = "78276544", cid = "1278256347"
        public static async Task<DeleteCommentEntity> DeleteCommentForSource(string access_token, string uid, string cid)
        {
            var entity = await API.Instance.DeleteCommentForSource(access_token, uid, cid);
            return entity;
        }

        // ---------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 获取新消息 GET
        /// </summary>
        /// <returns></returns>
        public static async Task<KaixinMessageSummaryEntity> GetUnreadMessageCount(string access_token)
        {
            var parameters = new Dictionary<string, string>();

            parameters.Add("access_token", access_token);

            return await API.Instance.DownloadString<KaixinMessageSummaryEntity>(API.Instance.GenerateRequestUri(KaixinConstant.METHODS.MessageSummary, parameters));
        }

        /// <summary>
        /// 新消息清零
        /// 用户查看新消息后，可调用此接口，将消息中心的新动态数量置为零 
        /// </summary>
        /// <param name="msg_type">msg_type：消息类型（必填）,注：多个字段用半角逗号隔开. message, sysmsg_notice, sysmsg_friends, sysmsg_birthday, sysmsg_mention, sysmsg_forward, bbs_msg, bbs_reply, rgroup_msg, rgroup_reply, comment, reply</param>
        /// <returns></returns>
        /// 注意：由于解析返回结果的实体相同，所以使用了KaixinDeleteRecordEntity
        public static async Task<KaixinDeleteRecordEntity> ClearUnreadMessageCount(string access_token, string msg_type)
        {
            var parameters = new Dictionary<string, string>();

            parameters.Add("access_token", access_token);
            parameters.Add("msg_type", msg_type);

            return await API.Instance.DownloadString<KaixinDeleteRecordEntity>(API.Instance.GenerateRequestUri(KaixinConstant.METHODS.MessageClear, parameters));
        }

        /// <summary>
        /// 获取别人给我的评论。需设置 scope=user_comment 才能访问。HTTP请求方式：GET。
        /// </summary>
        /// <param name="access_token">授权的access token</param>
        /// <param name="start">起始位置，默认为0。非必须参数</param>
        /// <param name="num">返回结果数量，默认为20。非必须参数</param>
        /// <returns></returns>
        public static async Task<ReceivedCommentListEntity> GetCommentListForMe(string access_token, int? start, int? num)
        {
            var entity = await API.Instance.RequestCommentListForMe(access_token, start, num);
            return entity;
        }

        /// <summary>
        /// 获取我给别人的评论及回复。需设置 scope=user_comment 才能访问。HTTP请求方式：GET 。
        /// </summary>
        /// <param name="access_token">授权的access token</param>
        /// <param name="start">起始位置，默认为0。非必须参数</param>
        /// <param name="num">返回结果数量，默认为20。非必须参数</param>
        /// <returns></returns>
        public static async Task<RepliedCommentListEntity> GetMyRepliedCommentList(string access_token, int? start, int? num)
        {
            var entity = await API.Instance.RequestMyRepliedCommentList(access_token, start, num);
            return entity;
        }

        // ---------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 获取记录的热门话题，返回数量不定 GET
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public static async Task<List<KaixinHotTopicEntity>> GetHotTopics(string access_token)
        {
            var parameters = new Dictionary<string, string>();

            parameters.Add("access_token", access_token);

            return await API.Instance.DownloadString<List<KaixinHotTopicEntity>>(API.Instance.GenerateRequestUri(KaixinConstant.METHODS.RecordsTopics, parameters));
        }

        // ---------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 获取当前登录用户的资料。
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="fields">用户自定义返回字段，多个属性之间用英文半角逗号作为分隔符。默认用户的基本属性。</param>
        /// <returns></returns>
        public static async Task<FriendEntity> GetUserInfo(string access_token, string fields)
        {
            var entity = await API.Instance.RequestUserInfo(access_token, fields);
            return entity;
        }

        /// <summary>
        /// 根据 UID 获取多个用户的资料。
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="uids">用户 ID，可以设置多个，以半角逗号分隔。最多不能超过50个。</param>
        /// <param name="fields">用户自定义返回字段，多个属性之间用英文半角逗号作为分隔符。默认用户的基本属性。</param>
        /// <returns></returns>
        public static async Task<FriendListEntity> GetUserInfoList(string access_token, string uids, string fields)
        {
            var entity = await API.Instance.RequestUserInfo(access_token, uids, fields);
            return entity;
        }

        /// <summary>
        /// 获取当前登录用户的好友资料。
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="fields">用户自定义返回字段，多个属性之间用英文半角逗号作为分隔符。默认用户的基本属性。</param>
        /// <param name="start">起始搜索（默认0）。</param>
        /// <param name="count">返回记录的个数（默认20，最多50）。</param>
        /// <returns></returns>
        public static async Task<FriendListEntity> GetFriendList(string access_token, string fields, int? start, int? count)
        {
            var entity = await API.Instance.RequestFriendList(access_token, fields, start, count);
            return entity;
        }

        // -------------------------------------------------------------------------------------------------------
        // -------------------------------------------------------------------------------------------------------
        // -------------------------------------------------------------------------------------------------------

        #region 开心网API调用测试语句（新版，2016-05-21更新）

        //private async void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    string content = "MyHub test content";
        //    //string accessTokenKaixin = kaixinClientOAuth.Access_Token;
        //    string accessTokenKaixin = "159068599_19e826ccf397c8c14586105eed52be3e";
        //    string uid = "";
        //    string objtype = "records";

        //    var entity_20 = await KaixinSnsDataAccessMethods.GetUserInfo(accessTokenKaixin, "");
        //    uid = entity_20.Uid.ToString();

        //    var entity_0 = await KaixinSnsDataAccessMethods.GetFriendsRecords(accessTokenKaixin, null, null, null);
        //    var entity_1 = await KaixinSnsDataAccessMethods.GetUserHomeRecords(accessTokenKaixin, null, null, null, null);
        //    var entity_2 = await KaixinSnsDataAccessMethods.GetPublicRecords(accessTokenKaixin, null, null);
        //    var entity_3 = await KaixinSnsDataAccessMethods.PostRecord(accessTokenKaixin, content, "USA", null, null, null, null, null);
        //    var entity_4 = await KaixinSnsDataAccessMethods.DeleteRecord(accessTokenKaixin, entity_3.Rid);
        //    var entity_5 = await KaixinSnsDataAccessMethods.CreateForwardForSource(accessTokenKaixin, objtype, entity_0.FriendsRecordsList[0].Rid, entity_0.FriendsRecordsList[0].User.Uid, content);
        //    var entity_6 = await KaixinSnsDataAccessMethods.CreateCommentForSource(accessTokenKaixin, entity_0.FriendsRecordsList[0].Rid, entity_0.FriendsRecordsList[0].User.Uid, content, objtype, null);
        //    var entity_7 = await KaixinSnsDataAccessMethods.CreateLikeForSource(accessTokenKaixin, objtype, entity_0.FriendsRecordsList[0].Rid, entity_0.FriendsRecordsList[0].User.Uid);
        //    var entity_8 = await KaixinSnsDataAccessMethods.CancelLikeForSource(accessTokenKaixin, objtype, entity_0.FriendsRecordsList[0].Rid, entity_0.FriendsRecordsList[0].User.Uid);
        //    var entity_9 = await KaixinSnsDataAccessMethods.CheckLikeForSource(accessTokenKaixin, objtype, entity_0.FriendsRecordsList[0].Rid, entity_0.FriendsRecordsList[0].User.Uid, uid);///
        //    var entity_10 = await KaixinSnsDataAccessMethods.GetForwardList(accessTokenKaixin, objtype, entity_0.FriendsRecordsList[0].Rid, entity_0.FriendsRecordsList[0].User.Uid, null, null);
        //    var entity_11 = await KaixinSnsDataAccessMethods.GetCommentList(accessTokenKaixin, entity_0.FriendsRecordsList[0].Rid, entity_0.FriendsRecordsList[0].User.Uid, objtype, null, null);
        //    var entity_12 = await KaixinSnsDataAccessMethods.GetLikeUserList(accessTokenKaixin, objtype, entity_0.FriendsRecordsList[0].Rid, entity_0.FriendsRecordsList[0].User.Uid, null, null);
        //    var entity_13 = await KaixinSnsDataAccessMethods.ReplyCommentForSource(accessTokenKaixin, entity_0.FriendsRecordsList[0].Rid, entity_11.Data[0].Uid, entity_6.Data.Thread_cid, entity_0.FriendsRecordsList[0].User.Uid, content, objtype);
        //    var entity_14 = await KaixinSnsDataAccessMethods.DeleteCommentForSource(accessTokenKaixin, entity_0.FriendsRecordsList[0].User.Uid, entity_6.Data.Thread_cid);
        //    var entity_15 = await KaixinSnsDataAccessMethods.GetUnreadMessageCount(accessTokenKaixin);
        //    var entity_16 = await KaixinSnsDataAccessMethods.ClearUnreadMessageCount(accessTokenKaixin, "sysmsg_forward,comment");
        //    var entity_17 = await KaixinSnsDataAccessMethods.GetCommentListForMe(accessTokenKaixin, null, null);
        //    var entity_18 = await KaixinSnsDataAccessMethods.GetMyRepliedCommentList(accessTokenKaixin, null, null);
        //    var entity_19 = await KaixinSnsDataAccessMethods.GetHotTopics(accessTokenKaixin);
        //    var entity_21 = await KaixinSnsDataAccessMethods.GetUserInfoList(accessTokenKaixin, uid, "");
        //    var entity_22 = await KaixinSnsDataAccessMethods.GetFriendList(accessTokenKaixin, "", null, null);
        //}

        #endregion 开心网API调用测试语句（新版）


        #region 开心网API调用测试语句（旧版）

        //var entity = await API.Instance.RequestFriendList(_oAuthEntity.Access_Token, fields, start, count);//获取朋友列表   ------OK
        //var entity = await API.Instance.RequestFriendsRecords(_oAuthEntity.Access_Token, 0, 20, 0);//将所有的好友新消息，取出来并反序列化成为Entity   -------OK
        //var entity = await API.Instance.RequestCommentList(_oAuthEntity.Access_Token, "1461652675778745", "78276544", "records", 0, 10);//获取某条记录的评论列表 rid: 1461652675778745, uid: 78276544  ------OK
        //var entity = await API.Instance.CreateCommentForSource(_oAuthEntity.Access_Token, "1461652675778745", "78276544", "This is a test comment for method CreateCommentForSource.", "records", 0);//添加对某一资源的评论。 rid: 1461652675778745, uid: 78276544, thread_cid: 1278256389  ------OK
        //var entity = await API.Instance.DeleteCommentForSource(_oAuthEntity.Access_Token, "78276544", "1278256347"); //删除评论 uid:78276544, thread_cid: 1278256347  ------ OK
        //var entity = await API.Instance.ReplyCommentForSource(_oAuthEntity.Access_Token, "1461652675778745", "159068599", "1278256389", "78276544", "This is a test reply for method ReplyCommentForSource. -- From MyHub", "records"); //对某一资源的某条评论进行回复 rid:1461652675778745, uid:159068599, thread_cid: 1278256389, owner_uid:78276544  -------- OK
        //var entity = await API.Instance.CreateForwardForSource(_oAuthEntity.Access_Token, "records", "1461805161468308", "65589562", "This is test content for method CreateForwardForSource. -- Form MyHub");//对某一资源进行转发 rid: 1461805161468308, uid:65589562 fid:1461830130897562  -------- OK
        //var entity = await API.Instance.RequestForwardList(_oAuthEntity.Access_Token, "records", "1461805161468308", "65589562", 0, 20);//获取某一资源的所有转发  rid: 1461805161468308, uid:65589562   ------ OK
        //var entity = await API.Instance.CreateLikeForSource(_oAuthEntity.Access_Token, "records", "1461652675778745", "78276544");//表达对某一资源的赞    -------- OK
        //var entity = await API.Instance.CancelLikeForSource(_oAuthEntity.Access_Token, "records", "1461652675778745", "78276544");//取消对某一资源的赞    -------- OK
        //var entity = await API.Instance.ShowLikeUserList(_oAuthEntity.Access_Token, "records", "1461652675778745", "78276544", 0, 10);//获取对某一资源赞过的用户列表 rid: 1461652675778745, uid: 78276544    -------- OK
        //var entity = await API.Instance.RequestCommentListForMe(_oAuthEntity.Access_Token, 0, 20);//获取别人给我的评论  --------- OK
        //var entity = await API.Instance.RequestMyRepliedCommentList(_oAuthEntity.Access_Token, 0, 20);//获取我给别人的评论及回复   -------- OK
        //var entity = await API.Instance.RequestUserInfo(_oAuthEntity.Access_Token, fields);//获取当前登录用户资料
        //var entity = await API.Instance.PostRecord(_oAuthEntity.Access_Token, content, null, null, null, null, null, null);//发布记录

        #endregion 开心网API调用测试语句（旧版）
    }
}
