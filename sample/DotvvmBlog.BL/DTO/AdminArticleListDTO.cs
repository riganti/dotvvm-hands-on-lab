using System;

namespace DotvvmBlog.BL.DTO
{
    public class AdminArticleListDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string BlogName { get; set; }
    }
}