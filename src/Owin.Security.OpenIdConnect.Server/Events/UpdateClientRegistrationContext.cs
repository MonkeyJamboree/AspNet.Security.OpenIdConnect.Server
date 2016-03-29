using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Notifications;
using Newtonsoft.Json.Linq;
using Owin.Security.OpenIdConnect.Extensions;
using System.Collections.Generic;

namespace Owin.Security.OpenIdConnect.Server
{
    public sealed class UpdateClientRegistrationContext : BaseNotification<OpenIdConnectServerOptions>
    {
        internal UpdateClientRegistrationContext(IOwinContext context,
            OpenIdConnectServerOptions options, AuthenticationTicket ticket)
            : base(context, options)
        {
            AuthenticationTicket = ticket;
            RedirectUris = new List<string>();
            ResponseTypes = new List<string> { OpenIdConnectConstants.ResponseTypes.Code };
            GrantTypes = new List<string> { OpenIdConnectConstants.GrantTypes.AuthorizationCode };
            ApplicationType = OpenIdConnectConstants.ApplicationTypes.Web;
            Contacts = new List<string>();
            Scopes = new List<string>();
        }

        /// <summary>
        /// Gets the authentication ticket.
        /// </summary>
        public AuthenticationTicket AuthenticationTicket { get; private set; }

        /// <summary>
        /// REQUIRED for clients using OpenID. Contains a list of Redirection URI values used by the client in an Authorization Request.
        /// </summary>
        public List<string> RedirectUris { get; set; }

        public string ClientId { get; set; }

        public List<string> ResponseTypes { get; set; }

        public List<string> GrantTypes { get; set; }

        public string ApplicationType { get; set; }

        public List<string> Contacts { get; set; }

        public string ClientName { get; set; }

        public string ClientUri { get; set; }

        public string LogoUri { get; set; }

        public List<string> Scopes { get; set; }

        public string TermsOfServiceUri { get; set; }

        public string PolicyUri { get; set; }

        public string JwksUri { get; set; }

        public JObject Jwks { get; set; }

        public string SoftwareId { get; set; }

        public string SoftwareVersion { get; set; }

        public string SectorIdentifierUri { get; set; }

        public string SubjectType { get; set; }
    }
}
