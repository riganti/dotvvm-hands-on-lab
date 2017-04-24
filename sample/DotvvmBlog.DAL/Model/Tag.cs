using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotvvmBlog.DAL.Model
{
    public class Tag
    {

        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public virtual ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();

    }
}