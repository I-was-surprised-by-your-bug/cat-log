using CatLog.Api.Data.Models;
using CatLog.Api.DtoParameters;
using CatLog.Api.Helpers;
using System.Threading.Tasks;

namespace CatLog.Api.Data.Interfaces
{
    public interface IArticleRepository
    {
        Task<PagedList<Article>> GetArticlesAsync(ArticleDtoParameters parameters);
        Task<PagedList<Article>> GetArticlesForColumnAsync(long columnId, ArticleDtoParameters parameters);
        Task<Article> GetArticleAsync(long articleId);
        Task<Article> GetArticleForColumnAsync(long columnId, long articleId);
        void AddArticle(Article article);
        void UpdateArticle(Article article);
        void RemoveArticle(Article article);
        Task<bool> ArticleExistsAsync(long articleId);
        Task<bool> SaveAsync();
    }
}
