using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotvvmBlog.BL.DTO;

namespace DotvvmBlog.BL.Services
{
    public class HomepageService : BaseService
    {

        public List<ArticleListDTO> GetRecentArticles()
        {
            using (var dc = CreateDbContext())
            {
                return dc.Articles
                    .Where(a => a.PublishedDate < DateTime.Now)
                    .OrderByDescending(a => a.PublishedDate)
                    .Take(10)
                    .ToList()
                    .Select(a => new ArticleListDTO()
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Abstract = HtmlUtils.Ellipsis(HtmlUtils.GetTextFromHtml(a.Html), 300),
                        PublishedDate = a.PublishedDate,
                        AuthorName = a.Blog.AuthorName
                    })
                    .ToList();
            }
        }

    }
}
