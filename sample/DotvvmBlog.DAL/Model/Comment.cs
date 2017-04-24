using System;
using System.ComponentModel.DataAnnotations;

namespace DotvvmBlog.DAL.Model
{
    public class Comment
    {

        public int Id { get; set; }

        public int ArticleId { get; set; }

        public virtual Article Article { get; set; }

        public string Html { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(100)]
        public string IpAddress { get; set; }

    }
}