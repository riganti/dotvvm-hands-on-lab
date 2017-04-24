using System.ComponentModel.DataAnnotations;

namespace DotvvmBlog.BL.DTO
{
    public class SignInDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}