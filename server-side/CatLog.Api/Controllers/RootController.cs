using CatLog.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CatLog.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        #region HttpGet

        [HttpGet(Name = nameof(GetRoot))]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDto>();
            links.Add(new LinkDto(Url.Link(nameof(GetRoot), new { }),
                      "self",
                      "GET"));
            links.Add(new LinkDto(Url.Link(nameof(SectionController.GetSections), new { }),
                      "sections",
                      "GET"));
            links.Add(new LinkDto(Url.Link(nameof(AllArticlesController.GetArticles), new { }),
                      "articles",
                      "GET"));
            var response = new
            {
                links = links
            };
            return Ok(response);
        }

        #endregion HttpGet
    }
}
