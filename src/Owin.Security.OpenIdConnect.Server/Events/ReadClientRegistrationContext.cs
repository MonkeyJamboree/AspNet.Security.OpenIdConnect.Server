using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Notifications;
using Newtonsoft.Json.Linq;
using Owin.Security.OpenIdConnect.Extensions;
using System.Collections.Generic;

namespace Owin.Security.OpenIdConnect.Server.Events
{
    public sealed class ReadClientRegistrationContext : BaseNotification<OpenIdConnectServerOptions>
    {
        internal ReadClientRegistrationContext(IOwinContext context,
            OpenIdConnectServerOptions options, AuthenticationTicket ticket)
            : base(context, options)
        {
            AuthenticationTicket = ticket;
        }

        /// <summary>
        /// Gets the authentication ticket.
        /// </summary>
        public AuthenticationTicket AuthenticationTicket { get; private set; }

        pubilc string ClientId { get; set; }
    }
}
