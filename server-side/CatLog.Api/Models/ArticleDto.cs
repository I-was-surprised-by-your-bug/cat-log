using System;

namespace CatLog.Api.Models
{
    public class ArticleDto
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 引言
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 发表时间
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 点击量
        /// </summary>
        public int ViewsCount { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        public int LikeCount { get; set; }
    }
}
