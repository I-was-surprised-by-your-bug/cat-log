using CatLog.Api.Services.Implements;
using System.Collections.Generic;

namespace CatLog.Api.Services.Interfaces
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
        bool ValidMappingExistsFor<TSource, TDestination>(string orderBy);
    }
}