using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WeiboSDKForWinRT;
using MyHub.Models.Weibo;
using MyHub.Tools;

namespace MyHub.Services
{
    /// <summary>
    /// 获取新浪微博信息的常用方法
    /// 一些方法的详细参数及说明请参照新浪微博开放平台API文档
    /// </summary>
    public static class WeiboSnsDataAccessMethods
    {
        /// <summary>
        /// 获取当前登录用户及关注用户的微博 
        /// </summary>
        /// <param name="baseAppPara">是否只获取当前应用的数据。0为否（所有数据），1为是（仅当前应用），默认为0。 </param>
        /// <param name="countPara">单页返回的记录条数，最大不超过100，默认为20。 </param>
        /// <param name="featurePara">过滤类型ID，0：全部、1：原创、2：图片、3：视频、4：音乐，默认为0。 </param>
        /// <param name="maxIDPara">若指定此参数，则返回ID小于或等于max_id的微博，默认为0。 </param>
        /// <param name="pagePara">返回结果的页码，默认为1。 </param>
        /// <param name="sinceIDPara">若指定此参数，则返回ID比since_id大的微博（即比since_id时间晚的微博），默认为0。 </param>
        /// <returns></returns>
        public static async Task<WeiboStatusesEntity> GetTimelines(string baseAppPara, string countPara, string featurePara, string maxIDPara, string pagePara, string sinceIDPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdTimelines()
            {
                BaseApp = baseAppPara,
                Count = countPara,
                Feature = featurePara,
                MaxID = maxIDPara,
                Page = pagePara,
                SinceID = sinceIDPara,
            };
            var result = await engine.RequestCmd(SdkRequestType.FRIENDS_TIMELINE, cmdBase);//向服务器发送请求，获取当前登录用户及关注用户的微博

            try
            {
                if (result.errCode == SdkErrCode.SUCCESS)
                {
                    var resultEntity = (WeiboStatusesEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboStatusesEntity));
                    return resultEntity;
                }
                else// 获取信息没有成功
                {
                    WeiboStatusesEntity errorResultEntry = new WeiboStatusesEntity();
                    errorResultEntry.Statuses = new List<WeiboStatusEntity>();
                    return errorResultEntry;
                }
            }
            catch (Exception)// 反序列化信息时出现问题
            {
                WeiboStatusesEntity errorResultEntry = new WeiboStatusesEntity();
                errorResultEntry.Statuses = new List<WeiboStatusEntity>();
                return errorResultEntry;
            }

        }

