using AutoMapper;
using CatLog.Api.Data.Interfaces;
using CatLog.Api.DtoParameters;
using CatLog.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using CatLog.Api.Helpers;
using CatLog.Api.Services.Interfaces;
using CatLog.Api.Entities;

namespace CatLog.Api.Controllers
{
    [Route("api/articles")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleDao _articleDao;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;

        public ArticlesController(IArticleDao articleDao, IMapper mapper, IPropertyMappingService propertyMappingService)
        {
            _articleDao = articleDao ?? throw new ArgumentNullException(nameof(articleDao));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        }

        #region HttpGet

        [HttpGet(Name = nameof(GetArticles))]
        public async Task<IActionResult> GetArticles([FromQuery]ArticleDtoParameters parameters)
        {
            // 无需先判断字符串是否为 Null, ValidMappingExistsFor 对 Null 值返回 true
            if (!_propertyMappingService.ValidMappingExistsFor<ArticleDto, Article>(parameters.OrderBy))
            {
                return BadRequest();
            }
            if (!_propertyMappingService.ValidMappingExistsFor<ArticleDto, Article>(parameters.Select))
            {
                return BadRequest();
            }

            var pagedArticles = await _articleDao.GetArticlesAsync(parameters);

            //向 Headers 中添加翻页信息
            var paginationMetdata = new
            {
                totalCount = pagedArticles.TotalCount,
                pageSize = pagedArticles.PageSize,
                currentPage = pagedArticles.PageNumber,
                totalPages = pagedArticles.TotalPages
            };
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetdata,
                                                                          new JsonSerializerOptions
                                                                          {   //为了防止 URI 中的‘&’、‘？’符号被转义，使用“不安全”的 Encoder
                                                                              Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                                                                          }));

            var returnDto = _mapper.Map<IEnumerable<ArticleDto>>(pagedArticles);

            /*
             * 在 ArticleDao 中已经对 select 进行了一次处理，未被 select 中的属性为默认值（null、0 等）
             * 如果想对返回的数据进行塑形，可以启用以下方法
             * 该方法使 API 更加规范，并且节约网络流量，但可能会略微降低性能，请按需启用。
             */
            return Ok(returnDto.ShapeData(parameters.Select));
            /*
             */

            return Ok(returnDto);
        }

        [HttpGet("{articleId}", Name = nameof(GetArticle))]
        public async Task<IActionResult> GetArticle([FromRoute]long articleId)
        {
            if(!await _articleDao.ArticleExistsAsync(articleId))
            {
                return NotFound();
            }
            var article = await _articleDao.GetArticleAsync(articleId);
            return Ok(article);
        }

        #endregion HttpGet

        #region HttpDelete
        [HttpDelete("{articleId}", Name = nameof(DeleteArticle))]
        public async Task<IActionResult> DeleteArticle([FromRoute]long articleId)
        {
            var article = await _articleDao.GetArticleAsync(articleId);
            if (article == null)
            {
                return NotFound();
            }
            _articleDao.RemoveArticle(article);
            await _articleDao.SaveAsync();
            return NoContent();
        }
        #endregion HttpDelete

    }
}
