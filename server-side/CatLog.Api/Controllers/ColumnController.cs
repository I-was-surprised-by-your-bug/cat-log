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
    [ApiController]
    [Route("api/sections/{sectionId}/columns")]
    public class ColumnController : ControllerBase
    {
        private readonly IColumnRepository _columnDao;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;

        public ColumnController(IColumnRepository columnDao, IMapper mapper, IPropertyMappingService propertyMappingService)
        {
            _columnDao = columnDao ?? throw new ArgumentNullException(nameof(columnDao));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        }

        #region HttpGet

        [HttpGet(Name = nameof(GetColumnsForSection))]
        public async Task<IActionResult> GetColumnsForSection([FromHeader(Name = "Accept")] string mediaType,
                                                              [FromRoute] long sectionId,
                                                              [FromQuery]ColumnDtoParameters parameters)
        {
            // 无需先判断字符串是否为 Null, ValidMappingExistsFor 对 Null 值返回 true
            if (!_propertyMappingService.ValidMappingExistsFor<ColumnDto, Column>(parameters.OrderBy))
            {
                return BadRequest();
            }
            if (!await _columnDao.SectionExistsAsync(sectionId))
            {
                return NotFound();
            }

            var pagedColumns = await _columnDao.GetColumnsForSectionAsync(sectionId, parameters);

            //向 Headers 中添加翻页信息
            var paginationMetdata = new
            {
                totalCount = pagedColumns.TotalCount,
                pageSize = pagedColumns.PageSize,
                currentPage = pagedColumns.PageNumber,
                totalPages = pagedColumns.TotalPages
            };
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetdata,
                                                                          new JsonSerializerOptions
                                                                          {   //为了防止 URI 中的‘&’、‘？’符号被转义，使用“不安全”的 Encoder
                                                                              Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                                                                          }));

            var columnDtos = _mapper.Map<IEnumerable<ColumnDto>>(pagedColumns);

            // 是否要求 HATEOAS 规范，在返回值中添加 links
            bool mediaTypeParsed = MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType);
            bool isHateoas = mediaTypeParsed ? parsedMediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase) //大小写不敏感
                                              : false;
            if (isHateoas)
            {
                // 首先将 Columns 列表中的每一个 Column 转为键值对形式，并添加 links 键值对。
                var columnDtosWithLinks = new List<IDictionary<string, Object>>();
                foreach (var dto in columnDtos)
                {
                    var dtoDic = dto.ToKeyValuePairs();
                    dtoDic.Add("links", CreateLinksForColumn(sectionId, dto.Id));
                    columnDtosWithLinks.Add(dtoDic);
                }
                // 新建容器 columnDtosDics，将上一步整理好的 Columns 列表作为 value 放入容器，再为该容器添加 links。
                var sectionDtosDics = new Dictionary<string, Object>();
                sectionDtosDics.Add("value", columnDtosWithLinks);
                sectionDtosDics.Add("links", CreateLinksForColumns(sectionId, parameters, pagedColumns.HasPrevious, pagedColumns.HasNext));
                return Ok(sectionDtosDics);
            }

            return Ok(columnDtos);
        }

        [HttpGet("{columnId}", Name = nameof(GetColumnForSection))]
        public async Task<IActionResult> GetColumnForSection([FromHeader(Name = "Accept")] string mediaType,
                                                             [FromRoute]long sectionId,
                                                             [FromRoute]long columnId)
        {
            var column = await _columnDao.GetColumnForSectionAsync(sectionId, columnId);
            if (column is null)
            {
                return NotFound();
            }
            var columnDto = _mapper.Map<ColumnDto>(column);

            // 是否要求 HATEOAS 规范，在返回值中添加 links
            bool mediaTypeParsed = MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue parsedMediaType);
            bool isHateoas = mediaTypeParsed ? parsedMediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase) //大小写不敏感
                                              : false;
            if (isHateoas)
            {
                var columnDtoDic = columnDto.ToKeyValuePairs();
                columnDtoDic.Add("links", CreateLinksForColumn(sectionId, columnId));
                return Ok(columnDtoDic);
            }

            return Ok(columnDto);
        }
        #endregion HttpGet

        #region HttpDelete
        [HttpDelete("{columnId}", Name = nameof(DeleteColumnForSection))]
        public async Task<IActionResult> DeleteColumnForSection([FromRoute]long sectionId, [FromRoute]long columnId)
        {
            var column = await _columnDao.GetColumnForSectionAsync(sectionId, columnId);
            if (column is null)
            {
                return NotFound();
            }
            _columnDao.RemoveColumn(column);
            await _columnDao.SaveAsync();
            return NoContent();
        }
        #endregion HttpDelete

        #region Helpers
        /// <summary>
        /// 生成 Column 列表上一页、下一页或当前页的 Uri
        /// </summary>
        /// <param name="parameters">ColumnDtoParameters</param>
        /// <param name="uriType">ResourceUriType</param>
        /// <returns>跳转到目标页的 Uri</returns>
        private string CreateColumnsResourceUri(long sectionId,
                                                ColumnDtoParameters parameters,
                                                ResourceUriType uriType)
        {
            switch (uriType)
            {
                case ResourceUriType.PreviousPage: //上一页
                    return Url.Link(
                        nameof(GetColumnsForSection),
                        new
                        {
                            sectionId,
                            pageNumber = parameters.PageNumber - 1,
                            pageSize = parameters.PageSize,
                            orderBy = parameters.OrderBy
                        });
                case ResourceUriType.NextPage: //下一页
                    return Url.Link(
                        nameof(GetColumnsForSection),
                        new
                        {
                            sectionId,
                            pageNumber = parameters.PageNumber + 1,
                            pageSize = parameters.PageSize,
                            orderBy = parameters.OrderBy
                        });
                //case ResourceUriType.CurrentPage: //当前页
                default:
                    return Url.Link(
                        nameof(GetColumnsForSection),
                        new
                        {
                            sectionId,
                            pageNumber = parameters.PageNumber,
                            pageSize = parameters.PageSize,
                            orderBy = parameters.OrderBy
                        });
            }
        }

        /// <summary>
        /// 为 Column 单个资源创建 HATEOAS 的 links
        /// </summary>
        /// <param name="sectionId">Section Id</param>
        /// <param name="columnId">Column Id</param>
        /// <returns>Column 单个资源的 links</returns>
        private IEnumerable<LinkDto> CreateLinksForColumn(long sectionId, long columnId)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(Url.Link(nameof(GetColumnForSection), new { sectionId, columnId }),
                                  "self",
                                  "GET"));
            links.Add(new LinkDto(Url.Link(nameof(DeleteColumnForSection), new { sectionId, columnId }),
                                  "delete_column_for_section",
                                  "DELETE"));
            return links;
        }

        /// <summary>
        /// 为 Column 集合资源创建 HATEOAS 的 links
        /// </summary>
        /// <param name="parameters">ColumnDtoParameters</param>
        /// <param name="hasPrevious">是否有上一页</param>
        /// <param name="hasNext">是否有下一页</param>
        /// <returns>Column 集合资源的 links</returns>
        private IEnumerable<LinkDto> CreateLinksForColumns(long sectionId, ColumnDtoParameters parameters, bool hasPrevious, bool hasNext)
        {
            var links = new List<LinkDto>();
            // 当前页链接
            links.Add(new LinkDto(CreateColumnsResourceUri(sectionId, parameters, ResourceUriType.CurrentPage),
                      "self",
                      "GET"));
            if (hasPrevious)
            {
                // 上一页链接
                links.Add(new LinkDto(CreateColumnsResourceUri(sectionId, parameters, ResourceUriType.PreviousPage),
                          "previous_page",
                          "GET"));
            }
            if (hasNext)
            {
                // 下一页链接
                links.Add(new LinkDto(CreateColumnsResourceUri(sectionId, parameters, ResourceUriType.NextPage),
                          "next_page",
                          "GET"));
            }
            return links;
        }
        #endregion helpers
    }
}
