using Microsoft.Owin;

namespace Owin.Security.OpenIdConnect.Server
{
    public class ValidateRegistrationRequestContext : BaseValidatingContext
    {
        public ValidateRegistrationRequestContext(
            IOwinContext context,
            OpenIdConnectServerOptions options,
            IOwinRequest request)
            : base(context, options) {
            Request = request;
        }

        /// <summary>
        /// Gets the authorization request.
        /// </summary>
        public new IOwinRequest Request { get; }

        public string ClientId { get; set; }

        public string HttpMethod => Request.Method;

        public bool IsCreateRequest { get; private set; }
        public bool IsReadRequest { get; private set; }
        public bool IsUpdateRequest { get; private set; }
        public bool IsDeleteRequest { get; private set; }

        public void MatchesCreateRequest()
        {
            IsCreateRequest = true;
            IsReadRequest = false;
            IsUpdateRequest = false;
            IsDeleteRequest = false;
        }

        public void MatchesReadRequest()
        {
            IsCreateRequest = false;
            IsReadRequest = true;
            IsUpdateRequest = false;
            IsDeleteRequest = false;
        }

        public void MatchesUpdateRequest()
        {
            IsCreateRequest = false;
            IsReadRequest = false;
            IsUpdateRequest = true;
            IsDeleteRequest = false;
        }

        public void MatchesDeleteRequest()
        {
            IsCreateRequest = false;
            IsReadRequest = false;
            IsUpdateRequest = false;
            IsDeleteRequest = true;
        }

        public void MatchesNothing()
        {
            IsCreateRequest = false;
            IsReadRequest = false;
            IsUpdateRequest = false;
            IsDeleteRequest = false;
        }
    }
}
