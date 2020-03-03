using CatLog.Api.Data.Contexts;
using CatLog.Api.Data.Interfaces;
using CatLog.Api.DtoParameters;
using CatLog.Api.Entities;
using CatLog.Api.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CatLog.Api.Data.Implements
{
    public class ArticleDao : IArticleDao
    {
        private readonly DbCatLogContext _context;

        public ArticleDao(DbCatLogContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddArticle(Article article)
        {
            if (article == null)
            {
                throw new ArgumentNullException(nameof(article));
            }
            _context.Articles.Add(article);
        }

        public async Task<bool> ArticleExistsAsync(long articleId)
        {
            return await _context.Articles.AnyAsync(x => x.Id==articleId); ////BUG
        }

        public void RemoveArticle(Article article)
        {
            if (article == null)
            {
                throw new ArgumentNullException(nameof(article));
            }
            _context.Articles.Remove(article);
        }

        public async Task<Article> GetArticleAsync(long articleId)
        {
            return _context.Articles.FirstOrDefault(x => x.Id == articleId);
        }

        public async Task<PagedList<Article>> GetArticlesAsync(ArticleDtoParameters parameters)
        {
            var queryExpression = _context.Articles as IQueryable<Article>;

            if (!string.IsNullOrWhiteSpace(parameters.Fields))
            {
                // 数据塑形
            }
            return await PagedList<Article>.CreateAsync(queryExpression, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        void IArticleDao.UpdateArticle(Article article)
        {
            // EF Core 自动实现
        }
    }
}
