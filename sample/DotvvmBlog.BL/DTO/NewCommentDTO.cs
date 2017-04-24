using System.ComponentModel.DataAnnotations;

namespace DotvvmBlog.BL.DTO
{
    public class NewCommentDTO
    {
        [Required]
        public string Text { get; set; }

        public string IpAddress { get; set; }

    }
}