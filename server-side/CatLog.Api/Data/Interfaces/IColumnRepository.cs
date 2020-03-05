using CatLog.Api.Data.Models;
using CatLog.Api.DtoParameters;
using CatLog.Api.Helpers;
using System.Threading.Tasks;

namespace CatLog.Api.Data.Interfaces
{
    public interface IColumnRepository
    {
        Task<PagedList<Column>> GetColumnsForSectionAsync(long sectionId, ColumnDtoParameters parameters);
        Task<Column> GetColumnForSectionAsync(long sectionId, long columnId);
        void AddColumn(Column column);
        void UpdateColumn(Column column);
        void RemoveColumn(Column column);
        Task<bool> SectionExistsAsync(long sectionId);
        Task<bool> ColumnExistsAsync(long columnId);
        Task<bool> ColumnExistsAsync(long sectionId, long columnId);
        Task<bool> SaveAsync();
    }
}
