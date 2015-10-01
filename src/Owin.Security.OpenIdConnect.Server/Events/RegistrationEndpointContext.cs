/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OpenIdConnect.Server
 * for more information concerning the license and the contributors participating to this project.
 */

using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Notifications;

namespace Owin.Security.OpenIdConnect.Server {
    /// <summary>
    /// Provides context information used when processing an OpenIdConnect token request.
    /// </summary>
    public sealed class RegistrationEndpointContext : BaseNotification<OpenIdConnectServerOptions> {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenEndpointContext"/> class
        /// </summary>
        /// <param name="context"></param>
        /// <param name="options"></param>
        /// <param name="request"></param>
        /// <param name="ticket"></param>
        internal TokenEndpointContext(
            IOwinContext context,
            OpenIdConnectServerOptions options,
            OpenIdConnectMessage request,
            AuthenticationTicket ticket)
            : base(context, options) {
            Request = request;
            Ticket = ticket;
        }

        /// <summary>
        /// Gets or sets the authentication ticket.
        /// </summary>
        public AuthenticationTicket Ticket { get; set; }

        /// <summary>
        /// Gets the token endpoint request. 
        /// </summary>
        public new OpenIdConnectMessage Request { get; }

        public string[] RedirectUris { get; set; }
        public string[] ResponseTypes { get; set; }
        public string[] GrantTypes { get; set; }
        public string[] Contacts { get; set; }
        public string ClientName { get; set; }
        public string LogoUri { get; set; }
        public string ClientUri { get; set; }
        public string PolicyUri { get; set; }
        public string TermsOfServiceUri { get; set; }
        public string JwksUri { get; set; }
        public string Jwks { get; set; }
        public string TokenEndpointAuthMethod { get; set; }
        public string ApplicationType { get; set; }
        public string SectorIdentifierUri { get; set; }
        public string SubjectType { get; set; }
        public string IdTokenSignedResponseAlg { get; set; }
        public string IdTokenEncryptedResponseAlg { get; set; }
        public string IdTokenEncryptedResponseEnc { get; set; }
        public string UserinfoSignedResponseAlg { get; set; }
        public string UserinfoEncryptedResponseAlg { get; set; }
        public string UserinfoEncryptedResponseEnc { get; set; }
        public string RequestObjectSigningAlg { get; set; }
        public string RequestObjectEncryptionAlg { get; set; }
        public string RequestObjectEncryptionEnc { get; set; }
        public string TokenEndpointAuthSigningAlg { get; set; }
        public int DefaultMaxAge { get; set; }
        public bool RequireAuthTime { get; set; }
        public string[] DefaultAcrValues { get; set; }
        public string InitiateLoginUri { get; set; }
        public string[] RequestUris { get; set; }
        public string Scope { get; set; }
        public string SoftwareId { get; set; }
        public string SoftwareVersion { get; set; }
    }
}
