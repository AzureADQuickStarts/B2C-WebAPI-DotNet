using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace TaskWebApp.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {

        private String accessToken;
        private String apiEndpoint = Startup.serviceUrl + "/api/tasks/";

        // GET: TodoList
        public async Task<ActionResult> Index()
        {
            try
            {
                acquireToken(new string[] { "https://fabrikamb2c.onmicrosoft.com/tasks/read" });

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken); // Add token in header
                HttpResponseMessage response = await client.SendAsync(request);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        String responseString = await response.Content.ReadAsStringAsync();
                        JArray tasks = JArray.Parse(responseString);
                        ViewBag.Tasks = tasks;
                        return View();
                    case HttpStatusCode.Unauthorized:
                        return await errorAction("Please sign in again. " + response.ReasonPhrase);
                    default:
                        return await errorAction("Error. Status code = " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return await errorAction("Error reading to do list: " + ex.Message);
            }
        }

        // POST: TodoList/Create
        [HttpPost]
        public async Task<ActionResult> Create(string description)
        {
            try
            {
                acquireToken(new string[] { "https://fabrikamb2c.onmicrosoft.com/tasks/read" });
                var httpContent = new[] {new KeyValuePair<string, string>("Text", description)};

                HttpClient client = new HttpClient();
                HttpContent content = new FormUrlEncodedContent(httpContent);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Content = content;
                HttpResponseMessage response = await client.SendAsync(request);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.NoContent:
                        return new RedirectResult("/Tasks");
                    case HttpStatusCode.Unauthorized:
                        return await errorAction("Please sign in again. " + response.ReasonPhrase);
                    default:
                        return await errorAction("Error. Status code = " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return await errorAction("Error writing to list: " + ex.Message);
            }
        }

        // POST: /TodoList/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                acquireToken(new string[] { "https://fabrikamb2c.onmicrosoft.com/tasks/read" });

                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, apiEndpoint + id);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken); // Add token in header
                HttpResponseMessage response = await client.SendAsync(request);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.NoContent:
                        return new RedirectResult("/Tasks");
                    case HttpStatusCode.Unauthorized:
                        return await errorAction("Please sign in again. " + response.ReasonPhrase);
                    default:
                        return await errorAction("Error. Status code = " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return await errorAction("Error deleting from list: " + ex.Message);
            }
        }

        private async void acquireToken(String[] scope)
        {
            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            string authority = String.Format(Startup.aadInstance, Startup.tenant, Startup.SignInPolicyId);

            ClientCredential credential = new ClientCredential(Startup.clientSecret);

            // Here you ask for a token using the web app's clientId as the scope, since the web app and service share the same clientId.
            ConfidentialClientApplication app = new ConfidentialClientApplication(authority, Startup.clientId, Startup.redirectUri, credential, new NaiveSessionCache(userObjectID, this.HttpContext)) { };
            AuthenticationResult result = await app.AcquireTokenSilentAsync(scope);

            accessToken = result.Token;
        }

        private async Task<ActionResult> errorAction(String message)
        {
            return new RedirectResult("/Error?message=" + message);
        }

    }
}