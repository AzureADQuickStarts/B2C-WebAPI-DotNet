using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskClient
{
    public static class Globals
    {
        // TODO: Replace these with your own configuration values
        public static string tenant = "{Enter the name of your B2C tenant - it usually looks like constoso.onmicrosoft.com}";
        public static string clientId = "{Enter the Application ID assigned to your app by the Azure Portal}";
        public static string signInPolicy = "{Enter the name of your sign in policy, e.g. b2c_1_sign_in}";
        public static string signUpPolicy = "{Enter the name of your sign up policy, e.g. b2c_1_sign_up}";
        public static string editProfilePolicy = "{Enter the name of your edit profile policy, e.g. b2c_1_edit_profile}";

        public static string taskServiceUrl = "https://localhost:44332";
        public static string aadInstance = "https://login.microsoftonline.com/";
        public static string redirectUri = "urn:ietf:wg:oauth:2.0:oob";

    }
}
