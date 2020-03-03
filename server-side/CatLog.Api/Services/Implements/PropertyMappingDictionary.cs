using CatLog.Api.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace CatLog.Api.Services.Implements
{
    /// <summary>
    /// 指明 TSource 与 TDestination 的属性映射关系字典
    /// </summary>
    /// <typeparam name="TSource">源类型</typeparam>
    /// <typeparam name="TDestination">目标类型</typeparam>
    public class PropertyMappingDictionary<TSource, TDestination> : IPropertyMappingDictionary
    {
        /// <summary>
        /// 属性映射关系字典
        /// </summary>
        public Dictionary<string, PropertyMappingValue> MappingDictionary { get; private set; }
        public PropertyMappingDictionary(Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            MappingDictionary = mappingDictionary ?? throw new ArgumentNullException(nameof(mappingDictionary));
        }
    }
}
