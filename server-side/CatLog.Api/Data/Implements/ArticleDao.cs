using CatLog.Api.Data.Contexts;
using CatLog.Api.Data.Interfaces;
using CatLog.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }
            return await _context.Articles.AnyAsync(x => x.Id == articleId);
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
            if (articleId == null)
            {
                throw new ArgumentNullException(nameof(articleId));
            }
            return _context.Articles.Where(x => x.Id == articleId).FirstOrDefault();
        }

        public async Task<IEnumerable<Article>> GetArticlesAsync()
        {
            return await _context.Articles.ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            throw new NotImplementedException();
        }

        void IArticleDao.UpdateArticle(Article article)
        {
            // EF Core 自动实现
        }
    }
}
