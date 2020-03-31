using System;
using System.Collections.Generic;

namespace CatLog.Api.Data.Models
{
    public partial class Article
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Introduction { get; set; }
        public long ViewsCount { get; set; }
        public DateTime Time { get; set; }
        public long LikeCount { get; set; }
        public long ColumnId { get; set; }

        public virtual Column Column { get; set; }
    }
}
