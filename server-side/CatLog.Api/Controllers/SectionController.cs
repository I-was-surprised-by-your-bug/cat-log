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
        public async Task<IActionResult> GetSections([FromQuery]SectionDtoParameters parameters)
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

            var returnDtos = _mapper.Map<IEnumerable<SectionDto>>(pagedSections);
            return Ok(returnDtos);
        }

        [HttpGet("{sectionId}", Name = nameof(GetSection))]
        public async Task<IActionResult> GetSection([FromRoute]long sectionId)
        {
            if(!await _sectionDao.SectionExistsAsync(sectionId))
            {
                return NotFound();
            }
            var section = await _sectionDao.GetSectionAsync(sectionId);
            var returnDto = _mapper.Map<SectionDto>(section);
            return Ok(returnDto);
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

    }
}
