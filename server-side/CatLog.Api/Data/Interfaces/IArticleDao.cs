using CatLog.Api.DtoParameters;
using CatLog.Api.Entities;
using CatLog.Api.Helpers;
using System.Threading.Tasks;

namespace CatLog.Api.Data.Interfaces
{
    public interface IArticleDao
    {
        Task<PagedList<Article>> GetArticlesAsync(ArticleDtoParameters parameters);
        Task<Article> GetArticleAsync(long articleId);
        void AddArticle(Article article);
        void UpdateArticle(Article article);
        void RemoveArticle(Article article);
        Task<bool> ArticleExistsAsync(long articleId);
        Task<bool> SaveAsync();
    }
}
