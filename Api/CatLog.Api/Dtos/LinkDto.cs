namespace CatLog.Api.Dtos
{
    /// <summary>
    /// HATEOAS 的 link Dto
    /// </summary>
    public class LinkDto
    {
        public string Uri { get; }
        public string Rel { get; }
        public string Method { get; }
        public LinkDto(string uri, string rel, string method)
        {
            Uri = uri;
            Rel = rel;
            Method = method;
        }
    }
}