        /// <summary>
        /// 获取某个用户最新发表的微博列表
        /// 注意：只能获取当前授权用户的发布的微博列表，screen name可以不用写
        /// </summary>
        /// <param name="baseAppPara">是否只获取当前应用的数据。0为否（所有数据），1为是（仅当前应用），默认为0。 </param>
        /// <param name="countPara">单页返回的记录条数，最大不超过100，超过100以100处理，默认为20。 </param>
        /// <param name="featurePara">过滤类型ID，0：全部、1：原创、2：图片、3：视频、4：音乐，默认为0。 </param>
        /// <param name="maxIDPara">若指定此参数，则返回ID小于或等于max_id的微博，默认为0。 </param>
        /// <param name="pagePara">返回结果的页码，默认为1。 </param>
        /// <param name="screenNamePara">需要查询的用户昵称。 </param>
        /// <param name="sinceIDPara">若指定此参数，则返回ID比since_id大的微博（即比since_id时间晚的微博），默认为0。 </param>
        /// <param name="userIDPara">需要查询的用户ID。 </param>
        /// <returns></returns>
        public static async Task<WeiboStatusesEntity> GetUserTimeline(string baseAppPara, string countPara, string featurePara, string maxIDPara, string pagePara, string screenNamePara, string sinceIDPara, string userIDPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdUserTimeline() //
            {
                BaseApp = baseAppPara,
                Count = countPara,
                Feature = featurePara,
                MaxID = maxIDPara,
                Page = pagePara,
                ScreenName = screenNamePara,
                SinceID = sinceIDPara,
                UserId = userIDPara
            };
            var result = await engine.RequestCmd(SdkRequestType.USER_TIMELINE, cmdBase);//向服务器发送请求，获取某个用户最新发表的微博列表

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboStatusesEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboStatusesEntity));//获取某个用户最新发表的微博列表
                return resultEntity;
            }
            else
            {
                return null;
            }
        }


        public static async Task<WeiboStatusesEntity> GetUserHomeTimeline(string countPara, string featurePara, string maxIDPara, string pagePara, string sinceIDPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdUserHomeTimeline()
            {
                Count = countPara,
                Max_id = maxIDPara,
                Page = pagePara,
                Since_id = sinceIDPara,
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取某个用户最新发表的微博列表

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboStatusesEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboStatusesEntity));//获取某个用户最新发表的微博列表
                return resultEntity;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 返回最新的公共微博
        /// http://open.weibo.com/wiki/2/statuses/public_timeline
        /// </summary>
        /// <param name="baseAppPara">是否只获取当前应用的数据。0为否（所有数据），1为是（仅当前应用），默认为0。 </param>
        /// <param name="countPara">单页返回的记录条数，最大不超过100，超过100以100处理，默认为20。 </param>
        /// <param name="pagePara">返回结果的页码，默认为1。 </param>
        /// <returns></returns>
        public static async Task<WeiboStatusesEntity> GetPulicTimeline(string baseAppPara, string countPara, string pagePara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdPublicTimelines()
            {
                BaseApp = baseAppPara,
                Count = countPara,
                Page = pagePara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取某个用户最新发表的微博列表

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboStatusesEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboStatusesEntity));//获取某个用户最新发表的微博列表
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取某个位置周边的动态
        /// http://open.weibo.com/wiki/2/place/nearby_timeline
        /// </summary>
        /// <param name="longitudePara">经度。有效范围：-180.0到+180.0，+表示东经</param>
        /// <param name="latitudePara">纬度。有效范围：-90.0到+90.0，+表示北纬。</param>
        /// <param name="rangePara">搜索范围，单位米，默认2000米，最大11132米。 </param>
        /// <param name="countPara">单页返回的记录条数，最大为50，默认为20。</param>
        /// <param name="pagePara">返回结果的页码，默认为1。</param>
        /// <returns></returns>
        public static async Task<WeiboStatusesEntity> GetLocalPublicTimeline(string latitudePara, string longitudePara, string rangePara, string countPara, string pagePara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdLocalPublicTimelines()
            {
                Longitude = longitudePara,
                Latitude = latitudePara,
                Range = rangePara,
                Count = countPara,
                Page = pagePara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取某个用户最新发表的微博列表

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboStatusesEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboStatusesEntity));//获取某个用户最新发表的微博列表
                return resultEntity;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 发布一条新微博
        /// </summary>
        /// <param name="statusPara">要发布的微博文本内容，必须做URLencode，内容不超过140个汉字。 </param>
        /// <param name="latPara">纬度，有效范围：-90.0到+90.0，+表示北纬，默认为0.0。 </param>
        /// <param name="longPara">经度，有效范围：-180.0到+180.0，+表示东经，默认为0.0。 </param>
        /// <returns></returns>
        /// 可用的测试内容：statusPara = "test for post message without picture --- From MyHub"
        public static async Task<WeiboStatusEntity> PostMessage(string statusPara, string latPara, string longPara)
        {
            var engine = new SdkNetEngine();

            if (latPara == null)
                latPara = "";
            if (longPara == null)
                longPara = "";

            ISdkCmdBase cmdBase = new CmdPostMessage() { Annotations = string.Empty, Lat = latPara, Long = longPara, ReplyId = string.Empty, Status = statusPara };
            var result = await engine.RequestCmd(SdkRequestType.POST_MESSAGE, cmdBase);//向服务器发送请求，发布一条新微博

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboStatusEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboStatusEntity));//发布一条新微博
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 上传图片并发布一条微博
        /// </summary>
        /// <param name="statusPara">要发布的微博文本内容，必须做URLencode，内容不超过140个汉字。 </param>
        /// <param name="picPathPara">要上传的图片，仅支持JPEG、GIF、PNG格式，图片大小小于5M。 </param>
        /// <param name="longPara">经度，有效范围：-180.0到+180.0，+表示东经，默认为0.0。 </param>
        /// <param name="latPara">纬度，有效范围：-90.0到+90.0，+表示北纬，默认为0.0。 </param>
        /// <returns></returns>
        /// 可用的测试内容： status = "test for post message with picture  ---- From MyHub"
        public static async Task<WeiboStatusEntity> PostMessageWithPicture(string statusPara, string picPathPara, string longPara, string latPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdPostMsgWithPic()
            {
                Status = statusPara,
                PicPath = picPathPara,
                Long = longPara,
                Lat = latPara
            };
            var result = await engine.RequestCmd(SdkRequestType.POST_MESSAGE_PIC, cmdBase);//向服务器发送请求，上传图片并发布一条微博

            if (result != null && result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboStatusEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboStatusEntity));//将 上传图片并发布一条新微博 返回结果反序列化
                return resultEntity;
            }
            else//如果向服务器发送请求失败，返回null
            {
                return null;
            }
        }

        /// <summary>
        /// 根据微博ID删除指定微博
        /// </summary>
        /// <param name="idPara">需要删除的微博ID。</param>
        /// <returns></returns>
        public static async Task<WeiboStatusEntity> DestroyStatuses(string idPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdStatusesDestroy()
            {
                Id = idPara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboStatusEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboStatusEntity));//根据微博ID删除指定微博
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 转发一条微博
        /// </summary>
        /// <param name="idPara">要转发的微博ID</param>
        /// <param name="statusPara">添加的转发文本，必须做URLencode，内容不超过140个汉字，不填则默认为“转发微博”。 </param>
        /// <param name="isCommentPara">是否在转发的同时发表评论，0：否、1：评论给当前微博、2：评论给原微博、3：都评论，默认为0 。 </param>
        /// <param name="ripPara">开发者上报的操作用户真实IP，形如：211.156.0.1。</param>
        /// <returns></returns>
        public static async Task<WeiboStatusEntity> ReportStatuses(string idPara, string statusPara, string isCommentPara, string ripPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdStatusesReport() { Id = idPara, Status = statusPara, Is_comment = isCommentPara, Rip = ripPara };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboStatusEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboStatusEntity));
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 对一条微博进行评论
        /// </summary>
        /// <param name="commentPara">评论内容，必须做URLencode，内容不超过140个汉字。 </param>
        /// <param name="idPara">需要评论的微博ID。 </param>
        /// <param name="commentOriPara">当评论转发微博时，是否评论给原微博，0：否、1：是，默认为0。 </param>
        /// <param name="ripPara">开发者上报的操作用户真实IP，形如：211.156.0.1。</param>
        /// <returns></returns>
        /// 可用的测试内容：Comment = "This is a comment from method CmdCommentsCreate  ---- From MyHub.", Id = "3686640654304387“
        public static async Task<WeiboCommentEntity> CreateComments(string idPara, string commentPara, string commentOriPara, string ripPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdCommentsCreate()
            {
                Comment = commentPara,
                Id = idPara,
                Comment_ori = commentOriPara,
                Rip = ripPara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboCommentEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboCommentEntity));//对一条微博进行评论
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 添加一条微博到收藏里
        /// http://open.weibo.com/wiki/2/favorites/create
        /// </summary>
        /// <param name="idPara"></param>
        /// <returns></returns>
        public static async Task<WeiboFavoriteStatusEntity> CreateFavorite(string idPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdFavoriteCreate()
            {
                Id = idPara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboFavoriteStatusEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboFavoriteStatusEntity));//对一条微博进行评论
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 取消收藏一条微博
        /// http://open.weibo.com/wiki/2/favorites/destroy
        /// </summary>
        /// <param name="idPara"></param>
        /// <returns></returns>
        public static async Task<WeiboFavoriteStatusEntity> DestroyFavorite(string idPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdFavoriteDestroy()
            {
                Id = idPara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboFavoriteStatusEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboFavoriteStatusEntity));//对一条微博进行评论
                return resultEntity;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 获取指定微博的转发微博列表 
        /// </summary>
        /// <param name="idPara">需要查询的微博ID。 </param>
        /// <param name="sinceIdPara">若指定此参数，则返回ID比since_id大的微博（即比since_id时间晚的微博），默认为0。 </param>
        /// <param name="maxIdPara">若指定此参数，则返回ID小于或等于max_id的微博，默认为0。 </param>
        /// <param name="countPara">单页返回的记录条数，最大不超过200，默认为20。 </param>
        /// <param name="pagePara">返回结果的页码，默认为1。 </param>
        /// <param name="filterByAuthorPara">作者筛选类型，0：全部、1：我关注的人、2：陌生人，默认为0。 </param>
        /// <returns></returns>
        public static async Task<WeiboReportsListEntity> GetReportTimeline(string idPara, string sinceIdPara, string maxIdPara, string countPara, string pagePara, string filterByAuthorPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdReportTimeline()
            {
                Id = idPara,
                Since_id = sinceIdPara,
                Max_id = maxIdPara,
                Count = countPara,
                Page = pagePara,
                Filter_by_author = filterByAuthorPara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboReportsListEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboReportsListEntity));//获取指定微博的转发微博列表 
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据微博ID返回某条微博的评论列表 
        /// </summary>
        /// <param name="idPara">需要查询的微博ID。</param>
        /// <param name="sinceIdPara">若指定此参数，则返回ID比since_id大的评论（即比since_id时间晚的评论），默认为0。</param>
        /// <param name="maxIdPara">若指定此参数，则返回ID小于或等于max_id的评论，默认为0。 </param>
        /// <param name="countPara">单页返回的记录条数，默认为50。 </param>
        /// <param name="pagePara">返回结果的页码，默认为1</param>
        /// <param name="filter_by_authorPara">作者筛选类型，0：全部、1：我关注的人、2：陌生人，默认为0。  </param>
        /// <returns></returns>
        public static async Task<WeiboCommentsListEntity> GetCommentsList(string idPara, string sinceIdPara, string maxIdPara, string countPara, string pagePara, string filter_by_authorPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdShowComment() { Id = idPara, Since_id = sinceIdPara, Max_id = maxIdPara, Count = countPara, Page = pagePara, Filter_by_author = filter_by_authorPara };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboCommentsListEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboCommentsListEntity));//根据微博ID返回某条微博的评论列表 -------OK
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 回复一条评论
        /// </summary>
        /// <param name="cidPara">需要回复的评论ID。</param>
        /// <param name="commentPara">评论内容，必须做URLencode，内容不超过140个汉字。 </param>
        /// <param name="idPara">需要评论的微博ID。</param>
        /// <param name="withoutMentionPara">回复中是否自动加入“回复@用户名”，0：是、1：否，默认为0。 </param>
        /// <param name="commentOriPara">当评论转发微博时，是否评论给原微博，0：否、1：是，默认为0。 </param>
        /// <param name="ripPara">开发者上报的操作用户真实IP，形如：211.156.0.1。</param>
        /// <returns></returns>
        /// 可用的测试内容： Cid = "3686673977843078", Id = "3686640654304387", Comment = "This is a reply from method CmdCommentReply ---- From MyHub."
        public static async Task<WeiboCommentEntity> ReplyComments(string cidPara, string commentPara, string idPara, string withoutMentionPara, string commentOriPara, string ripPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdCommentsReply()
            {
                Cid = cidPara,
                Comment = commentPara,
                Id = idPara,
                Without_mention = withoutMentionPara,
                Comment_ori = commentOriPara,
                Rip = ripPara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboCommentEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboCommentEntity));//回复一条评论
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 删除一条评论
        /// </summary>
        /// <param name="cidPara">要删除的评论ID，只能删除登录用户自己发布的评论。</param>
        /// <returns></returns>
        /// 可用的测试内容：Cid = "3686686623568259"
        public static async Task<WeiboCommentEntity> DestroyComments(string cidPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdCommentsDestroy() { Cid = cidPara };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboCommentEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboCommentEntity));//删除一条评论 ------ OK
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        // ------------------------------------------------------------------------------------------

        /// <summary>
        /// 获取某个用户的各种消息未读数
        /// http://open.weibo.com/wiki/2/remind/unread_count
        /// </summary>
        /// <param name="uidPara">需要获取消息未读数的用户UID，必须是当前登录用户。</param>
        /// <returns></returns>
        public static async Task<WeiboRemindEntity> GetRemindUnreadCount(string uidPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdRemindUnreadCount() { Uid = uidPara};
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboRemindEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboRemindEntity));
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 对当前登录用户某一种消息未读数进行清零【高权限接口】
        /// http://open.weibo.com/wiki/2/remind/set_count
        /// </summary>
        /// <param name="type">"follower", "cmt", "dm", "mention_status", "mention_cmt", "group", "notice", "invite", "badge", "photo"中的一个</param>
        /// <returns></returns>
        public static async Task<bool> ClearRemindCount(string type)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdRemindClearCount() { Type = type };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取当前登录用户的最新评论包括接收到的与发出的
        /// http://open.weibo.com/wiki/2/comments/timeline
        /// </summary>
        /// <param name="sinceIdPara">若指定此参数，则返回ID比since_id大的评论（即比since_id时间晚的评论），默认为0。</param>
        /// <param name="maxIdPara">若指定此参数，则返回ID小于或等于max_id的评论，默认为0。 </param>
        /// <param name="countPara">单页返回的记录条数，默认为50。 </param>
        /// <param name="pagePara">返回结果的页码，默认为1。 </param>
        /// <param name="trimUserPara">返回值中user字段开关，0：返回完整user字段、1：user字段仅返回user_id，默认为0。</param>
        /// <returns></returns>
        public static async Task<WeiboCommentsListEntity> GetCommentsAboutMe(string sinceIdPara, string maxIdPara, string countPara, string pagePara, string trimUserPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdCommentsAboutMe
            {
                Since_id = sinceIdPara,
                Max_id = maxIdPara,
                Count = countPara,
                Page = pagePara,
                Trim_User = trimUserPara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboCommentsListEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboCommentsListEntity));//获取当前登录用户所收到的评论列表
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取当前登录用户所接收到的评论列表 
        /// </summary>
        /// <param name="sinceIdPara">若指定此参数，则返回ID比since_id大的评论（即比since_id时间晚的评论），默认为0。</param>
        /// <param name="maxIdPara">若指定此参数，则返回ID小于或等于max_id的评论，默认为0。 </param>
        /// <param name="countPara">单页返回的记录条数，默认为50。 </param>
        /// <param name="pagePara">返回结果的页码，默认为1。</param>
        /// <param name="filter_by_authorPara">作者筛选类型，0：全部、1：我关注的人、2：陌生人，默认为0。  </param>
        /// <param name="filter_by_sourcePara">来源筛选类型，0：全部、1：来自微博的评论、2：来自微群的评论，默认为0。 </param>
        /// <returns></returns>
        public static async Task<WeiboCommentsListEntity> GetCommentsToMe(string sinceIdPara, string maxIdPara, string countPara, string pagePara, string filter_by_authorPara, string filter_by_sourcePara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdCommentsToMe()
            {
                Since_id = sinceIdPara,
                Max_id = maxIdPara,
                Count = countPara,
                Page = pagePara,
                Filter_by_author = filter_by_authorPara,
                Filter_by_source = filter_by_sourcePara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboCommentsListEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboCommentsListEntity));//获取当前登录用户所收到的评论列表
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取当前登录用户所发出的评论列表
        /// </summary>
        /// <param name="sinceIdPara">若指定此参数，则返回ID比since_id大的评论（即比since_id时间晚的评论），默认为0。</param>
        /// <param name="maxIdPara">若指定此参数，则返回ID小于或等于max_id的评论，默认为0。 </param>
        /// <param name="countPara">单页返回的记录条数，默认为50。 </param>
        /// <param name="pagePara">返回结果的页码，默认为1。</param>
        /// <param name="filter_by_authorPara">作者筛选类型，0：全部、1：我关注的人、2：陌生人，默认为0。</param>
        /// <returns></returns>
        public static async Task<WeiboCommentsListEntity> GetCommentsByMe(string sinceIdPara, string maxIdPara, string countPara, string pagePara, string filter_by_authorPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdCommentsByMe() { Since_id = sinceIdPara, Max_id = maxIdPara, Count = countPara, Page = pagePara, Filter_by_author = filter_by_authorPara };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboCommentsListEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboCommentsListEntity));//获取当前登录用户所发出的评论列表  -----OK
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取最新的提到登录用户的微博列表，即@我的微博 
        /// http://open.weibo.com/wiki/2/statuses/mentions
        /// </summary>
        /// <param name="sinceIdPara">若指定此参数，则返回ID比since_id大的评论（即比since_id时间晚的评论），默认为0。</param>
        /// <param name="maxIdPara">若指定此参数，则返回ID小于或等于max_id的评论，默认为0。 </param>
        /// <param name="countPara">单页返回的记录条数，默认为50。 </param>
        /// <param name="pagePara">返回结果的页码，默认为1。</param>
        /// <param name="filter_by_authorPara">作者筛选类型，0：全部、1：我关注的人、2：陌生人，默认为0。</param>
        /// <param name="filter_by_sourcePara">来源筛选类型，0：全部、1：来自微博、2：来自微群，默认为0。 </param>
        /// <returns></returns>
        public static async Task<WeiboStatusesEntity> GetMentionsStatus(string sinceIdPara, string maxIdPara, string countPara, string pagePara, string filter_by_authorPara, string filter_by_sourcePara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdMentionsStatus()
            {
                Since_id = sinceIdPara,
                Max_id = maxIdPara,
                Count = countPara,
                Page = pagePara,
                Filter_by_author = filter_by_authorPara,
                Filter_by_source = filter_by_sourcePara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboStatusesEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboStatusesEntity));//获取最新的提到当前登录用户的评论，即@我的评论
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取最新的提到当前登录用户的评论，即@我的评论
        /// </summary>
        /// <param name="sinceIdPara">若指定此参数，则返回ID比since_id大的评论（即比since_id时间晚的评论），默认为0。</param>
        /// <param name="maxIdPara">若指定此参数，则返回ID小于或等于max_id的评论，默认为0。 </param>
        /// <param name="countPara">单页返回的记录条数，默认为50。 </param>
        /// <param name="pagePara">返回结果的页码，默认为1。 </param>
        /// <param name="filter_by_authorPara">作者筛选类型，0：全部、1：我关注的人、2：陌生人，默认为0。  </param>
        /// <param name="filter_by_sourcePara">来源筛选类型，0：全部、1：来自微博的评论、2：来自微群的评论，默认为0。 </param>
        /// <returns></returns>
        public static async Task<WeiboCommentsListEntity> GetMentionsComments(string sinceIdPara, string maxIdPara, string countPara, string pagePara, string filter_by_authorPara, string filter_by_sourcePara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdCommentsMentions()
            {
                Since_id = sinceIdPara,
                Max_id = maxIdPara,
                Count = countPara,
                Page = pagePara,
                Filter_by_author = filter_by_authorPara,
                Filter_by_source = filter_by_sourcePara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboCommentsListEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboCommentsListEntity));//获取最新的提到当前登录用户的评论，即@我的评论
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        // ------------------------------------------------------------------------------------------

        /// <summary>
        /// 返回最近一周内的热门话题(目前无法解析发挥的Json数据）
        /// http://open.weibo.com/wiki/2/trends/weekly
        /// </summary>
        /// <param name="baseappPara"></param>
        /// <returns></returns>
        public static async Task<WeiboTrendsEntity> GetTrendsWeekly(string baseappPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdTrendsWeekly()
            {
                Base_app = baseappPara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboTrendsEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboTrendsEntity));
                return resultEntity;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 搜索某一话题下的微博【高权限接口】
        /// http://open.weibo.com/wiki/2/search/topics
        /// </summary>
        /// <param name="topicPara"></param>
        /// <param name="pagePara"></param>
        /// <param name="countPara"></param>
        /// <returns></returns>
        public static async Task<WeiboStatusesEntity> SearchStatusWithTopic(string topicPara, string countPara, string pagePara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdSearchStatusWithTopic()
            {
                Topic = topicPara,
                Page = pagePara,
                Count = countPara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboStatusesEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboStatusesEntity));
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据用户名获取用户信息
        /// </summary>
        /// <param name="userIdPara">需要查询的用户用户ID。</param>
        /// <returns></returns>
        public static async Task<WeiboUserEntity> GetUserInfomation(string userIdPara, string screenNamePara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdUsersShow()
            {
                Screen_name = screenNamePara,
                Uid = userIdPara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboUserEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboUserEntity));//根据用户ID获取用户信息
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取当前登录用户的收藏列表
        /// http://open.weibo.com/wiki/2/favorites
        /// </summary>
        /// <param name="pagePara"></param>
        /// <param name="countPara"></param>
        /// <returns></returns>
        public static async Task<WeiboFavoriteStatusesListEntity> GetUserFavoriteList(string pagePara, string countPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdGetFavoritesList()
            {
                Page = pagePara,
                Count = countPara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboFavoriteStatusesListEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboFavoriteStatusesListEntity));
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取用户的粉丝列表
        /// </summary>
        /// <param name="screenNamePara">需要查询的用户昵称。 </param>
        /// <param name="countPara">单页返回的记录条数，默认为50，最大不超过200。</param>
        /// <param name="cursorPara">返回结果的游标，下一页用返回值里的next_cursor，上一页用previous_cursor，默认为0。</param>
        /// <param name="trimStatusPara">返回值中user字段中的status字段开关，0：返回完整status字段、1：status字段仅返回status_id，默认为1。</param>
        /// <returns></returns>
        public static async Task<WeiboUsersListEntity> GetFollowersList(string screenNamePara, string countPara, string cursorPara, string trimStatusPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdFriendshipsFollowers()
            {
                Screen_name = screenNamePara,
                Count = countPara,
                Cursor = cursorPara,
                Trim_status = trimStatusPara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboUsersListEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboUsersListEntity));//获取用户的粉丝列表
                return resultEntity;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 获取用户关注的列表
        /// </summary>
        /// <param name="userIdPara">需要查询的用户id。 </param>
        /// <param name="cursorPara">返回结果的游标，下一页用返回值里的next_cursor，上一页用previous_cursor，默认为0。 </param>
        /// <param name="countPara">单页返回的记录条数，默认为50，最大不超过200。 </param>
        /// <param name="trimStatusPara">返回值中user字段中的status字段开关，0：返回完整status字段、1：status字段仅返回status_id，默认为1。 </param>
        /// <returns></returns>
        public static async Task<WeiboUsersListEntity> GetFriendsList(string userIdPara, string cursorPara, string countPara, string trimStatusPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdFriendList() { Uid = userIdPara, Cursor = cursorPara, Count = countPara, TrimStatus = trimStatusPara };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboUsersListEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboUsersListEntity));
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        // ------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------

        /// <summary>
        /// 关注一个用户
        /// </summary>
        /// <param name="screenNamePara">微博昵称</param>
        /// <returns></returns>
        public static async Task<WeiboUserEntity> CreateFriendships(string screenNamePara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdFriendShip() { ScreenName = screenNamePara };
            var result = await engine.RequestCmd(SdkRequestType.FRIENDSHIP_CREATE, cmdBase);//向服务器发送请求，关注一个用户

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboUserEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboUserEntity));//关注一个用户 -------- OK
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 取消关注某用户 
        /// </summary>
        /// <param name="screenNamePara">微博昵称</param>
        /// <returns></returns>
        public static async Task<WeiboUserEntity> DestroyFriendships(string screenNamePara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdFriendShip() { ScreenName = screenNamePara };
            var result = await engine.RequestCmd(SdkRequestType.FRIENDSHIP_DESDROY, cmdBase);//向服务器发送请求，取消关注某用户

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboUserEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboUserEntity));
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取两个用户关系的详细情况
        /// </summary>
        /// <param name="screenNamePara">目标用户的微博昵称。 </param>
        /// <param name="sourceScreenNamePara">源用户的微博昵称。 </param>
        /// <returns></returns>
        public static async Task<WeiboFriendshipsEntity> GetFriendships(string screenNamePara, string sourceScreenNamePara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdFriendShip() { SourceScreenName = sourceScreenNamePara, ScreenName = screenNamePara };
            var result = await engine.RequestCmd(SdkRequestType.FRIENDSHIP_SHOW, cmdBase);//向服务器发送请求，获取两个用户关系的详细情况

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboFriendshipsEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboFriendshipsEntity));
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取两个用户之间的共同关注人列表
        /// </summary>
        /// <param name="uidPara">需要获取共同关注关系的用户UID。</param>
        /// <param name="suidPara">需要获取共同关注关系的用户UID，默认为当前登录用户。 </param>
        /// <param name="countPara">单页返回的记录条数，默认为50。 </param>
        /// <param name="pagePara">返回结果的页码，默认为1。</param>
        /// <param name="trimStatusPara">返回值中user字段中的status字段开关，0：返回完整status字段、1：status字段仅返回status_id，默认为1。</param>
        /// <returns></returns>
        /// 可用的测试内容：Uid = "1658716582", Count = "20"
        public static async Task<WeiboUsersListEntity> GetFriendsInCommon(string uidPara, string suidPara, string countPara, string pagePara, string trimStatusPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdFriendshipsFriendsInCommon()
            {
                Uid = uidPara,
                Suid = suidPara,
                Count = countPara,
                Page = pagePara,
                Trim_status = trimStatusPara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboUsersListEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboUsersListEntity));//获取两个用户之间的共同关注人列表
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取附近地点
        /// http://open.weibo.com/wiki/2/place/nearby/pois
        /// </summary>
        /// <param name="latitudePara">纬度，有效范围：-90.0到+90.0，+表示北纬。 </param>
        /// <param name="longitudePara">经度，有效范围：-180.0到+180.0，+表示东经。 </param>
        /// <param name="rangePara">查询范围半径，默认为2000，最大为10000，单位米。 </param>
        /// <param name="keywordPara">查询的关键词，必须进行URLencode。 </param>
        /// <param name="countPara">单页返回的记录条数，默认为20，最大为50。 </param>
        /// <param name="pagePara">返回结果的页码，默认为1。 </param>
        /// <returns></returns>
        public static async Task<WeiboPositionEntity> GetNearbyPois(string latitudePara, string longitudePara, string rangePara, string keywordPara, string countPara, string pagePara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdNearbyPois()
            {
                Latitude = latitudePara,
                Longitude = longitudePara,
                Range = rangePara,
                Keyword = keywordPara,
                Count = countPara,
                Page = pagePara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboPositionEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboPositionEntity));
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据关键词按地址位置获取POI点的信息 
        /// http://open.weibo.com/wiki/2/location/pois/search/by_location
        /// </summary>
        /// <param name="keywordPara">查询的关键词，必须进行URLencode</param>
        /// <param name="pagePara">返回结果的页码，默认为1，最大为40。 </param>
        /// <param name="countPara">单页返回的记录条数，默认为10，最大为20。 </param>
        /// <returns></returns>
        public static async Task<WeiboPositionEntity> SearchPoisByLocation(string keywordPara, string pagePara, string countPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdSearchPoisByLocation()
            {
                Keyword = keywordPara,
                Count = countPara,
                Page = pagePara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboPositionEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboPositionEntity));
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 长链转短链
        /// </summary>
        /// <param name="originalUrl">原始链接</param>
        /// <returns></returns>
        /// 可用的测试数据：originalUrl = "http://wwww.baidu.com"
        public static async Task<WeiboUrlEntity> ShortenUrl(string originalUrl)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdShortenUrl() { OriginalUrl = originalUrl };
            var result = await engine.RequestCmd(SdkRequestType.SHORTEN_URL, cmdBase);

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboUrlEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboUrlEntity));
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 将一个或多个短链接还原成原始的长链接
        /// </summary>
        /// <param name="urlShortPara"></param>
        /// <returns></returns>
        /// 可用的测试内容：Url_short = "http://t.cn/h4DwT1"
        public static async Task<WeiboUrlEntity> GetShortUrlExpanded(string urlShortPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdShorturlExpand()
            {
                Url_short = urlShortPara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (WeiboUrlEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboUrlEntity));//将一个或多个短链接还原成原始的长链接
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// @联想搜索
        /// </summary>
        /// <param name="countPara">返回的记录条数，默认为10，粉丝最多1000，关注最多2000。 </param>
        /// <param name="keywordPara">搜索的关键字，必须做URLencoding。 </param>
        /// <param name="rangePara">联想范围，0：只联想关注人、1：只联想关注人的备注、2：全部，默认为2。 </param>
        /// <param name="typePara">联想类型，0：关注、1：粉丝。 </param>
        /// <returns></returns>
        public static async Task<List<WeiboSearchSuggestionsAtUsersEntity>> GetSearchSuggestionAtUser(string countPara, string keywordPara, string rangePara, string typePara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdAtUsers() { Count = countPara, Keyword = keywordPara, Range = rangePara, Type = typePara };
            var result = await engine.RequestCmd(SdkRequestType.AT_USERS, cmdBase);

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (List<WeiboSearchSuggestionsAtUsersEntity>)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(List<WeiboSearchSuggestionsAtUsersEntity>));
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 搜索用户时的联想搜索建议
        /// http://open.weibo.com/wiki/2/search/suggestions/users
        /// </summary>
        /// <param name="countPara"></param>
        /// <param name="keywordPara"></param>
        /// <returns></returns>
        public static async Task<List<WeiboSearchSuggestionUserEntity>> GetSearchSuggestionUsers(string countPara, string keywordPara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdSearchSuggestionUsers()
            {
                Count = countPara,
                Keyword = keywordPara
            };
            var result = await engine.RequestCmd(SdkRequestType.AT_USERS, cmdBase);

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (List<WeiboSearchSuggestionUserEntity>)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(List<WeiboSearchSuggestionUserEntity>));
                return resultEntity;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取微博官方表情的详细信息
        /// </summary>
        /// <param name="typePara">表情类别，face：普通表情、ani：魔法表情、cartoon：动漫表情，默认为face。 </param>
        /// <param name="languagePara">语言类别，cnname：简体、twname：繁体，默认为cnname。</param>
        /// <returns></returns>
        public static async Task<List<WeiboEmotionsEntity>> GetWeiboEmotions(string typePara, string languagePara)
        {
            var engine = new SdkNetEngine();

            ISdkCmdBase cmdBase = new CmdEmotions()
            {
                Type = typePara,
                Language = languagePara
            };
            var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

            if (result.errCode == SdkErrCode.SUCCESS)
            {
                var resultEntity = (List<WeiboEmotionsEntity>)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(List<WeiboEmotionsEntity>));//获取微博官方表情的详细信息 
                return resultEntity;
            }
            else
            {
                return null;
            }
        }


        // ------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------

        #region 新浪微博API调用测试语句（新版）

        //private async void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    string pageNumber = "1";
        //    string pageCount = "10";
        //    string screenName = "秋泽Aollo";
        //    string userId = "2325214344";

        //    var entity_0 = await WeiboSnsDataAccessMethods.GetTimelines("", pageCount, "", "", pageNumber, "");
        //    var entity_1 = await WeiboSnsDataAccessMethods.GetUserTimeline("", pageCount, "", "", pageNumber, "", "", "");
        //    var entity_2 = await WeiboSnsDataAccessMethods.GetPulicTimeline("", pageCount, pageNumber);
        //    var entity_3 = await WeiboSnsDataAccessMethods.GetLocalPublicTimeline("37.5193220000", "122.1282450000", "", pageCount, pageNumber);
        //    var entity_4 = await WeiboSnsDataAccessMethods.PostMessage("test post message", "37.5193220000", "122.1282450000");
        //    var entity_5 = await WeiboSnsDataAccessMethods.PostMessageWithPicture("test post picture message", "", "", "");
        //    var entity_6 = await WeiboSnsDataAccessMethods.DestroyStatuses(entity_4.idstr);
        //    var entity_7 = await WeiboSnsDataAccessMethods.ReportStatuses(entity_0.Statuses[0].idstr, "test report message", "", "");
        //    var entity_8 = await WeiboSnsDataAccessMethods.CreateComments(entity_7.idstr, "comment message", "", "");
        //    var entity_9 = await WeiboSnsDataAccessMethods.CreateFavorite(entity_7.idstr);
        //    var entity_10 = await WeiboSnsDataAccessMethods.DestroyFavorite(entity_9.status.idstr);
        //    var entity_11 = await WeiboSnsDataAccessMethods.GetReportTimeline(entity_0.Statuses[0].idstr, "", "", pageCount, pageNumber, "");
        //    var entity_12 = await WeiboSnsDataAccessMethods.GetCommentsList(entity_7.idstr, "", "", pageCount, pageNumber, "");
        //    var entity_13 = await WeiboSnsDataAccessMethods.ReplyComments(entity_8.idstr, "test reply commments", entity_7.idstr, "", "", "");
        //    var entity_14 = await WeiboSnsDataAccessMethods.DestroyComments(entity_13.idstr);
        //    var entity_15 = await WeiboSnsDataAccessMethods.GetRemindUnreadCount(userId);
        //    var entity_16 = await WeiboSnsDataAccessMethods.ClearRemindCount(entity_4.user.idstr);
        //    var entity_17 = await WeiboSnsDataAccessMethods.GetCommentsAboutMe("", "", pageCount, pageNumber, "");
        //    var entity_18 = await WeiboSnsDataAccessMethods.GetCommentsToMe("", "", pageCount, pageNumber, "", "");
        //    var entity_19 = await WeiboSnsDataAccessMethods.GetCommentsByMe("", "", pageCount, pageNumber, "");
        //    var entity_20 = await WeiboSnsDataAccessMethods.GetMentionsStatus("", "", pageCount, pageNumber, "", "");
        //    var entity_21 = await WeiboSnsDataAccessMethods.GetMentionsComments("", "", pageCount, pageNumber, "", "");
        //    var entity_22 = await WeiboSnsDataAccessMethods.SearchStatusWithTopic("Windows 10", pageCount, pageNumber);
        //    var entity_23 = await WeiboSnsDataAccessMethods.GetUserInfomation(screenName);
        //    var entity_24 = await WeiboSnsDataAccessMethods.GetUserFavoriteList(pageNumber, pageCount);
        //    var entity_25 = await WeiboSnsDataAccessMethods.GetFollowersList(screenName, pageCount, "", "");
        //    var entity_26 = await WeiboSnsDataAccessMethods.GetFriendsList(screenName, "", pageCount, "");
        //    var entity_27 = await WeiboSnsDataAccessMethods.ShortenUrl("http://www.baidu.com");
        //    var entity_28 = await WeiboSnsDataAccessMethods.GetShortUrlExpanded(entity_27.urls[0].url_short);
        //    var entity_29 = await WeiboSnsDataAccessMethods.GetSearchSuggestionAtUser("", "宇哥", "", "");
        //    var entity_30 = await WeiboSnsDataAccessMethods.GetSearchSuggestionUsers(pageCount, "宇哥");
        //    var entity_31 = await WeiboSnsDataAccessMethods.GetWeiboEmotions("", "");
        //    var entity_32 = await WeiboSnsDataAccessMethods.GetTrendsWeekly("");
        //}

        #endregion 新浪微博API调用测试语句（新版）

        #region 新浪微博API调用测试语句（旧版）
        /// <summary>
        /// 新浪微博API调用测试语句
        /// </summary>
        //private async void GetWeiboStatusButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var engine = new SdkNetEngine();

        //    ISdkCmdBase cmdBase = new CmdPostMsgWithPic() { Status = "test for post message with picture  ---- From MyHub", PicPath = picPath };//上传图片并发布一条微博
        //    //ISdkCmdBase cmdBase = new CmdShortenUrl() { OriginalUrl = "http://wwww.baidu.com" };//长链转短链
        //    //ISdkCmdBase cmdBase = new CmdAtUsers() { Keyword = "a" };//@联想搜索
        //    //ISdkCmdBase cmdBase = new CmdFriendShip() { SourceScreenName = "KevinF_HIT", ScreenName = "秋泽Aollo" };//获取两个用户关系的详细情况 KevinF_HIT 花之粥
        //    //ISdkCmdBase cmdBase = new CmdFriendShip() { ScreenName = "秋泽Aollo" };//取消关注某用户 
        //    //ISdkCmdBase cmdBase = new CmdFriendShip() { ScreenName = "秋泽Aollo" };//关注一个用户 
        //    //ISdkCmdBase cmdBase = new CmdUserTimeline() { Count = "20", Page = "1" };//获取某个用户最新发表的微博列表 
        //    //ISdkCmdBase cmdBase = new CmdPostMessage() { Status = "test for post message without picture --- From MyHub" };//发布一条新微博 
        //    //ISdkCmdBase cmdBase = new CmdTimelines() { Count = "20", Page = "1" };//获取当前登录用户及关注用户的微博 
        //    //ISdkCmdBase cmdBase = new CmdFriendList() { Count = "20", Uid = "1658716582", };//获取用户关注的列表  //------------------------------------------------以下是发送OtherAPI请求
        //    //ISdkCmdBase cmdBase = new CmdStatusesReport() { Id = "3686640654304387", Status = "This is the content while repost status  ---- From MyHub." };//转发一条微博 
        //    //ISdkCmdBase cmdBase = new CmdShowComment() { Id = "3686640654304387", Count = "20"};//根据微博ID返回某条微博的评论列表 
        //    //ISdkCmdBase cmdBase = new CmdCommentsByMe() { Count = "20" };//获取当前登录用户所发出的评论列表 
        //    //ISdkCmdBase cmdBase = new CmdCommentsToMe() { Count = "20", };//获取当前登录用户所接收到的评论列表 
        //    //ISdkCmdBase cmdBase = new CmdCommentsMentions() { Count = "20" };//获取最新的提到当前登录用户的评论，即@我的评论 
        //    //ISdkCmdBase cmdBase = new CmdCommentsCreate() { Comment = "This is a comment from method CmdCommentsCreate  ---- From MyHub.", Id = "3686640654304387", };//对一条微博进行评论 
        //    //ISdkCmdBase cmdBase = new CmdCommentsDestroy() { Cid = "3686686623568259" };//删除一条评论 
        //    //ISdkCmdBase cmdBase = new CmdCommentsReply() { Cid = "3686673977843078", Id = "3686640654304387", Comment = "This is a reply from method CmdCommentReply ---- From MyHub." };//回复一条评论 
        //    //ISdkCmdBase cmdBase = new CmdUsersShow() { Screen_name = "秋泽Aollo" };//根据用户ID获取用户信息 id = 2325214344, 1658716582
        //    //ISdkCmdBase cmdBase = new CmdStatusesDestroy() { Id = "3686665027495345" };//根据微博ID删除指定微博
        //    //ISdkCmdBase cmdBase = new CmdFriendshipsFriendsInCommon() { Uid = "1658716582", Count = "20" };//获取两个用户之间的共同关注人列表
        //    //ISdkCmdBase cmdBase = new CmdFriendshipsFollowers() { Screen_name = "秋泽Aollo", Count = "20" };//获取用户的粉丝列表
        //    //ISdkCmdBase cmdBase = new CmdRegisterVerifyNickname() { Nickname = "" };//验证昵称是否可用，并给予建议昵称 
        //    //ISdkCmdBase cmdBase = new CmdShorturlExpand() { Url_short = "http://t.cn/h4DwT1" };//将一个或多个短链接还原成原始的长链接 
        //    //ISdkCmdBase cmdBase = new CmdStatusesMentionsShield() { Id = "0" };//屏蔽某个@到我的微博以及后续由对其转发引起的@提及，高级接口（需要授权）
        //    //ISdkCmdBase cmdBase = new CmdStatusesFilterCreate() { Id = "3686669809486250" };//屏蔽某条微博，高级接口（需要授权）
        //    //ISdkCmdBase cmdBase = new CmdEmotions() { };//获取微博官方表情的详细信息 

        //    var result = await engine.RequestCmd(SdkRequestType.POST_MESSAGE_PIC, cmdBase);//向服务器发送请求，上传图片并发布一条微博
        //    //var result = await engine.RequestCmd(SdkRequestType.SHORTEN_URL, cmdBase);//向服务器发送请求，长链转短链
        //    //var result = await engine.RequestCmd(SdkRequestType.AT_USERS, cmdBase);//向服务器发送请求，@联想搜索
        //    //var result = await engine.RequestCmd(SdkRequestType.FRIENDSHIP_SHOW, cmdBase);//向服务器发送请求，获取两个用户关系的详细情况
        //    //var result = await engine.RequestCmd(SdkRequestType.FRIENDSHIP_DESDROY, cmdBase);//向服务器发送请求，取消关注某用户
        //    //var result = await engine.RequestCmd(SdkRequestType.FRIENDSHIP_CREATE, cmdBase);//向服务器发送请求，关注一个用户
        //    //var result = await engine.RequestCmd(SdkRequestType.USER_TIMELINE, cmdBase);//向服务器发送请求，获取某个用户最新发表的微博列表
        //    //var result = await engine.RequestCmd(SdkRequestType.POST_MESSAGE, cmdBase);//向服务器发送请求，发布一条新微博
        //    //var result = await engine.RequestCmd(SdkRequestType.FRIENDS_TIMELINE, cmdBase);//向服务器发送请求，获取当前登录用户及关注用户的微博
        //    //var result = await engine.RequestCmd(SdkRequestType.OTHER_API, cmdBase);//向服务器发送请求，获取其他API信息

        //    if (result.errCode == SdkErrCode.SUCCESS)
        //    {
        //        this.ShowResultTextBlock.Text = "Success!!!!";

        //        var resultEntity = (StatusesUploadEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(StatusesUploadEntity));//将 上传图片并发布一条新微博 返回结果反序列化  ------- OK
        //        //var resultEntity = (WeiboUrlEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboUrlEntity));//长链转短链  ------OK
        //        //var resultEntity = (List<WeiboSearchSuggestionsAtUsersEntity>)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(List<WeiboSearchSuggestionsAtUsersEntity>));//@联想搜索 ------- OK
        //        //var resultEntity = (WeiboFriendshipsEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboFriendshipsEntity));//获取两个用户关系的详细情况   --------- OK
        //        //var resultEntity = (FriendshipsDestroyEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(FriendshipsDestroyEntity));//取消关注某用户  --------- OK
        //        //var resultEntity = (FriendshipsCreateEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(FriendshipsCreateEntity));//关注一个用户 -------- OK
        //        //var resultEntity = (UserTimelineEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(UserTimelineEntity));//获取某个用户最新发表的微博列表   -------- OK
        //        //var resultEntity = (StatusesUpdateEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(StatusesUpdateEntity));//发布一条新微博 ------ OK
        //        //var resultEntity = (WeiboStatusesEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboStatusesEntity));//获取当前登录用户及关注用户的微博  ----- OK
        //        //var resultEntity = (FriendshipsFriendsListEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(FriendshipsFriendsListEntity));//获取用户关注的列表  ------ OK  //------------------------------------------------以下是发送OtherAPI请求
        //        //var resultEntity = (ReportStatusesEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(ReportStatusesEntity));//转发一条微博  ----- OK
        //        //var resultEntity = (ShowCommentEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(ShowCommentEntity));//根据微博ID返回某条微博的评论列表 -------OK
        //        //var resultEntity = (GetCommentsByMeEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(GetCommentsByMeEntity));//获取当前登录用户所发出的评论列表  -----OK
        //        //var resultEntity = (GetCommentsToMeEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(GetCommentsToMeEntity));//获取当前登录用户所收到的评论列表  -------- OK
        //        //var resultEntity = (GetCommentsToMeEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(GetCommentsToMeEntity));//获取最新的提到当前登录用户的评论，即@我的评论  ------- OK
        //        //var resultEntity = (CreateCommentsEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(CreateCommentsEntity));//对一条微博进行评论   ------ OK
        //        //var resultEntity = (DestroyCommentsEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(DestroyCommentsEntity));//删除一条评论 ------ OK
        //        //var resultEntity = (ReplyCommentsEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(ReplyCommentsEntity));//回复一条评论  ------ OK
        //        //var resultEntity = (WeiboUserEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboUserEntity));//根据用户ID获取用户信息   -------- OK
        //        //var resultEntity = (DestroyStatusesEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(DestroyStatusesEntity));//根据微博ID删除指定微博  ------- OK
        //        //var resultEntity = (GetFriendsInCommonEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(GetFriendsInCommonEntity));//获取两个用户之间的共同关注人列表  -------OK 
        //        //var resultEntity = (GetFriendshipsFollowersEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(GetFriendshipsFollowersEntity));//获取用户的粉丝列表   ------- OK
        //        //var resultEntity = (VerifyNicknameEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(VerifyNicknameEntity));//验证昵称是否可用，并给予建议昵称   -------- OK
        //        //var resultEntity = (ExpandShorturlEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(ExpandShorturlEntity));//将一个或多个短链接还原成原始的长链接  -------- OK
        //        //var resultEntity = (ShieldMentionsStatusesEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(ShieldMentionsStatusesEntity));//屏蔽某个@到我的微博以及后续由对其转发引起的@提及，高级接口（需要授权）
        //        //var resultEntity = (WeiboStatusEntity)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(WeiboStatusEntity));//屏蔽某条微博，高级接口（需要授权）
        //        //var resultEntity = (List<WeiboEmotionsEntity>)JsonHelper.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(result.content)), typeof(List<WeiboEmotionsEntity>));//获取微博官方表情的详细信息   ---------- OK

        //        //for (int i = 0; i < resultEntity.Statuses.Count; i++)
        //        //{
        //        //    StatusCommonInfomation tempCommonInfomation = new StatusCommonInfomation();

        //        //    tempCommonInfomation.Content = resultEntity.Statuses[i].text;
        //        //    tempCommonInfomation.Created_Time = resultEntity.Statuses[i].created_at;
        //        //    tempCommonInfomation.UserInfo.Name = resultEntity.Statuses[i].user.name;

        //        //    commonStatus.Add(tempCommonInfomation);
        //        //}

        //        //this.ShowResultTextBlock.Text = result.content;
        //    }
        //    else
        //    {
        //        // TODO: deal the error.
        //        this.ShowResultTextBlock.Text = "Failed!!!!";
        //    }
        //}
        #endregion 新浪微博API调用测试语句
    }
}
