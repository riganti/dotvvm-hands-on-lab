using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotvvmBlog.BL.DTO;
using DotvvmBlog.BL.Services;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.ViewModel;

namespace DotvvmBlog.ViewModels
{
	public class SignInViewModel : SiteViewModel
	{
	    public override string PageTitle => "Sign In | DotVVM Blog";

	    public SignInDTO Data { get; set; } = new SignInDTO();

	    public string ErrorMessage { get; set; }

	    public void SignIn()
	    {
	        var userService = new UserService();
	        var identity = userService.SignIn(Data);
	        if (identity == null)
	        {
	            ErrorMessage = "Invalid user name or password!";
	        }
	        else
	        {
	            Context.GetAuthentication().SignIn(identity);
                Context.RedirectToRoute("Default");
	        }
	    }
	}
}

