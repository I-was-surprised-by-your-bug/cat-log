using CatLog.Api.Data.Models;
using CatLog.Api.DtoParameters;
using CatLog.Api.Helpers;
using System.Threading.Tasks;

namespace CatLog.Api.Data.Interfaces
{
    public interface ISectionDao
    {
        Task<PagedList<Section>> GetSectionsAsync(SectionDtoParameters parameters);
        Task<Section> GetSectionAsync(long sectionId);
        void AddSection(Section section);
        void UpdateSection(Section section);
        void RemoveSection(Section section);
        Task<bool> SectionExistsAsync(long sectionId);
        Task<bool> SaveAsync();
    }
}
