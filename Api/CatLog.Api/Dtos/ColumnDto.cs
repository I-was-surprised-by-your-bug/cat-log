namespace CatLog.Api.Dtos
{
    public class SectionDto
    {
        /// <summary>
        /// 栏目 ID
        /// </summary>
        public long Id { get; set; }
        
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
