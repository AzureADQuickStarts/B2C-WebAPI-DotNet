using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

// The following using statements were added for this sample.
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security.Cookies;
using TaskWebApp.Policies;
using System.Security.Claims;
using Microsoft.Experimental.IdentityModel.Clients.ActiveDirectory;
using TaskWebApp.Utils;
using System.Globalization;

namespace TaskWebApp.Controllers
{
    public class AccountController : Controller
    {
        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties(
                        new Dictionary<string, string> 
                        { 
                            {Startup.PolicyKey, Startup.SignInPolicyId}
                        })
                    {
                        RedirectUri = "/",
                    }, OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }
        public void SignUp()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties(
                        new Dictionary<string, string> 
                        { 
                            {Startup.PolicyKey, Startup.SignUpPolicyId}
                        })
                    {
                        RedirectUri = "/",
                    }, OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }


        public void Profile()
        {
            if (Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties(
                        new Dictionary<string, string> 
                        { 
                            {Startup.PolicyKey, Startup.ProfilePolicyId}
                        })
                    {
                        RedirectUri = "/",
                    }, OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        public void SignOut()
        {
            if (Request.IsAuthenticated)
            {
                // When the user signs out, clear their token cache in the process
                string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
                string authority = String.Format(CultureInfo.InvariantCulture, Startup.aadInstance, Startup.tenant, string.Empty, string.Empty);
                AuthenticationContext authContext = new AuthenticationContext(authority, new NaiveSessionCache(userObjectID));
                authContext.TokenCache.Clear();

                HttpContext.GetOwinContext().Authentication.SignOut(
                new AuthenticationProperties(
                    new Dictionary<string, string> 
                    { 
                        {Startup.PolicyKey, ClaimsPrincipal.Current.FindFirst(Startup.AcrClaimType).Value}
                    }), OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
            }
            
        }
	}
}