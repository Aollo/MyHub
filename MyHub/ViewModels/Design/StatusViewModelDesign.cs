using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MyHub.Models;

namespace MyHub.ViewModels.Design
{
    public class StatusViewModelDesign
    {
        private Status _status = new Status
        {
            Sns = new SnsType { ID = 1, Name = "新浪微博" },
            StatusId = "1111",
            Author = new User
            {
                UserId = "111111",
                NickName = "用户名",
                LogoUrl = "ms-appx:///Assets/StoreLogo.png"
            },
            Content = "这是一条新鲜事 这是一条新鲜事 这是一条新鲜事 这是一条新鲜事 这是一条新鲜事",
            CreateTime = DateTime.Now,
            Source = "MyHub",
            ThumbnailPicUrl = "ms-appx:///Assets/StoreLogo.png",
            OriginalPicUrl = null,
            PictureUrls = null,
            Favorited = false,
            RepostsCount = 10,
            CommentsCount = 1,
            AttitudesCount = 0,
            LocationInfo = null,
            RetweetedStatus = null
        };

        public Status OneStatus
        {
            get
            {
                return _status;
            }
        }

        public ObservableCollection<Status> StatusList
        {
            get
            {
                return new ObservableCollection<Status>()
                {
                    _status,
                    _status,
                    _status,
                    _status,
                    _status,
                    _status,
                    _status,
                    _status,
                };
            }
        }

        public ObservableCollection<string> SnsTypes
        {
            get
            {
                return new ObservableCollection<string>
                {
                    "整合显示",
                    "新浪微博",
                    "开心网"
                };
            }
        }

        public StatusViewModelDesign()
        {
            
        }
    }
}