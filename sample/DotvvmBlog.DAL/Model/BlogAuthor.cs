using Microsoft.AspNet.Identity.EntityFramework;

namespace DotvvmBlog.DAL.Model
{
    public class BlogAuthor
    {

        public int Id { get; set; }

        public int BlogId { get; set; }

        public virtual Blog Blog { get; set; }

        public string IdentityUserId { get; set; }

        public virtual IdentityUser IdentityUser { get; set; }

    }
}