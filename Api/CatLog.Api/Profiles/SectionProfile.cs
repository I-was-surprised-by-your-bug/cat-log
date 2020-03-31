using AutoMapper;
using CatLog.Api.Data.Models;
using CatLog.Api.Dtos;

namespace CatLog.Api.Profiles
{
    /// <summary>
    /// AutoMapper 针对 Section 的映射关系
    /// </summary>
    public class SectionProfile : Profile
    {
        public SectionProfile()
        {
            /*
             * CreateMap<源类型,目标类型>();
             * 属性名称一致时自动赋值
             * 自动忽略空引用
            */
            CreateMap<Section, SectionDto>();
        }
    }
}
