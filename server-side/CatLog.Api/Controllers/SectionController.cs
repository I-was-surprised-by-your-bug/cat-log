using AutoMapper;
using CatLog.Api.Data.Interfaces;
using CatLog.Api.Data.Models;
using CatLog.Api.DtoParameters;
using CatLog.Api.Dtos;
using CatLog.Api.Helpers;
using CatLog.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace CatLog.Api.Controllers
{
    [Route("api/sections")]
    [ApiController]
    public class SectionController : ControllerBase
    {
        private readonly ISectionRepository _sectionDao;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;

        public SectionController(ISectionRepository sectionDao, IMapper mapper, IPropertyMappingService propertyMappingService)
        {
            _sectionDao = sectionDao ?? throw new ArgumentNullException(nameof(sectionDao));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        }

        #region HttpGet

        [HttpGet(Name = nameof(GetSections))]
        public async Task<IActionResult> GetSections([FromQuery]SectionDtoParameters parameters,
                                                     [FromHeader(Name = "Accept")] string mediaTypeStr)
        {
            // 无需先判断字符串是否为 Null, ValidMappingExistsFor 对 Null 值返回 true
            if (!_propertyMappingService.ValidMappingExistsFor<SectionDto, Section>(parameters.OrderBy))
            {
                return BadRequest();
            }

            var pagedSections = await _sectionDao.GetSectionsAsync(parameters);

            //向 Headers 中添加翻页信息
            var paginationMetdata = new
            {
                totalCount = pagedSections.TotalCount,
                pageSize = pagedSections.PageSize,
                currentPage = pagedSections.PageNumber,
                totalPages = pagedSections.TotalPages
            };
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetdata,
                                                                          new JsonSerializerOptions
                                                                          {   //为了防止 URI 中的‘&’、‘？’符号被转义，使用“不安全”的 Encoder
                                                                              Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                                                                          }));

            var sectionDtos = _mapper.Map<IEnumerable<SectionDto>>(pagedSections);

            // 是否要求 HATEOAS 规范，在返回值中添加 links
            if (mediaTypeStr.AcceptHateoasMediaType())
            {
                // 首先将 Sections 列表中的每一个 Section 转为键值对形式，并添加 links 键值对。
                var sectionDtosWithLinks = new List<IDictionary<string, Object>>();
                foreach (var dto in sectionDtos)
                {
                    var dtoDic = dto.ToKeyValuePairs();
                    dtoDic.Add("links", CreateLinksForSection(dto.Id));
                    sectionDtosWithLinks.Add(dtoDic);
                }
                // 新建容器，将上一步整理好的 Section 列表作为 value 放入容器，再为该容器添加 links。
                var linkedCollectionResource = new
                {
                    value = sectionDtosWithLinks,
                    links = CreateLinksForSections(parameters, pagedSections.HasPrevious, pagedSections.HasNext)
                };
                return Ok(linkedCollectionResource);
            }

            return Ok(sectionDtos);
        }

        [HttpGet("{sectionId}", Name = nameof(GetSection))]
        public async Task<IActionResult> GetSection([FromRoute]long sectionId,
                                                    [FromHeader(Name = "Accept")] string mediaTypeStr)
        {
            var section = await _sectionDao.GetSectionAsync(sectionId);
            if (section is null)
            {
                return NotFound();
            }
            var sectionDto = _mapper.Map<SectionDto>(section);

            // 是否要求 HATEOAS 规范，在返回值中添加 links
            if (mediaTypeStr.AcceptHateoasMediaType())
            {
                var sectionDtoDic = sectionDto.ToKeyValuePairs();
                sectionDtoDic.Add("links", CreateLinksForSection(sectionId));
                return Ok(sectionDtoDic);
            }

            return Ok(sectionDto);
        }

        #endregion HttpGet

        #region HttpDelete
        [HttpDelete("{sectionId}", Name = nameof(DeleteSection))]
        public async Task<IActionResult> DeleteSection([FromRoute]long sectionId)
        {
            var section = await _sectionDao.GetSectionAsync(sectionId);
            if (section == null)
            {
                return NotFound();
            }
            _sectionDao.RemoveSection(section);
            await _sectionDao.SaveAsync();
            return NoContent();
        }
        #endregion HttpDelete

        #region Helpers

        /// <summary>
        /// 生成 Section 列表上一页、下一页或当前页的 Uri
        /// </summary>
        /// <param name="parameters">SectionDtoParameters</param>
        /// <param name="uriType">ResourceUriType</param>
        /// <returns>跳转到目标页的 Uri</returns>
        private string CreateSectionsResourceUri(SectionDtoParameters parameters,
                                                ResourceUriType uriType)
        {
            switch (uriType)
            {
                case ResourceUriType.PreviousPage: //上一页
                    return Url.Link(
                        nameof(GetSections), //方法名
                        new                   //Uri Query 字符串参数
                        {
                            pageNumber = parameters.PageNumber - 1,
                            pageSize = parameters.PageSize,
                            orderBy = parameters.OrderBy
                        });
                case ResourceUriType.NextPage: //下一页
                    return Url.Link(
                        nameof(GetSections),
                        new
                        {
                            pageNumber = parameters.PageNumber + 1,
                            pageSize = parameters.PageSize,
                            orderBy = parameters.OrderBy
                        });
                //case ResourceUriType.CurrentPage: //当前页
                default:
                    return Url.Link(
                        nameof(GetSections),
                        new
                        {
                            pageNumber = parameters.PageNumber,
                            pageSize = parameters.PageSize,
                            orderBy = parameters.OrderBy
                        });
            }
        }

        /// <summary>
        /// 为 Section 单个资源创建 HATEOAS 的 links
        /// </summary>
        /// <param name="sectionId">Section Id</param>
        /// <returns>Section 单个资源的 links</returns>
        private IEnumerable<LinkDto> CreateLinksForSection(long sectionId)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(Url.Link(nameof(GetSection), new { sectionId }),
                                  "self",
                                  "GET"));
            links.Add(new LinkDto(Url.Link(nameof(DeleteSection), new { sectionId }),
                                  "delete_section",
                                  "DELETE"));
            links.Add(new LinkDto(Url.Link(nameof(ColumnController.GetColumnsForSection), new { sectionId }),
                                  "columns",
                                  "GET"));
            return links;
        }

        /// <summary>
        /// 为 Section 集合资源创建 HATEOAS 的 links
        /// </summary>
        /// <param name="parameters">SectionDtoParameters</param>
        /// <param name="hasPrevious">是否有上一页</param>
        /// <param name="hasNext">是否有下一页</param>
        /// <returns>Section 集合资源的 links</returns>
        private IEnumerable<LinkDto> CreateLinksForSections(SectionDtoParameters parameters, bool hasPrevious, bool hasNext)
        {
            var links = new List<LinkDto>();
            // 当前页链接
            links.Add(new LinkDto(CreateSectionsResourceUri(parameters, ResourceUriType.CurrentPage),
                      "self",
                      "GET"));
            if (hasPrevious)
            {
                // 上一页链接
                links.Add(new LinkDto(CreateSectionsResourceUri(parameters, ResourceUriType.PreviousPage),
                          "previous_page",
                          "GET"));
            }
            if (hasNext)
            {
                // 下一页链接
                links.Add(new LinkDto(CreateSectionsResourceUri(parameters, ResourceUriType.NextPage),
                          "next_page",
                          "GET"));
            }
            return links;
        }
        #endregion Helpers
    }
}
