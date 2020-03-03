using CatLog.Api.Services.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace CatLog.Api.Helpers
{
    /// <summary>
    /// IQueryable<T> 的拓展方法
    /// </summary>
    public static class IQueryableExtensions
    {
        /// <summary>
        /// 对集合资源进行排序
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="source">IQueryable 资源集合</param>
        /// <param name="orderBy">Uri Query 中的 orderBy 字符串，用于声明资源集合的排序规则；如果无需排序传入 null 即可</param>
        /// <param name="mappingDictionary">资源类型的映射关系字典</param>
        /// <returns>排好序的 IQueryable 资源集合</returns>
        public static System.Linq.IQueryable<T> ApplyOrderBy<T>(this System.Linq.IQueryable<T> source,
                                                                string orderBy,
                                                                Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (mappingDictionary == null)
            {
                throw new ArgumentNullException(nameof(mappingDictionary));
            }
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            //空字符串用来储存 order by T-SQL 命令
            string ordering = "";

            //将 orderBy 字符串按 ',' 划分为数组
            var orderByAfterSplit = orderBy.Split(",");
            //依次处理数组中的每个排序依据
            foreach (var orderByClause in orderByAfterSplit)
            {
                var trimmedOrderByClause = orderByClause.Trim();
                //是否 DESC
                var orderDescending = trimmedOrderByClause.EndsWith(" desc");
                //第一个空格的 index
                var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ");
                //属性名
                //如果存在空格，移除空格后面的内容（用来移除" desc"）
                var propertyName = indexOfFirstSpace == -1 ? trimmedOrderByClause : trimmedOrderByClause.Remove(indexOfFirstSpace);

                //在属性映射字典中查找
                //属性映射字典的 Key 大小写不敏感，不用担心大小写问题
                if (!mappingDictionary.ContainsKey(propertyName))
                {
                    throw new ArgumentNullException($"没有找到Key为{propertyName}的映射");
                }

                var propertyMappingValue = mappingDictionary[propertyName];
                if (propertyMappingValue == null)
                {
                    throw new ArgumentNullException(nameof(propertyMappingValue));
                }

                foreach (var destinationProperty in propertyMappingValue.DestinationProperties)
                {
                    if (propertyMappingValue.Revert)
                    {
                        orderDescending = !orderDescending;
                    }

                    if (ordering.Length > 0)
                    {
                        ordering += ",";
                    }
                    ordering += destinationProperty + (orderDescending ? " descending" : " ascending");
                }
            }

            //执行 order by T-SQL 命令
            //需要安装 System.Linq.Dynamic.Core 包，才能使用以下代码
            source = source.OrderBy<T>(ordering);
            return source;
        }

        /// <summary>
        /// 对集合资源进行塑形
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="source">IQueryable 资源集合</param>
        /// <param name="orderBy">Uri Query 中的 orderBy 字符串，用于声明资源集合的排序规则；如果无需排序传入 null 即可</param>
        /// <param name="mappingDictionary">资源类型的映射关系字典</param>
        /// <returns>排好序的 IQueryable 资源集合</returns>
        public static System.Linq.IQueryable<T> ApplySelect<T>(this System.Linq.IQueryable<T> source,
                                                               string select,
                                                               Dictionary<string, PropertyMappingValue> mappingDictionary)
        {

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (mappingDictionary == null)
            {
                throw new ArgumentNullException(nameof(mappingDictionary));
            }
            if (string.IsNullOrWhiteSpace(select))
            {
                return source;
            }

            //空字符串用来储存 select T-SQL 命令
            string selector = "new(";

            //将 select 字符串按 ',' 划分为数组
            var selectAfterSplit = select.Split(",");
            //依次处理数组中的每个排序依据
            foreach (var orderByClause in selectAfterSplit)
            {
                var trimmedSelectClause = orderByClause.Trim();

                //在属性映射字典中查找
                //属性映射字典的 Key 大小写不敏感，不用担心大小写问题
                if (!mappingDictionary.ContainsKey(trimmedSelectClause))
                {
                    throw new ArgumentNullException($"没有找到Key为{trimmedSelectClause}的映射");
                }

                var propertyMappingValue = mappingDictionary[trimmedSelectClause];
                if (propertyMappingValue == null)
                {
                    throw new ArgumentNullException(nameof(propertyMappingValue));
                }

                foreach (var destinationProperty in propertyMappingValue.DestinationProperties)
                {
                    if (selector.Length > 4)
                    {
                        selector += ",";
                    }
                    selector += destinationProperty;
                }
            }
            selector += ")";

            //执行 order by T-SQL 命令
            //需要安装 System.Linq.Dynamic.Core 包，才能使用以下代码
            source = source.Select<T>(selector);
            return source;
        }
    }
}
