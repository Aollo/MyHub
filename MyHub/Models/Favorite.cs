using System;
using System.Collections.Generic;

namespace MyHub.Models
{
    public class Favorite
    {
        public Status StatusInfo { get; set; }

        public List<FavoriteTag> Tags { get; set; }

        public DateTime FavoriteTime { get; set; }
    }
}
