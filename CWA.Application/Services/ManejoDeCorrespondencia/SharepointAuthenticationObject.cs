using DocumentFormat.OpenXml.Bibliography;
using Microsoft.Extensions.Configuration;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Application.Services.ManejoDeCorrespondencia
{
    public class SharepointAuthenticationObject
    {
        public string Site { get; }
        public ClientContext ClientContext { get; }
        public string Token { get; }

        public SharepointAuthenticationObject(IConfiguration _config) 
        {
            Site = _config["Authentication:Sharepoint:Site"];
            
            Uri siteUri = new Uri(Site);
            string grant_type = _config["Authentication:Sharepoint:GrantType"];
            string client_id = _config["Authentication:Sharepoint:ClientId"];
            string client_secret = _config["Authentication:Sharepoint:ClientSecret"];
            string resource = _config["Authentication:Sharepoint:Resource"];

            using var authenticationManager = new SharepointAuthenticationManager();
            using var context = authenticationManager.GetContext( siteUri
                                                                , grant_type
                                                                , client_id
                                                                , client_secret
                                                                , resource);
            string token = authenticationManager.EnsureAccessTokenAsync(new Uri($"{siteUri.Scheme}://{siteUri.DnsSafeHost}"), grant_type, client_id, client_secret, resource).GetAwaiter().GetResult();

            ClientContext = context;
            Token = token;
        }


    }
}
