using System;
using System.Collections.Generic;

namespace CatLog.Api.Data.Models
{
    public partial class Column
    {
        public Column()
        {
            TArticles = new HashSet<Article>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long SectionId { get; set; }

        public virtual Section Section { get; set; }
        public virtual ICollection<Article> TArticles { get; set; }
    }
}
