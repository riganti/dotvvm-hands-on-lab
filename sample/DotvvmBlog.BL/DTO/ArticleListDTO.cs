using System;
using System.Collections.Generic;

namespace DotvvmBlog.BL.DTO
{
    public class ArticleListDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string AuthorName { get; set; }
    }
}