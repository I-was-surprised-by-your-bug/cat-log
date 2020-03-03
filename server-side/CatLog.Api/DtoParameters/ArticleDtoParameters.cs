namespace CatLog.Api.DtoParameters
{
    public class ArticleDtoParameters
    {
        private const int MaxPageSize = 20;

        private int _pageSize = 5;

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// 数据塑形参数
        /// </summary>
        public string Select { get; set; }

        /// <summary>
        /// 排序参数
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// 单页条目数
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize ? MaxPageSize : value);
        }
    }
}
