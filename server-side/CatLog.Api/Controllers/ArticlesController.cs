using AutoMapper;
using CatLog.Api.Data.Interfaces;
using CatLog.Api.DtoParameters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace CatLog.Api.Controllers
{
    [Route("api/articles")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleDao _articleDao;
        private readonly IMapper _mapper;

        public ArticlesController(IArticleDao articleDao, IMapper mapper)
        {
            _articleDao = articleDao ?? throw new ArgumentNullException(nameof(articleDao));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #region HttpGet

        [HttpGet(Name = nameof(GetArticles))]
        public async Task<IActionResult> GetArticles([FromQuery]ArticleDtoParameters parameters)
        {
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

            return Ok(pagedArticles);
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
    }
}
