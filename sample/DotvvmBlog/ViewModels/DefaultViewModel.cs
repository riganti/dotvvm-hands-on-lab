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
	public class DefaultViewModel : SiteViewModel
	{

	    public override string PageTitle => "DotVVM Blog";



        public List<ArticleListDTO> RecentArticles { get; private set; }

	    public override Task PreRender()
	    {
	        var service = new HomepageService();
	        RecentArticles = service.GetRecentArticles();

	        return base.PreRender();
	    }
	}
}

