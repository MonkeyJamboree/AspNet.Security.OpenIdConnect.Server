using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Notifications;

namespace Owin.Security.OpenIdConnect.Server
{
    public sealed class DeleteClientRegistrationContext : BaseNotification<OpenIdConnectServerOptions>
    {
        internal DeleteClientRegistrationContext(IOwinContext context,
            OpenIdConnectServerOptions options, AuthenticationTicket ticket)
            : base(context, options)
        {
            AuthenticationTicket = ticket;
        }

        /// <summary>
        /// Gets the authentication ticket.
        /// </summary>
        public AuthenticationTicket AuthenticationTicket { get; private set; }

        public string ClientId { get; set; }
    }
}
