using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatLog.Api.Entities
{
    [Table("t_articles")]
    public class Article
    {
        /// <summary>
        /// 文章ID
        /// </summary>
        [Column("article_id")]
        public long Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Column("article_title")]
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Column("article_content")]
        public string Content { get; set; }

        /// <summary>
        /// 发表时间
        /// </summary>
        [Column("article_time")]
        public DateTime Time { get; set; }

        /// <summary>
        /// 点击量
        /// </summary>
        [Column("article_views_count")]
        public int ViewsCount { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        [Column("article_like_count")]
        public int LikeCount { get; set; }
    }
}
