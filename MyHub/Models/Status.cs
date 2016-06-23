using System;
using System.Collections.Generic;
using MyHub.ComponentModel;

namespace MyHub.Models
{
    public class Status : ObservableObjectBase
    {

        public SnsType Sns { get; set; }

        public string StatusId { get; set; }

        public User Author { get; set; }

        public string Content { get; set; }

        public DateTime CreateTime { get; set; }

        public string Source { get; set; }

        public string ThumbnailPicUrl { get; set; }

        public string OriginalPicUrl { get; set; }

        public List<string> PictureUrls { get; set; }

        public bool Favorited { get; set; }

        private int _repostCount;
        public int RepostsCount
        {
            get { return _repostCount; }
            set
            {
                if(_repostCount != value)
                {
                    _repostCount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _commentCount;
        public int CommentsCount
        {
            get { return _commentCount; }
            set
            {
                if (_commentCount != value)
                {
                    _commentCount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int _attitudeCount;
        public int AttitudesCount
        {
            get { return _attitudeCount; }
            set
            {
                if (_attitudeCount != value)
                {
                    _attitudeCount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public Location LocationInfo { get; set; }

        public Status RetweetedStatus { get; set; }
        
    }
}
