using CatLog.Api.Entities;
using CatLog.Api.Models;
using CatLog.Api.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatLog.Api.Services.Implements
{
    /// <summary>
    /// 属性映射服务
    /// </summary>
    public class PropertyMappingService : IPropertyMappingService
    {
        //ArticleDto 的属性映射关系字典
        private readonly Dictionary<string, PropertyMappingValue> _articlePropertyMapping
            = new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase) //Key 大小写不敏感
            {
                {"Id",new PropertyMappingValue(new List<string>{"Id"}) },
                {"Title",new PropertyMappingValue(new List<string>{"Title"}) },
                {"Introduction",new PropertyMappingValue(new List<string>{"Introduction"}) },
                {"Content",new PropertyMappingValue(new List<string>{"Content"}) },
                {"Time",new PropertyMappingValue(new List<string>{"Time"}) },
                {"ViewsCount",new PropertyMappingValue(new List<string>{"ViewsCount"}) },
                {"LikeCount",new PropertyMappingValue(new List<string>{"LikeCount"}) },
            };

        /// <summary>
        /// “指明 TSource 与 TDestination 的属性映射关系字典”的列表
        /// </summary>
        private IList<IPropertyMappingDictionary> _propertyMappings = new List<IPropertyMappingDictionary>();

        public PropertyMappingService()
        {
            //向列表中添加属性映射关系字典，同时指明该字典对应的源类型与目标类型
            //即向列表中添加“指明 TSource 与 TDestination 的属性映射关系字典”
            _propertyMappings.Add(new PropertyMappingDictionary<ArticleDto, Article>(_articlePropertyMapping));
        }

        /// <summary>
        /// 如果 TSource 与 TDestination 存在映射关系，返回属性映射关系字典
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <returns>从源类型到目标类型的属性映射关系</returns>
        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = _propertyMappings.OfType<PropertyMappingDictionary<TSource, TDestination>>();
            var propertyMapping = matchingMapping.ToList();
            if (propertyMapping.Count != 1)
            {
                throw new Exception($"无法找到{typeof(TSource)}与{typeof(TDestination)}的映射关系");
            }
            //如果 TSource 与 TDestination 存在映射关系，返回对应的属性映射关系字典
            return propertyMapping.First().MappingDictionary;
        }

        /// <summary>
        /// 判断 Uri query parameters 的 orderBy 或 select 字符串是否合法
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="parameter">Uri Query 中的 parameter 字符串，大小写不敏感</param>
        /// <returns>parameter 字符串是否合法</returns>
        public bool ValidMappingExistsFor<TSource, TDestination>(string parameter)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();
            if (string.IsNullOrWhiteSpace(parameter))
            {
                return true;
            }
            var fieldAfterSplit = parameter.Split(",");
            foreach (var field in fieldAfterSplit)
            {
                var trimedField = field.Trim();
                var indexOfFirstSpace = trimedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ? trimedField : trimedField.Remove(indexOfFirstSpace);
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
