using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Jwt;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols;
using System.Net.Http;

namespace TaskService.App_Start
{
    // This class is necessary because the OAuthBearer Middleware does not leverage
    // the OpenID Connect metadata endpoint exposed by the STS by default.
    public class OpenIdConnectCachingSecurityTokenProvider : IIssuerSecurityTokenProvider
    {
        public ConfigurationManager<OpenIdConnectConfiguration> _configManager;

        public OpenIdConnectCachingSecurityTokenProvider(string metadataEndpoint)
        {
            HttpClient httpClient = new HttpClient();
            _configManager = new ConfigurationManager<OpenIdConnectConfiguration>(metadataEndpoint, httpClient);

            RetrieveMetadata();
        }

        /// <summary>
        /// Gets the issuer the credentials are for.
        /// </summary>
        /// <value>
        /// The issuer the credentials are for.
        /// </value>
        public string Issuer
        {
            get
            {
                return RetrieveMetadata().Result.Issuer;
            }
        }

        /// <summary>
        /// Gets all known security tokens.
        /// </summary>
        /// <value>
        /// All known security tokens.
        /// </value>
        public IEnumerable<SecurityToken> SecurityTokens
        {
            get
            {
                return RetrieveMetadata().Result.SigningTokens;
            }
        }

        private async Task<OpenIdConnectConfiguration> RetrieveMetadata()
        {
            OpenIdConnectConfiguration config = await _configManager.GetConfigurationAsync();

            return config;
        }
    }
}
