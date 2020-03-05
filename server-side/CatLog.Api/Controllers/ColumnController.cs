using AutoMapper;
using CatLog.Api.Data.Interfaces;
using CatLog.Api.Data.Models;
using CatLog.Api.DtoParameters;
using CatLog.Api.Dtos;
using CatLog.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet(Name = nameof(GetColumns))]
        public async Task<IActionResult> GetColumns([FromRoute] long sectionId, [FromQuery]ColumnDtoParameters parameters)
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

            var pagedColumns = await _columnDao.GetColumnsAsync(sectionId, parameters);

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

            var returnDtos = _mapper.Map<IEnumerable<ColumnDto>>(pagedColumns);
            return Ok(returnDtos);
        }

        [HttpGet("{columnId}", Name = nameof(GetColumn))]
        public async Task<IActionResult> GetColumn([FromRoute]long sectionId, [FromRoute]long columnId)
        {
            var column = await _columnDao.GetColumnAsync(sectionId, columnId);
            if (column is null)
            {
                return NotFound();
            }
            var returnDto = _mapper.Map<ColumnDto>(column);
            return Ok(returnDto);
        }

        #endregion HttpGet

        #region HttpDelete
        [HttpDelete("{columnId}", Name = nameof(DeleteColumn))]
        public async Task<IActionResult> DeleteColumn([FromRoute]long sectionId, [FromRoute]long columnId)
        {
            var column = await _columnDao.GetColumnAsync(sectionId, columnId);
            if (column is null)
            {
                return NotFound();
            }
            _columnDao.RemoveColumn(column);
            await _columnDao.SaveAsync();
            return NoContent();
        }
        #endregion HttpDelete

    }
}
