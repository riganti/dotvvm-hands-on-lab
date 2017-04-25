using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotvvmBlog.BL.DTO;
using DotvvmBlog.BL.Services;
using DotVVM.Framework.ViewModel;

namespace DotvvmBlog.ViewModels
{
    public class ArticleDetailViewModel : SiteViewModel
    {
        public override string PageTitle => Article.Title + " | DotVVM Blog";

        public ArticleDetailDTO Article { get; set; }

        public List<ArticleCommentDTO> Comments { get; set; }

        public NewCommentDTO NewComment { get; set; } = new NewCommentDTO();

        public override Task PreRender()
        {
            var articleId = Convert.ToInt32(Context.Parameters["Id"]);

            var service = new ArticleDetailService();

            if (!Context.IsPostBack)
            {
                Article = service.GetArticle(articleId);
            }

            Comments = service.GetComments(articleId);

            return base.PreRender();
        }

        public void PostComment()
        {
            var articleId = Convert.ToInt32(Context.Parameters["Id"]);

            var service = new ArticleDetailService();
            service.PostComment(articleId, NewComment);

            NewComment = new NewCommentDTO();
        }

    }
}

