using System;
using System.Collections.Generic;
using DotVVM.Framework.ViewModel;

namespace DotvvmBlog.BL.DTO
{
    public class ArticleDetailDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }
        
        //[Bind(Direction.None)]
        public string Html { get; set; }

        public DateTime? PublishedDate { get; set; }

        public string AuthorName { get; set; }

        public IEnumerable<string> Tags { get; set; }
        
        public string BlogName { get; set; }
    }
}