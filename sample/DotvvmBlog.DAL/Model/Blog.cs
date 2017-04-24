using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;

namespace DotvvmBlog.DAL.Model
{
    public class Blog
    {

        public int Id { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(100)]
        public string Subtitle { get; set; }

        [StringLength(100)]
        public string AuthorName { get; set; }

        public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

        public virtual ICollection<BlogAuthor> BlogAuthors { get; set; } = new List<BlogAuthor>();

    }
}