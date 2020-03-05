using CatLog.Api.Data.Contexts;
using CatLog.Api.Data.Interfaces;
using CatLog.Api.Data.Models;
using CatLog.Api.DtoParameters;
using CatLog.Api.Dtos;
using CatLog.Api.Helpers;
using CatLog.Api.Services.Implements;
using CatLog.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatLog.Api.Data.Implements
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly CatLogContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public ArticleRepository(CatLogContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public void AddArticle(Article article)
        {
            if (article is null)
            {
                throw new ArgumentNullException(nameof(article));
            }
            _context.TArticles.Add(article);
        }

        public async Task<bool> ArticleExistsAsync(long articleId)
        {
            /*
             * 使用 MySql.Data.EntityFrameworkCore 库 8.0.19 版本时，此处引发错误
             * InvalidOperationException: No coercion operator is defined between types 'System.Int16' and 'System.Boolean'.
             * 更换为 Pomelo.EntityFrameworkCore.MySql 库后运行正常
             * https://ask.csdn.net/questions/1056992
             */
            return await _context.TArticles.AnyAsync(x => x.Id == articleId);
        }

        public void RemoveArticle(Article article)
        {
            if (article is null)
            {
                throw new ArgumentNullException(nameof(article));
            }
            _context.TArticles.Remove(article);
        }

        public async Task<Article> GetArticleAsync(long articleId)
        {
            return await _context.TArticles.FirstOrDefaultAsync(x => x.Id == articleId);
        }

        public async Task<PagedList<Article>> GetArticlesAsync(ArticleDtoParameters parameters)
        {
            var queryExpression = _context.TArticles as IQueryable<Article>;

            //映射关系字典
            Dictionary<string, PropertyMappingValue> mappingDictionary = null;

#warning 允许 OrderBy 所有属性，即使该属性未被 Select
            if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
            {
                mappingDictionary = _propertyMappingService.GetPropertyMapping<ArticleDto, Article>();
                queryExpression = queryExpression.ApplyOrderBy(parameters.OrderBy, mappingDictionary);
            }
            if (!string.IsNullOrWhiteSpace(parameters.Select))
            {
                // 通过 ??= 减少获取映射关系字典的次数
                mappingDictionary ??= _propertyMappingService.GetPropertyMapping<ArticleDto, Article>();
                queryExpression = queryExpression.ApplySelect(parameters.Select, mappingDictionary);
            }
            
            return await PagedList<Article>.CreateAsync(queryExpression, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public void UpdateArticle(Article article)
        {
            // EF Core 自动实现
        }
    }
}
