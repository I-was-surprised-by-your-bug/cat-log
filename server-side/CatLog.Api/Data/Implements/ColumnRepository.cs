using CatLog.Api.Data.Contexts;
using CatLog.Api.Data.Interfaces;
using CatLog.Api.Data.Models;
using CatLog.Api.DtoParameters;
using CatLog.Api.Dtos;
using CatLog.Api.Helpers;
using CatLog.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CatLog.Api.Data.Implements
{
    public class ColumnRepository : IColumnRepository
    {
        private readonly CatLogContext _context;
        private readonly IPropertyMappingService _propertyMappingService;

        public ColumnRepository(CatLogContext context, IPropertyMappingService propertyMappingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public void AddColumn(Column column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }
            _context.TColumns.Add(column);
        }

        public async Task<bool> SectionExistsAsync(long sectionId)
        {
            return await _context.TSections.AnyAsync(x => x.Id == sectionId);
        }

        public async Task<bool> ColumnExistsAsync(long columnId)
        {
            return await _context.TColumns.AnyAsync(x => x.Id == columnId);
        }

        public async Task<bool> ColumnExistsAsync(long sectionId, long columnId)
        {
            return await _context.TColumns.AnyAsync(x => x.SectionId == sectionId && x.Id == columnId);
        }

        public void RemoveColumn(Column column)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }
            _context.TColumns.Remove(column);
        }

        public async Task<Column> GetColumnAsync(long columnId)
        {
            return _context.TColumns.FirstOrDefault(x => x.Id == columnId);
        }

        public async Task<PagedList<Column>> GetColumnsAsync(long sectionId, ColumnDtoParameters parameters)
        {
            var queryExpression = _context.TColumns.Where(x => x.SectionId == sectionId);


            if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
            {
                //取得映射关系字典
                var mappingDictionary = _propertyMappingService.GetPropertyMapping<ColumnDto, Column>();
                queryExpression = queryExpression.ApplyOrderBy(parameters.OrderBy, mappingDictionary);
            }

            return await PagedList<Column>.CreateAsync(queryExpression, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public void UpdateColumn(Column column)
        {
            // EF Core 自动实现
        }
    }
}
