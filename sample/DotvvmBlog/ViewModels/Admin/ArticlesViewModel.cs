using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotvvmBlog.BL.DTO;
using DotvvmBlog.BL.Services;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Runtime.Filters;
using DotVVM.Framework.ViewModel;

namespace DotvvmBlog.ViewModels.Admin
{
    [Authorize]
	public class ArticlesViewModel : DotvvmBlog.ViewModels.SiteViewModel
    {

        public override string PageTitle => "Administration | DotVVM Blog";

        public GridViewDataSet<AdminArticleListDTO> Articles { get; set; } = new GridViewDataSet<AdminArticleListDTO>()
        {
            PagingOptions =
            {
                PageSize = 20
            },
            SortingOptions =
            {
                SortExpression = nameof(AdminArticleListDTO.PublishedDate),
                SortDescending = true
            }
        };

        public override Task PreRender()
        {
            if (Articles.IsRefreshRequired)
            {
                var service = new AdminArticleService();
                service.LoadArticles(Articles, Context.HttpContext.User);
            }

            return base.PreRender();
        }
    }
}

