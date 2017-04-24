using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotvvmBlog.DAL.Model
{
    public class Article
    {

        public int Id { get; set; }

        public int BlogId { get; set; }

        public virtual Blog Blog { get; set; }

        public DateTime? PublishedDate { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        public string Html { get; set; }

        public virtual ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    }
}