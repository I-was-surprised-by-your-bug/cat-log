using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;

namespace CatLog.Api.Helpers
{
    public static class StringExtensions
    {
        /// <summary>
        /// 判断客户端是否主动接受 HATEOAS 媒体类型
        /// </summary>
        /// <param name="mediaTypeStr">MediaType 字符串</param>
        /// <returns>客户端是否接受 HATEOAS 媒体类型</returns>
        public static bool AcceptHateoasMediaType(this string mediaTypeStr)
        {
            var mediaTypes = mediaTypeStr.Split(',');
            bool mediaTypeParseSucceed = MediaTypeHeaderValue.TryParseList(mediaTypes, out IList<MediaTypeHeaderValue> parsedValues);
            if (mediaTypeParseSucceed)
            {
                foreach (var value in parsedValues)
                {
                    if (value.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase)) // 大小写不敏感
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
