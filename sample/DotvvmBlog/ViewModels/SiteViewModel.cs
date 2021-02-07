using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.ViewModel;

namespace DotvvmBlog.ViewModels
{
    public abstract class SiteViewModel : DotvvmViewModelBase
    {

        public abstract string PageTitle { get; }

        public string CurrentUserName => Context.HttpContext.User.Identity.Name;

        public void SignOut()
        {
            Context.GetAuthentication().SignOut();
            Context.RedirectToRoute("Default");
        }

    }
}

