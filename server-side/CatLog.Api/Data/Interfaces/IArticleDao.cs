using CatLog.Api.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatLog.Api.Data.Interfaces
{
    public interface IArticleDao
    {
        Task<IEnumerable<Article>> GetArticlesAsync();
        Task<Article> GetArticleAsync(long articleId);
        void AddArticle(Article article);
        void UpdateArticle(Article article);
        void RemoveArticle(Article article);
        Task<bool> ArticleExistsAsync(long articleId);
        Task<bool> SaveAsync();
    }
}
