//----------------------------------------------------------------------------------------------
//    Copyright 2014 Microsoft Corporation
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//----------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

// The following using statements were added for this sample.
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using System.Runtime.InteropServices;
using System.Configuration;
using Microsoft.Experimental.IdentityModel.Clients.ActiveDirectory;

namespace TaskClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HttpClient httpClient = new HttpClient();
        private AuthenticationContext authContext = null;

        protected async override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            authContext = new AuthenticationContext(Globals.aadInstance + Globals.tenant, new FileCache());
            
            AuthenticationResult result = null;
            try
            {
                TokenCacheItem tci = authContext.TokenCache.ReadItems().Where(i => i.Scope.Contains(Globals.clientId) && !string.IsNullOrEmpty(i.Policy)).FirstOrDefault();
                string existingPolicy = tci == null ? null : tci.Policy;
                result = await authContext.AcquireTokenAsync(new string[] { Globals.clientId },
                    null, Globals.clientId, new Uri(Globals.redirectUri),
                    new PlatformParameters(PromptBehavior.Never, null), existingPolicy);

                SignInButton.Visibility = Visibility.Collapsed;
                SignUpButton.Visibility = Visibility.Collapsed;
                EditProfileButton.Visibility = Visibility.Visible;
                SignOutButton.Visibility = Visibility.Visible;
                UsernameLabel.Content = result.UserInfo.Name;
                GetTodoList();
            }
            catch (AdalException ex)
            {
                if (ex.ErrorCode == "user_interaction_required")
                {
                    // There are no tokens in the cache.  Proceed without calling the To Do list service.
                }
                else
                {
                    // An unexpected error occurred.
                    string message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        message += "Inner Exception : " + ex.InnerException.Message;
                    }
                    MessageBox.Show(message);
                }
                return;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void GetTodoList()
        {
            AuthenticationResult result = null;
            try
            {
                TokenCacheItem tci = authContext.TokenCache.ReadItems().Where(i => i.Scope.Contains(Globals.clientId) && !string.IsNullOrEmpty(i.Policy)).FirstOrDefault();
                string existingPolicy = tci == null ? null : tci.Policy;

                result = await authContext.AcquireTokenAsync(new string[] { Globals.clientId },
                    null, Globals.clientId, new Uri(Globals.redirectUri),
                    new PlatformParameters(PromptBehavior.Never, null), existingPolicy);
            
            }
            catch (AdalException ex)
            {
                // There is no access token in the cache, so prompt the user to sign-in.
                if (ex.ErrorCode == "user_interaction_required")
                {
                    MessageBox.Show("Please sign up or sign in first");
                    SignInButton.Visibility = Visibility.Visible;
                    SignUpButton.Visibility = Visibility.Visible;
                    EditProfileButton.Visibility = Visibility.Collapsed;
                    SignOutButton.Visibility = Visibility.Collapsed;
                    UsernameLabel.Content = string.Empty;

                }
                else
                {
                    // An unexpected error occurred.
                    string message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        message += "Inner Exception : " + ex.InnerException.Message;
                    }
                    MessageBox.Show(message);
                }

                return;
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);

            // Call the To Do list service.
            HttpResponseMessage response = await httpClient.GetAsync(Globals.taskServiceUrl + "/api/tasks");

            if (response.IsSuccessStatusCode)
            {
                // Read the response and databind to the GridView to display To Do items.
                string s = await response.Content.ReadAsStringAsync();
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                List<Models.Task> taskArray = serializer.Deserialize<List<Models.Task>>(s);

                TaskList.ItemsSource = taskArray.Select(t => new { t.task });
            }
            else
            {
                MessageBox.Show("An error occurred : " + response.ReasonPhrase);
            }

            return;
        }

        private async void AddTodoItem(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TaskText.Text))
            {
                MessageBox.Show("Please enter a value for the To Do item name");
                return;
            }

            AuthenticationResult result = null;
            try
            {
                TokenCacheItem tci = authContext.TokenCache.ReadItems().Where(i => i.Scope.Contains(Globals.clientId) && !string.IsNullOrEmpty(i.Policy)).FirstOrDefault();
                string existingPolicy = tci == null ? null : tci.Policy;
                result = await authContext.AcquireTokenAsync(new string[] { Globals.clientId },
                    null, Globals.clientId, new Uri(Globals.redirectUri),
                    new PlatformParameters(PromptBehavior.Never, null), existingPolicy);
            }
            catch (AdalException ex)
            {
                // There is no access token in the cache, so prompt the user to sign-in.
                if (ex.ErrorCode == "user_interaction_required")
                {
                    MessageBox.Show("Please sign up or sign in first");
                    SignInButton.Visibility = Visibility.Visible;
                    SignUpButton.Visibility = Visibility.Visible;
                    EditProfileButton.Visibility = Visibility.Collapsed;
                    SignOutButton.Visibility = Visibility.Collapsed;
                    UsernameLabel.Content = string.Empty;
                }
                else
                {
                    // An unexpected error occurred.
                    string message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        message += "Inner Exception : " + ex.InnerException.Message;
                    }

                    MessageBox.Show(message);
                }

                return;
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);

            HttpContent content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("task", TaskText.Text) });

            // Call the To Do list service.
            HttpResponseMessage response = await httpClient.PostAsync(Globals.taskServiceUrl + "/api/tasks", content);

            if (response.IsSuccessStatusCode)
            {
                TaskText.Text = "";
                GetTodoList();
            }
            else
            {
                MessageBox.Show("An error occurred : " + response.ReasonPhrase);
            }
        }

        private async void SignIn(object sender = null, RoutedEventArgs args = null)
        {
            AuthenticationResult result = null;
            try
            {
                result = await authContext.AcquireTokenAsync(new string[] { Globals.clientId },
                    null, Globals.clientId, new Uri(Globals.redirectUri),
                    new PlatformParameters(PromptBehavior.Always, null), Globals.signInPolicy);
                
                SignInButton.Visibility = Visibility.Collapsed;
                SignUpButton.Visibility = Visibility.Collapsed;
                EditProfileButton.Visibility = Visibility.Visible;
                SignOutButton.Visibility = Visibility.Visible;
                UsernameLabel.Content = result.UserInfo.Name;
                GetTodoList();
            }
            catch (AdalException ex)
            {
                if (ex.ErrorCode == "authentication_canceled")
                {
                    MessageBox.Show("Sign in was canceled by the user");
                }
                else
                {
                    // An unexpected error occurred.
                    string message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        message += "Inner Exception : " + ex.InnerException.Message;
                    }

                    MessageBox.Show(message);
                }

                return;
            }

        }

        // This function clears cookies from the browser control used by ADAL.
        private void ClearCookies()
        {
            const int INTERNET_OPTION_END_BROWSER_SESSION = 42;
            InternetSetOption(IntPtr.Zero, INTERNET_OPTION_END_BROWSER_SESSION, IntPtr.Zero, 0);
        }

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

        private async void SignUp(object sender, RoutedEventArgs e)
        {
            AuthenticationResult result = null;
            try
            {
                result = await authContext.AcquireTokenAsync(new string[] { Globals.clientId },
                    null, Globals.clientId, new Uri(Globals.redirectUri),
                    new PlatformParameters(PromptBehavior.Always, null), Globals.signUpPolicy);

                SignInButton.Visibility = Visibility.Collapsed;
                SignUpButton.Visibility = Visibility.Collapsed;
                EditProfileButton.Visibility = Visibility.Visible;
                SignOutButton.Visibility = Visibility.Visible;
                
                UsernameLabel.Content = result.UserInfo.Name;

                GetTodoList();
            }
            catch (AdalException ex)
            {
                if (ex.ErrorCode == "authentication_canceled")
                {
                    MessageBox.Show("Sign up was canceled by the user");
                }
                else
                {
                    // An unexpected error occurred.
                    string message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        message += "Inner Exception : " + ex.InnerException.Message;
                    }

                    MessageBox.Show(message);
                }

                return;
            }
        }

        private async void EditProfile(object sender, RoutedEventArgs e)
        {
            AuthenticationResult result = null;
            try
            {
                result = await authContext.AcquireTokenAsync(new string[] { Globals.clientId },
                    null, Globals.clientId, new Uri(Globals.redirectUri),
                    new PlatformParameters(PromptBehavior.Always, null), Globals.editProfilePolicy);
                UsernameLabel.Content = result.UserInfo.Name;
                GetTodoList();
            }
            catch (AdalException ex)
            {
                if (ex.ErrorCode != "authentication_canceled")
                {
                    // An unexpected error occurred.
                    string message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        message += "Inner Exception : " + ex.InnerException.Message;
                    }

                    MessageBox.Show(message);
                }
                
                return;
            }
        }

        private void SignOut(object sender, RoutedEventArgs e)
        {
            authContext.TokenCache.Clear();

            ClearCookies();

            TaskList.ItemsSource = string.Empty;
            SignInButton.Visibility = Visibility.Visible;
            SignUpButton.Visibility = Visibility.Visible;
            EditProfileButton.Visibility = Visibility.Collapsed;
            SignOutButton.Visibility = Visibility.Collapsed;
            return;
        }

    }
}
