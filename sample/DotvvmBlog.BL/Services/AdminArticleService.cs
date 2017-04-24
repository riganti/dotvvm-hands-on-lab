using System;
using System.Linq;
using System.Security.Claims;
using DotvvmBlog.BL.DTO;
using DotvvmBlog.DAL;
using DotvvmBlog.DAL.Model;
using DotVVM.Framework.Controls;

namespace DotvvmBlog.BL.Services
{
    public class AdminArticleService : BaseService
    {
        public void LoadArticles(GridViewDataSet<AdminArticleListDTO> dataSet, ClaimsPrincipal currentUser)
        {
            using (var dc = CreateDbContext())
            {
                var articles = GetArticlesForUser(currentUser, dc);
                var queryable = articles.Select(a => new AdminArticleListDTO()
                {
                    Id = a.Id,
                    Title = a.Title,
                    PublishedDate = a.PublishedDate,
                    BlogName = a.Blog.Title
                });

                dataSet.LoadFromQueryable(queryable);
            }
        }

        private IQueryable<Article> GetArticlesForUser(ClaimsPrincipal currentUser, BlogDbContext dc)
        {
            IQueryable<Article> articles = dc.Articles;
            if (!IsAdmin(currentUser))
            {
                articles = articles.Where(a => a.Blog.BlogAuthors.Any(b => b.IdentityUserId == currentUser.Identity.Name));
            }
            return articles;
        }

        private bool IsAdmin(ClaimsPrincipal currentUser)
        {
            return currentUser.IsInRole("administrator");
        }
    }
}