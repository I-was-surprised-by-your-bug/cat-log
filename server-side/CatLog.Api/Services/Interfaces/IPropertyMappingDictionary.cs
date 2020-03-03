using System.Collections.Generic;

namespace CatLog.Api.Services.Interfaces
{
    public interface IPropertyMappingDictionary
    {
        /*
         * 因为不能在 IList<T> 中直接使用泛型了，无法解析 IList<PropertyMappingDictionary<TSource, TDestination>>
         * 所有需要定义一个接口 IPropertyMappingDictionary，让 PropertyMappingDictionary 实现这个接口
         * 然后使用 IList<IPropertyMappingDictionary> 来实现
         */
    }
}