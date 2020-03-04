using CatLog.Api.Data.Contexts;
using CatLog.Api.Data.Interfaces;
using CatLog.Api.Data.Models;
using CatLog.Api.DtoParameters;
using CatLog.Api.Helpers;
using CatLog.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CatLog.Api.Data.Implements
{
    public class SectionDao : ISectionDao
    {
        private readonly CatLogContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public SectionDao(CatLogContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public void AddSection(Section section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }
            _context.TSections.Add(section);
        }

        public async Task<bool> SectionExistsAsync(long sectionId)
        {
            return await _context.TSections.AnyAsync(x => x.Id == sectionId);
        }

        public void RemoveSection(Section section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }
            _context.TSections.Remove(section);
        }

        public async Task<Section> GetSectionAsync(long sectionId)
        {
            return _context.TSections.FirstOrDefault(x => x.Id == sectionId);
        }

        public async Task<PagedList<Section>> GetSectionsAsync(SectionDtoParameters parameters)
        {
            var queryExpression = _context.TSections as IQueryable<Section>;

            //取得映射关系字典
            var mappingDictionary = _propertyMappingService.GetPropertyMapping<SectionDto, Section>();

            if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
            {
                queryExpression = queryExpression.ApplyOrderBy(parameters.OrderBy, mappingDictionary);
            }
            
            return await PagedList<Article>.CreateAsync(queryExpression, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public void UpdateSection(Section section)
        {
            // EF Core 自动实现
        }
    }
}
