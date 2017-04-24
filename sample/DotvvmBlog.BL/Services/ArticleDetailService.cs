using System;
using System.Collections.Generic;
using System.Linq;
using DotvvmBlog.BL.DTO;
using DotvvmBlog.DAL.Model;

namespace DotvvmBlog.BL.Services
{
    public class ArticleDetailService : BaseService
    {

        public ArticleDetailDTO GetArticle(int articleId)
        {
            using (var dc = CreateDbContext())
            {
                return dc.Articles
                    .Select(a => new ArticleDetailDTO()
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Html = a.Html,
                        PublishedDate = a.PublishedDate,
                        AuthorName = a.Blog.AuthorName,
                        BlogName = a.Blog.Title,
                        Tags = a.ArticleTags.Select(t => t.Tag.Name)
                    })
                    .FirstOrDefault(a => a.Id == articleId);
            }
        }

        public List<ArticleCommentDTO> GetComments(int articleId)
        {
            using (var dc = CreateDbContext())
            {
                return dc.Comments
                    .Where(c => c.ArticleId == articleId)
                    .OrderBy(c => c.CreatedDate)
                    .Select(a => new ArticleCommentDTO()
                    {
                        Html = a.Html,
                        CreatedDate = a.CreatedDate
                    })
                    .ToList();
            }
        }

        public void PostComment(int articleId, NewCommentDTO data)
        {
            using (var dc = CreateDbContext())
            {
                var comment = new Comment()
                {
                    ArticleId = articleId,
                    CreatedDate = DateTime.Now,
                    Html = HtmlUtils.GetHtmlFromText(data.Text),
                    IpAddress = data.IpAddress
                };
                dc.Comments.Add(comment);
                dc.SaveChanges();
            }
        }
    }
}