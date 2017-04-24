using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotvvmBlog.DAL.Model;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DotvvmBlog.DAL
{
    public class BlogDbContext : IdentityDbContext
    {

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Article> Articles { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<ArticleTag> ArticleTags { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<BlogAuthor> BlogAuthors { get; set; }


        public BlogDbContext() : base("DB")
        {
            
        }
    }
}
