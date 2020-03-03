using System;
using System.Collections.Generic;

namespace CatLog.Api.Services.Implements
{
    /// <summary>
    /// 单条属性映射关系
    /// </summary>
    public class PropertyMappingValue
    {
        public IEnumerable<string> DestinationProperties { get; set; }

        /// <summary>
        /// 排序时顺序是否需要翻转
        /// </summary>
        public bool Revert { get; set; }

        public PropertyMappingValue(IEnumerable<string> destinaryProperties, bool revert = false)
        {
            DestinationProperties = destinaryProperties ?? throw new ArgumentNullException(nameof(destinaryProperties));
            Revert = revert;
        }
    }
}
