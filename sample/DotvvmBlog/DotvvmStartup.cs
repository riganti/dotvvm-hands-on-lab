using System.Web.Hosting;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;
using DotVVM.Framework;
using DotVVM.Framework.Configuration;
using DotVVM.Framework.ResourceManagement;
using DotVVM.Framework.Routing;

namespace DotvvmBlog
{
    public class DotvvmStartup : IDotvvmStartup
    {
        // For more information about this class, visit https://dotvvm.com/docs/tutorials/basics-project-structure
        public void Configure(DotvvmConfiguration config, string applicationPath)
        {
            ConfigureRoutes(config, applicationPath);
            ConfigureControls(config, applicationPath);
            ConfigureResources(config, applicationPath);
        }

        private void ConfigureRoutes(DotvvmConfiguration config, string applicationPath)
        {
            config.RouteTable.Add("Default", "", "Views/default.dothtml");
            config.RouteTable.Add("ArticleDetail", "article/{Id}", "Views/ArticleDetail.dothtml");

            config.RouteTable.Add("SignIn", "sign-in", "Views/SignIn.dothtml");

            config.RouteTable.Add("Admin_Articles", "admin/articles", "Views/Admin/Articles.dothtml");
            config.RouteTable.Add("Admin_ArticleDetail", "admin/article/{Id}", "Views/Admin/ArticleDetail.dothtml");

            // Uncomment the following line to auto-register all dothtml files in the Views folder
            // config.RouteTable.AutoDiscoverRoutes(new DefaultRouteStrategy(config));    
        }

        private void ConfigureControls(DotvvmConfiguration config, string applicationPath)
        {
            // register code-only controls and markup controls
            config.Markup.AddMarkupControl("cc", "Menu", "Controls/Menu.dotcontrol");
        }

        private void ConfigureResources(DotvvmConfiguration config, string applicationPath)
        {
            // register custom resources and adjust paths to the built-in resources
            config.Resources.Register("bootstrap", new StylesheetResource()
            {
                Location = new UrlResourceLocation("/Content/bootstrap.min.css"),
                Dependencies = new[] { "bootstrap-js" }
            });
            config.Resources.Register("bootstrap-js", new ScriptResource()
            {
                Location = new UrlResourceLocation("/Scripts/bootstrap.min.js"),
                Dependencies = new[] { "jquery" }
            });

            config.Resources.Register("site", new StylesheetResource()
            {
                Location = new UrlResourceLocation("/Content/site.css"),
                Dependencies = new[] { "bootstrap" }
            });
        }
    }
}
