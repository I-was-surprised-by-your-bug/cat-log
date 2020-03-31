using System;
using System.Collections.Generic;

namespace CatLog.Api.Data.Models
{
    public partial class Section
    {
        public Section()
        {
            TColumns = new HashSet<Column>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Column> TColumns { get; set; }
    }
}
