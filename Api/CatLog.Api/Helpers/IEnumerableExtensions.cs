using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace CatLog.Api.Helpers
{
    /// <summary>
    /// IEnumerable<T> 的拓展方法
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// 对集合资源进行数据塑形
        /// </summary>
        /// <typeparam name="TSource">资源类型</typeparam>
        /// <param name="source">资源集合</param>
        /// <param name="select">Uri Query 中的 fields 字符串，大小写不敏感，用于指明需要的属性/字段；如果需要所有属性传入 null 即可</param>
        /// <returns>塑形后的集合资源</returns>
        public static IEnumerable<ExpandoObject> ShapeData<TSource>(this IEnumerable<TSource> source,
                                                                    string select)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var expandoObjectList = new List<ExpandoObject>(source.Count());
            var propertyInfoList = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(select))
            {
                var propertyInfos = typeof(TSource)
                                    .GetProperties(BindingFlags.Public
                                    | BindingFlags.Instance);
                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                var fieldsAfterSplit = select.Split(",");
                foreach (var field in fieldsAfterSplit)
                {
                    var propertyName = field.Trim();
                    var propertyInfo = typeof(TSource)
                                       .GetProperty(propertyName,
                                                   BindingFlags.IgnoreCase //IgnoreCase 忽略大小写
                                                   | BindingFlags.Public
                                                   | BindingFlags.Instance);
                    if (propertyInfo == null)
                    {
                        throw new Exception($"Property:{propertyName} 没有找到：{typeof(TSource)}");
                    }

                    propertyInfoList.Add(propertyInfo);
                }
            }

            foreach (TSource obj in source)
            {
                var shapedObj = new ExpandoObject();

                foreach (var propertyInfo in propertyInfoList)
                {
                    var propertyValue = propertyInfo.GetValue(obj);
                    ((IDictionary<string, object>)shapedObj).Add(propertyInfo.Name, propertyValue);
                }

                expandoObjectList.Add(shapedObj);
            }

            return expandoObjectList;
        }
    }
}
