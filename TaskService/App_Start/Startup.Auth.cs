using Microsoft.Owin.Security.ActiveDirectory;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskService.App_Start;

namespace TaskService
{
    public partial class Startup
    {
        // These values are pulled from web.config
        public static string aadInstance = ConfigurationManager.AppSettings["ida:AadInstance"];
        public static string tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        public static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        public static string commonPolicy = ConfigurationManager.AppSettings["ida:PolicyId"];
        private const string discoverySuffix = ".well-known/openid-configuration";

        public void ConfigureAuth(IAppBuilder app)
        {   
            // TODO: Configure the OWIN OAuth Pipeline
        }
    }
}
