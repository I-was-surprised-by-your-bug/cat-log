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
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace CatLog.Api.Controllers
{
    [Route("api/sections/{sectionId}/columns/{columnId}/articles")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleRepository _articleDao;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMappingService;

        public ArticlesController(IArticleRepository articleDao, IMapper mapper, IPropertyMappingService propertyMappingService)
        {
            _articleDao = articleDao ?? throw new ArgumentNullException(nameof(articleDao));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        }

        #region HttpGet

        /// <summary>
        /// 获得 Column 的所有 Articles
        /// </summary>
        /// <param name="sectionId">Section ID</param>
        /// <param name="columnId">Column ID</param>
        /// <param name="parameters">Uri parameters</param>
        /// <param name="mediaTypeStr">Accept media type 字符串</param>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetArticlesForColumn))]
        public async Task<IActionResult> GetArticlesForColumn([FromRoute]long sectionId,
                                                              [FromRoute]long columnId,
                                                              [FromQuery]ArticleDtoParameters parameters,
                                                              [FromHeader(Name = "Accept")] string mediaTypeStr)
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
            if (!await _articleDao.SectionExistsAsync(sectionId))
            {
                return NotFound();
            }

            var pagedArticles = await _articleDao.GetArticlesForColumnAsync(columnId, parameters);

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

            var articleDtos = _mapper.Map<IEnumerable<ArticleDto>>(pagedArticles);
            // 数据塑形
            var shapedArticles = articleDtos.ShapeData(parameters.Select);

            // 是否要求 HATEOAS 规范，在返回值中添加 links
            if (mediaTypeStr.AcceptHateoasMediaType())
            {
                var shapedArticlesWithLinks = shapedArticles.Select(x =>
                {
                    var articleDic = x as IDictionary<string, object>;
                    articleDic.Add("links", CreateLinksForArticle(sectionId, columnId, (long)articleDic["Id"]));
                    return articleDic;
                });
                // 新建容器，将上一步整理好的 Article 列表作为 value 放入容器，再为该容器添加 links。
                var linkedCollectionResource = new
                {
                    value = shapedArticlesWithLinks,
                    links = CreateLinksForArticles(sectionId, columnId, parameters, pagedArticles.HasPrevious, pagedArticles.HasNext)
                };
                return Ok(linkedCollectionResource);
            }

            return Ok(shapedArticles);
        }

        /// <summary>
        /// 获得 Column 的一篇 Articlea
        /// </summary>
        /// <param name="sectionId">Section ID</param>
        /// <param name="columnId">Column ID</param>
        /// <param name="articleId">Article ID</param>
        /// <param name="mediaTypeStr">Accept media type 字符串</param>
        /// <returns></returns>
        [HttpGet("{articleId}", Name = nameof(GetArticleForColumn))]
        public async Task<IActionResult> GetArticleForColumn([FromRoute]long sectionId,
                                                             [FromRoute]long columnId,
                                                             [FromRoute]long articleId,
                                                             [FromHeader(Name = "Accept")] string mediaTypeStr)
        {
            if (!await _articleDao.SectionExistsAsync(sectionId))
            {
                return NotFound();
            }
            var article = await _articleDao.GetArticleForColumnAsync(columnId, articleId);
            if (article is null)
            {
                return NotFound();
            }
            var articleDto = _mapper.Map<ArticleDto>(article);

            // 是否要求 HATEOAS 规范，在返回值中添加 links
            if (mediaTypeStr.AcceptHateoasMediaType())
            {
                var articleDtoDic = articleDto.ToKeyValuePairs();
                articleDtoDic.Add("links", CreateLinksForArticle(sectionId, columnId, articleId));
                return Ok(articleDtoDic);
            }

            return Ok(articleDto);
        }

        #endregion HttpGet

        #region Helpers
        /// <summary>
        /// 生成 Article 列表上一页、下一页或当前页的 Uri
        /// </summary>
        /// <param name="sectionId">Section Id</param>
        /// <param name="columnId">Column Id</param>
        /// <param name="parameters">ArticleDtoParameters</param>
        /// <param name="uriType">ResourceUriType</param>
        /// <returns></returns>
        private string CreateArticlesResourceUri(long sectionId, long columnId,
                                                 ArticleDtoParameters parameters,
                                                 ResourceUriType uriType)
        {
            switch (uriType)
            {
                case ResourceUriType.PreviousPage: //上一页
                    return Url.Link(
                        nameof(GetArticlesForColumn),
                        new
                        {
                            sectionId,
                            columnId,
                            pageNumber = parameters.PageNumber - 1,
                            pageSize = parameters.PageSize,
                            orderBy = parameters.OrderBy,
                            select = parameters.Select
                        });
                case ResourceUriType.NextPage: //下一页
                    return Url.Link(
                        nameof(GetArticlesForColumn),
                        new
                        {
                            sectionId,
                            columnId,
                            pageNumber = parameters.PageNumber + 1,
                            pageSize = parameters.PageSize,
                            orderBy = parameters.OrderBy,
                            select = parameters.Select
                        });
                //case ResourceUriType.CurrentPage: //当前页
                default:
                    return Url.Link(
                        nameof(GetArticlesForColumn),
                        new
                        {
                            sectionId,
                            columnId,
                            pageNumber = parameters.PageNumber,
                            pageSize = parameters.PageSize,
                            orderBy = parameters.OrderBy,
                            select = parameters.Select
                        });
            }
        }

        /// <summary>
        /// 为 Article 单个资源创建 HATEOAS 的 links
        /// </summary>
        /// <param name="sectionId">Section Id</param>
        /// <param name="columnId">Column Id</param>
        /// <param name="articleId">Article Id</param>
        /// <returns>Article 单个资源的 links</returns>
        private IEnumerable<LinkDto> CreateLinksForArticle(long sectionId, long columnId, long articleId)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(Url.Link(nameof(GetArticleForColumn), new { sectionId, columnId, articleId }),
                                  "self",
                                  "GET"));
            return links;
        }

        /// <summary>
        /// 为 Article 集合资源创建 HATEOAS 的 links
        /// </summary>
        /// <param name="parameters">ArticleDtoParameters</param>
        /// <param name="hasPrevious">是否有上一页</param>
        /// <param name="hasNext">是否有下一页</param>
        /// <returns>Article 集合资源的 links</returns>
        private IEnumerable<LinkDto> CreateLinksForArticles(long sectionId,
                                                           long columnId,
                                                           ArticleDtoParameters parameters,
                                                           bool hasPrevious,
                                                           bool hasNext)
        {
            var links = new List<LinkDto>();
            // 当前页链接
            links.Add(new LinkDto(CreateArticlesResourceUri(sectionId, columnId, parameters, ResourceUriType.CurrentPage),
                      "self",
                      "GET"));
            if (hasPrevious)
            {
                // 上一页链接
                links.Add(new LinkDto(CreateArticlesResourceUri(sectionId, columnId, parameters, ResourceUriType.PreviousPage),
                          "previous_page",
                          "GET"));
            }
            if (hasNext)
            {
                // 下一页链接
                links.Add(new LinkDto(CreateArticlesResourceUri(sectionId, columnId, parameters, ResourceUriType.NextPage),
                          "next_page",
                          "GET"));
            }
            return links;
        }
        #endregion helpers
    }
}
