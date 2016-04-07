﻿/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OpenIdConnect.Server
 * for more information concerning the license and the contributors participating to this project.
 */

using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Notifications;

namespace Owin.Security.OpenIdConnect.Server {
    /// <summary>
    /// Default implementation of <see cref="IOpenIdConnectServerProvider"/> used by the authorization
    /// server to communicate with the web application while processing requests.
    /// <see cref="OpenIdConnectServerProvider"/> provides some default behavior, 
    /// may be used as a virtual base class, and offers delegate properties
    /// which may be used to handle individual calls without declaring a new class type.
    /// </summary>
    public class OpenIdConnectServerProvider : IOpenIdConnectServerProvider {
        /// <summary>
        /// Called to determine if an incoming request is treated as an authorization or token
        /// endpoint. If Options.AuthorizationEndpointPath or Options.TokenEndpointPath
        /// are assigned values, then handling this event is optional and context.IsAuthorizationEndpoint and context.IsTokenEndpoint
        /// will already be true if the request path matches.
        /// </summary>
        public Func<MatchEndpointContext, Task> OnMatchEndpoint { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called for each request to the authorization endpoint to determine if the request is valid and should continue.
        /// </summary>
        public Func<ValidateAuthorizationRequestContext, Task> OnValidateAuthorizationRequest { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called for each request to the configuration endpoint to determine if the request is valid and should continue.
        /// </summary>
        public Func<ValidateConfigurationRequestContext, Task> OnValidateConfigurationRequest { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called for each request to the cryptography endpoint to determine if the request is valid and should continue.
        /// </summary>
        public Func<ValidateCryptographyRequestContext, Task> OnValidateCryptographyRequest { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called for each request to the introspection endpoint to determine if the request is valid and should continue.
        /// </summary>
        public Func<ValidateIntrospectionRequestContext, Task> OnValidateIntrospectionRequest { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called for each request to the logout endpoint to determine if the request is valid and should continue.
        /// </summary>
        public Func<ValidateLogoutRequestContext, Task> OnValidateLogoutRequest { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called for each request to the registration endpoint to determine if the request is valid and should continue.
        /// </summary>
        public Func<ValidateRegistrationRequestContext, Task> OnValidateRegistrationRequest { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called for each request to the token endpoint to determine if the request is valid and should continue.
        /// </summary>
        public Func<ValidateTokenRequestContext, Task> OnValidateTokenRequest { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called for each request to the userinfo endpoint to determine if the request is valid and should continue.
        /// </summary>
        public Func<ValidateUserinfoRequestContext, Task> OnValidateUserinfoRequest { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called when a request to the Token endpoint arrives with a "grant_type" of "authorization_code". This occurs after the authorization
        /// endpoint as redirected the user-agent back to the client with a "code" parameter, and the client is exchanging that for an "access_token".
        /// The claims and properties associated with the authorization code are present in the context.Ticket.
        /// The token request is automatically handled, but the application can call context.Rejected to instruct the Authorization Server middleware to reject the authorization code.
        /// The application may explicitly call context.Validated and flow a different AuthenticationTicket or ClaimsIdentity in order to control which information flows from authorization code to access token.
        /// The default behavior when using the OpenIdConnectServerProvider is to flow information from the authorization code to the access token unmodified.
        /// See also http://tools.ietf.org/html/rfc6749#section-4.1.3
        /// </summary>
        public Func<GrantAuthorizationCodeContext, Task> OnGrantAuthorizationCode { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called when a request to the Token endpoint arrives with a "grant_type" of "password". This occurs when the user has provided name and password
        /// credentials directly into the client application's user interface, and the client application is using those to acquire an "access_token" and 
        /// optional "refresh_token". If the web application supports the
        /// resource owner credentials grant type it must validate the context.Username and context.Password as appropriate. To issue an
        /// access token the context.Validated must be called with a new ticket containing the claims about the resource owner which should be associated
        /// with the access token. The application should take appropriate measures to ensure that the endpoint isn�t abused by malicious callers.
        /// The default behavior is to reject this grant type.
        /// See also http://tools.ietf.org/html/rfc6749#section-4.3.2
        /// </summary>
        public Func<GrantResourceOwnerCredentialsContext, Task> OnGrantResourceOwnerCredentials { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called when a request to the Token endpoint arrives with a "grant_type" of "client_credentials". This occurs when a registered client
        /// application wishes to acquire an "access_token" to interact with protected resources on it's own behalf, rather than on behalf of an authenticated user. 
        /// If the web application supports the client credentials it may assume the context.ClientId has been validated by the ValidateClientAuthentication call.
        /// To issue an access token the context.Validated must be called with a new ticket containing the claims about the client application which should be associated
        /// with the access token. The application should take appropriate measures to ensure that the endpoint isn�t abused by malicious callers.
        /// The default behavior is to reject this grant type.
        /// See also http://tools.ietf.org/html/rfc6749#section-4.4.2
        /// </summary>
        public Func<GrantClientCredentialsContext, Task> OnGrantClientCredentials { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called when a request to the Token endpoint arrives with a "grant_type" of "refresh_token". This occurs if your application has issued a "refresh_token" 
        /// along with the "access_token", and the client is attempting to use the "refresh_token" to acquire a new "access_token", and possibly a new "refresh_token".
        /// The claims and properties associated with the refresh token are present in the context.Ticket. The token request is automatically handled,
        /// but the application can call context.Rejected to instruct the Authorization Server middleware to reject the token.
        /// The application may explicitly call context.Validated and flow a different AuthenticationTicket or ClaimsIdentity in order to control
        /// which information flows from the refresh token to the access token. The default behavior when using the OpenIdConnectServerProvider
        /// is to flow information from the refresh token to the access token unmodified.
        /// See also http://tools.ietf.org/html/rfc6749#section-6
        /// </summary>
        public Func<GrantRefreshTokenContext, Task> OnGrantRefreshToken { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called when a request to the Token andpoint arrives with a "grant_type" of any other value. If the application supports custom grant types
        /// it is entirely responsible for determining if the request should result in an access_token. If context.Validated is called with ticket
        /// information the response body is produced in the same way as the other standard grant types. If additional response parameters must be
        /// included they may be added in the final TokenEndpoint call.
        /// See also http://tools.ietf.org/html/rfc6749#section-4.5
        /// </summary>
        public Func<GrantCustomExtensionContext, Task> OnGrantCustomExtension { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called at the final stage of an incoming authorization endpoint request before the execution continues on to the web application component 
        /// responsible for producing the html response. Anything present in the OWIN pipeline following the Authorization Server may produce the
        /// response for the authorization page. If running on IIS any ASP.NET technology running on the server may produce the response for the 
        /// authorization page. If the web application wishes to produce the response directly in the AuthorizationEndpoint call it may write to the 
        /// context.Response directly and should call context.RequestCompleted to stop other handlers from executing. If the web application wishes
        /// to grant the authorization directly in the AuthorizationEndpoint call it cay call context.OwinContext.Authentication.SignIn with the
        /// appropriate ClaimsIdentity and should call context.RequestCompleted to stop other handlers from executing.
        /// </summary>
        public Func<HandleAuthorizationRequestContext, Task> OnHandleAuthorizationRequest { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called by the client applications to retrieve the OpenID Connect configuration associated with this instance.
        /// An application may implement this call in order to do any final modification to the configuration metadata.
        /// </summary>
        public Func<HandleConfigurationRequestContext, Task> OnHandleConfigurationRequest { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called by the client applications to retrieve the OpenID Connect JSON Web Key set associated with this instance.
        /// An application may implement this call in order to do any final modification to the keys set.
        /// </summary>
        public Func<HandleCryptographyRequestContext, Task> OnHandleCryptographyRequest { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called by the applications to determine the status and metadata for a token.
        /// Validation conforms to the OAuth 2.0 Token Introspection specification with some additions. See documentation for details.
        /// An application may implement this call in order to do any final modification to the token status and metadata.
        /// </summary>
        public Func<HandleIntrospectionRequestContext, Task> OnHandleIntrospectionRequest { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called at the final stage of an incoming logout endpoint request before the execution continues on to the web application component 
        /// responsible for producing the html response. Anything present in the OWIN pipeline following the Authorization Server may produce the
        /// response for the logout page. If running on IIS any ASP.NET technology running on the server may produce the response for the 
        /// logout page. If the web application wishes to produce the response directly in the LogoutEndpoint call it may write to the 
        /// context.Response directly and should call context.RequestCompleted to stop other handlers from executing.
        /// </summary>
        public Func<HandleLogoutRequestContext, Task> OnHandleLogoutRequest { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called at the final stage of an incoming authorization endpoint request before the execution continues on to the web application component 
        /// responsible for producing the response. Anything present in the OWIN pipeline following the Authorization Server may produce the
        /// response for the registration request. If running on IIS any ASP.NET technology running on the server may produce the response for the 
        /// registration request. If the web application wishes to produce the response directly in the HandleRegistrationResponse call it may write to the 
        /// context.Response directly and should call context.RequestCompleted to stop other handlers from executing.
        /// </summary>
        public Func<HandleRegistrationRequestContext, Task> OnHandleRegistrationRequest { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called at the final stage of a successful Token endpoint request.
        /// An application may implement this call in order to do any final 
        /// modification of the claims being used to issue access or refresh tokens. 
        /// </summary>
        public Func<HandleTokenRequestContext, Task> OnHandleTokenRequest { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called at the final stage of an incoming userinfo endpoint request before the execution continues on to the web application component 
        /// responsible for producing the JSON response. Anything present in the OWIN pipeline following the Authorization Server may produce the
        /// response for the userinfo response. If the web application wishes to produce the response directly in the UserinfoEndpoint call it
        /// may write to the context.Response directly and should call context.HandleResponse to stop other handlers from executing.
        /// </summary>
        public Func<HandleUserinfoRequestContext, Task> OnHandleUserinfoRequest { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called before the AuthorizationEndpoint redirects its response to the caller.
        /// The response could contain an access token when using implicit flow or
        /// an authorization code when using the authorization code flow.
        /// If the web application wishes to produce the authorization response directly in the AuthorizationEndpoint call it may write to the 
        /// context.Response directly and should call context.RequestCompleted to stop other handlers from executing.
        /// This call may also be used to add additional response parameters to the authorization response.
        /// </summary>
        public Func<ApplyAuthorizationResponseContext, Task> OnApplyAuthorizationResponse { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called before the authorization server starts emitting the OpenID Connect configuration associated with this instance.
        /// If the web application wishes to produce the configuration metadata directly in this call, it may write to the 
        /// context.Response directly and should call context.RequestCompleted to stop the default behavior from executing.
        /// </summary>
        public Func<ApplyConfigurationResponseContext, Task> OnApplyConfigurationResponse { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called before the authorization server starts emitting the OpenID Connect JSON Web Key set associated with this instance.
        /// If the web application wishes to produce the JSON Web Key set directly in this call, it may write to the 
        /// context.Response directly and should call context.RequestCompleted to stop the default behavior from executing.
        /// </summary>
        public Func<ApplyCryptographyResponseContext, Task> OnApplyCryptographyResponse { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called before the authorization server starts emitting the status and metadata associated with the token received.
        /// Validation conforms to the OAuth 2.0 Token Introspection specification with some additions. See documentation for details.
        /// If the web application wishes to produce the token status and metadata directly in this call, it may write to the 
        /// context.Response directly and should call context.RequestCompleted to stop the default behavior from executing.
        /// </summary>
        public Func<ApplyIntrospectionResponseContext, Task> OnApplyIntrospectionResponse { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called before the LogoutEndpoint endpoint redirects its response to the caller.
        /// If the web application wishes to produce the authorization response directly in the LogoutEndpoint call it may write to the 
        /// context.Response directly and should call context.RequestCompleted to stop other handlers from executing.
        /// This call may also be used to add additional response parameters to the authorization response.
        /// </summary>
        public Func<ApplyLogoutResponseContext, Task> OnApplyLogoutResponse { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called before the Registration endpoint redirects its response to the caller.
        /// If the web application wishes to produce the authorization response directly in the RegistrationEndpoint call it may write to the 
        /// context.Response directly and should call context.RequestCompleted to stop other handlers from executing.
        /// This call may also be used to add additional response parameters to the authorization response.
        /// </summary>
        public Func<ApplyRegistrationResponseContext, Task> OnApplyRegistrationResponse { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called before the TokenEndpoint redirects its response to the caller.
        /// This call may also be used in order to add additional 
        /// response parameters to the JSON response payload.
        /// </summary>
        public Func<ApplyTokenResponseContext, Task> OnApplyTokenResponse { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called before the UserinfoEndpoint endpoint starts writing to the response stream.
        /// If the web application wishes to produce the userinfo response directly in the UserinfoEndpoint call it may write to the 
        /// context.Response directly and should call context.RequestCompleted to stop other handlers from executing.
        /// This call may also be used to add additional response parameters to the authorization response.
        /// </summary>
        public Func<ApplyUserinfoResponseContext, Task> OnApplyUserinfoResponse { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called to create a new authorization code. An application may use this context
        /// to replace the authentication ticket before it is serialized or to use its own code store
        /// and skip the default logic using <see cref="BaseControlContext.HandleResponse"/>.
        /// </summary>
        public Func<SerializeAuthorizationCodeContext, Task> OnSerializeAuthorizationCode { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called to create a new access token. An application may use this context
        /// to replace the authentication ticket before it is serialized or to use its own token format
        /// and skip the default logic using <see cref="BaseControlContext.HandleResponse"/>.
        /// </summary>
        public Func<SerializeAccessTokenContext, Task> OnSerializeAccessToken { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called to create a new identity token. An application may use this context
        /// to replace the authentication ticket before it is serialized or to use its own token format
        /// and skip the default logic using <see cref="BaseControlContext.HandleResponse"/>.
        /// </summary>
        public Func<SerializeIdentityTokenContext, Task> OnSerializeIdentityToken { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called to create a new refresh token. An application may use this context
        /// to replace the authentication ticket before it is serialized or to use its own token format
        /// and skip the default logic using <see cref="BaseControlContext.HandleResponse"/>.
        /// </summary>
        public Func<SerializeRefreshTokenContext, Task> OnSerializeRefreshToken { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called when receiving an authorization code. An application may use this context
        /// to deserialize the code using a custom format and to skip the default logic using
        /// <see cref="BaseControlContext.HandleResponse"/>.
        /// </summary>
        public Func<DeserializeAuthorizationCodeContext, Task> OnDeserializeAuthorizationCode { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called when receiving an access token. An application may use this context
        /// to deserialize the token using a custom format and to skip the default logic using
        /// <see cref="BaseControlContext.HandleResponse"/>.
        /// </summary>
        public Func<DeserializeAccessTokenContext, Task> OnDeserializeAccessToken { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called when receiving an identity token. An application may use this context
        /// to deserialize the token using a custom format and to skip the default logic using
        /// <see cref="BaseControlContext.HandleResponse"/>.
        /// </summary>
        public Func<DeserializeIdentityTokenContext, Task> OnDeserializeIdentityToken { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called when receiving a refresh token. An application may use this context
        /// to deserialize the code using a custom format and to skip the default logic using
        /// <see cref="BaseControlContext.HandleResponse"/>.
        /// </summary>
        public Func<DeserializeRefreshTokenContext, Task> OnDeserializeRefreshToken { get; set; } = context => Task.FromResult<object>(null);

        public Func<CreateClientRegistrationContext, Task> OnCreateClientRegistration { get; set; } = context => Task.FromResult<object>(null);
        public Func<ReadClientRegistrationContext, Task> OnReadClientRegistration { get; set; } = context => Task.FromResult<object>(null);
        public Func<UpdateClientRegistrationContext, Task> OnUpdateClientRegistration { get; set; } = context => Task.FromResult<object>(null);
        public Func<DeleteClientRegistrationContext, Task> OnDeleteClientRegistration { get; set; } = context => Task.FromResult<object>(null);

        /// <summary>
        /// Called to determine if an incoming request is treated as an authorization or token
        /// endpoint. If Options.AuthorizationEndpointPath or Options.TokenEndpointPath
        /// are assigned values, then handling this event is optional and context.IsAuthorizationEndpoint and context.IsTokenEndpoint
        /// will already be true if the request path matches.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task MatchEndpoint(MatchEndpointContext context) => OnMatchEndpoint(context);

        /// <summary>
        /// Called for each request to the authorization endpoint to determine if the request is valid and should continue.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task ValidateAuthorizationRequest(ValidateAuthorizationRequestContext context) => OnValidateAuthorizationRequest(context);

        /// <summary>
        /// Called for each request to the configuration endpoint to determine if the request is valid and should continue.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task ValidateConfigurationRequest(ValidateConfigurationRequestContext context) => OnValidateConfigurationRequest(context);

        /// <summary>
        /// Called for each request to the cryptography endpoint to determine if the request is valid and should continue.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task ValidateCryptographyRequest(ValidateCryptographyRequestContext context) => OnValidateCryptographyRequest(context);

        /// <summary>
        /// Called for each request to the introspection endpoint to determine if the request is valid and should continue.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task ValidateIntrospectionRequest(ValidateIntrospectionRequestContext context) => OnValidateIntrospectionRequest(context);

        /// <summary>
        /// Called for each request to the logout endpoint to determine if the request is valid and should continue.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task ValidateLogoutRequest(ValidateLogoutRequestContext context) => OnValidateLogoutRequest(context);

        /// <summary>
        /// Called for each request to the token endpoint to determine if the request is valid and should continue.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task ValidateTokenRequest(ValidateTokenRequestContext context) => OnValidateTokenRequest(context);

        /// <summary>
        /// Called for each request to the userinfo endpoint to determine if the request is valid and should continue.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task ValidateUserinfoRequest(ValidateUserinfoRequestContext context) => OnValidateUserinfoRequest(context);

        /// <summary>
        /// Called when a request to the Token endpoint arrives with a "grant_type" of "authorization_code". This occurs after the authorization
        /// endpoint as redirected the user-agent back to the client with a "code" parameter, and the client is exchanging that for an "access_token".
        /// The claims and properties associated with the authorization code are present in the context.Ticket.
        /// The token request is automatically handled, but the application can call context.Rejected to instruct the Authorization Server middleware to reject the authorization code.
        /// The application may explicitly call context.Validated and flow a different AuthenticationTicket or ClaimsIdentity in order to control which information flows from authorization code to access token.
        /// The default behavior when using the OpenIdConnectServerProvider is to flow information from the authorization code to the access token unmodified.
        /// See also http://tools.ietf.org/html/rfc6749#section-4.1.3
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task GrantAuthorizationCode(GrantAuthorizationCodeContext context) => OnGrantAuthorizationCode(context);

        /// <summary>
        /// Called when a request to the Token endpoint arrives with a "grant_type" of "refresh_token". This occurs if your application has issued a "refresh_token" 
        /// along with the "access_token", and the client is attempting to use the "refresh_token" to acquire a new "access_token", and possibly a new "refresh_token".
        /// The claims and properties associated with the refresh token are present in the context.Ticket. The token request is automatically handled,
        /// but the application can call context.Rejected to instruct the Authorization Server middleware to reject the token.
        /// The application may explicitly call context.Validated and flow a different AuthenticationTicket or ClaimsIdentity in order to control
        /// which information flows from the refresh token to the access token. The default behavior when using the OpenIdConnectServerProvider
        /// is to flow information from the refresh token to the access token unmodified.
        /// See also http://tools.ietf.org/html/rfc6749#section-6
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task GrantRefreshToken(GrantRefreshTokenContext context) => OnGrantRefreshToken(context);

        /// <summary>
        /// Called when a request to the Token endpoint arrives with a "grant_type" of "password". This occurs when the user has provided name and password
        /// credentials directly into the client application's user interface, and the client application is using those to acquire an "access_token" and 
        /// optional "refresh_token". If the web application supports the
        /// resource owner credentials grant type it must validate the context.Username and context.Password as appropriate. To issue an
        /// access token the context.Validated must be called with a new ticket containing the claims about the resource owner which should be associated
        /// with the access token. The application should take appropriate measures to ensure that the endpoint isn't abused by malicious callers.
        /// The default behavior is to reject this grant type.
        /// See also http://tools.ietf.org/html/rfc6749#section-4.3.2
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task GrantResourceOwnerCredentials(GrantResourceOwnerCredentialsContext context) => OnGrantResourceOwnerCredentials(context);

        /// <summary>
        /// Called when a request to the Token endpoint arrives with a "grant_type" of "client_credentials". This occurs when a registered client
        /// application wishes to acquire an "access_token" to interact with protected resources on it's own behalf, rather than on behalf of an authenticated user. 
        /// If the web application supports the client credentials it may assume the context.ClientId has been validated by the ValidateClientAuthentication call.
        /// To issue an access token the context.Validated must be called with a new ticket containing the claims about the client application which should be associated
        /// with the access token. The application should take appropriate measures to ensure that the endpoint isn't abused by malicious callers.
        /// The default behavior is to reject this grant type.
        /// See also http://tools.ietf.org/html/rfc6749#section-4.4.2
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task GrantClientCredentials(GrantClientCredentialsContext context) => OnGrantClientCredentials(context);

        /// <summary>
        /// Called when a request to the Token andpoint arrives with a "grant_type" of any other value. If the application supports custom grant types
        /// it is entirely responsible for determining if the request should result in an access_token. If context.Validated is called with ticket
        /// information the response body is produced in the same way as the other standard grant types. If additional response parameters must be
        /// included they may be added in the final TokenEndpoint call.
        /// See also http://tools.ietf.org/html/rfc6749#section-4.5
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task GrantCustomExtension(GrantCustomExtensionContext context) => OnGrantCustomExtension(context);

        /// <summary>
        /// Called at the final stage of an incoming authorization endpoint request before the execution continues on to the web application component 
        /// responsible for producing the html response. Anything present in the OWIN pipeline following the Authorization Server may produce the
        /// response for the authorization page. If running on IIS any ASP.NET technology running on the server may produce the response for the 
        /// authorization page. If the web application wishes to produce the response directly in the AuthorizationEndpoint call it may write to the 
        /// context.Response directly and should call context.RequestCompleted to stop other handlers from executing. If the web application wishes
        /// to grant the authorization directly in the AuthorizationEndpoint call it cay call context.OwinContext.Authentication.SignIn with the
        /// appropriate ClaimsIdentity and should call context.RequestCompleted to stop other handlers from executing.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task HandleAuthorizationRequest(HandleAuthorizationRequestContext context) => OnHandleAuthorizationRequest(context);

        /// <summary>
        /// Called before the AuthorizationEndpoint redirects its response to the caller.
        /// The response could contain an access token when using implicit flow or
        /// an authorization code when using the authorization code flow.
        /// If the web application wishes to produce the authorization response directly in the AuthorizationEndpoint call it may write to the 
        /// context.Response directly and should call context.RequestCompleted to stop other handlers from executing.
        /// This call may also be used to add additional response parameters to the authorization response.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task ApplyAuthorizationResponse(ApplyAuthorizationResponseContext context) => OnApplyAuthorizationResponse(context);

        /// <summary>
        /// Called at the final stage of an incoming logout endpoint request before the execution continues on to the web application component 
        /// responsible for producing the html response. Anything present in the OWIN pipeline following the Authorization Server may produce the
        /// response for the logout page. If running on IIS any ASP.NET technology running on the server may produce the response for the 
        /// authorization page. If the web application wishes to produce the response directly in the LogoutEndpoint call it may write to the 
        /// context.Response directly and should call context.RequestCompleted to stop other handlers from executing.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task HandleLogoutRequest(HandleLogoutRequestContext context) => OnHandleLogoutRequest(context);

        /// <summary>
        /// Called before the LogoutEndpoint endpoint redirects its response to the caller.
        /// If the web application wishes to produce the authorization response directly in the LogoutEndpoint call it may write to the 
        /// context.Response directly and should call context.RequestCompleted to stop other handlers from executing.
        /// This call may also be used to add additional response parameters to the authorization response.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task ApplyLogoutResponse(ApplyLogoutResponseContext context) => OnApplyLogoutResponse(context);

        /// <summary>
        /// Called at the final stage of an incoming userinfo endpoint request before the execution continues on to the web application component 
        /// responsible for producing the JSON response. Anything present in the OWIN pipeline following the Authorization Server may produce the
        /// response for the userinfo response. If the web application wishes to produce the response directly in the UserinfoEndpoint call it
        /// may write to the context.Response directly and should call context.HandleResponse to stop other handlers from executing.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task HandleUserinfoRequest(HandleUserinfoRequestContext context) => OnHandleUserinfoRequest(context);

        /// <summary>
        /// Called before the UserinfoEndpoint endpoint starts writing to the response stream.
        /// If the web application wishes to produce the userinfo response directly in the UserinfoEndpoint call it may write to the 
        /// context.Response directly and should call context.RequestCompleted to stop other handlers from executing.
        /// This call may also be used to add additional response parameters to the authorization response.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task ApplyUserinfoResponse(ApplyUserinfoResponseContext context) => OnApplyUserinfoResponse(context);

        /// <summary>
        /// Called by the client applications to retrieve the OpenID Connect configuration associated with this instance.
        /// An application may implement this call in order to do any final modification to the configuration metadata.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task HandleConfigurationRequest(HandleConfigurationRequestContext context) => OnHandleConfigurationRequest(context);

        /// <summary>
        /// Called before the authorization server starts emitting the OpenID Connect configuration associated with this instance.
        /// If the web application wishes to produce the configuration metadata directly in this call, it may write to the 
        /// context.Response directly and should call context.RequestCompleted to stop the default behavior from executing.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task ApplyConfigurationResponse(ApplyConfigurationResponseContext context) => OnApplyConfigurationResponse(context);

        /// <summary>
        /// Called by the client applications to retrieve the OpenID Connect JSON Web Key set associated with this instance.
        /// An application may implement this call in order to do any final modification to the keys set.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task HandleCryptographyRequest(HandleCryptographyRequestContext context) => OnHandleCryptographyRequest(context);

        /// <summary>
        /// Called before the authorization server starts emitting the OpenID Connect JSON Web Key set associated with this instance.
        /// If the web application wishes to produce the JSON Web Key set directly in this call, it may write to the 
        /// context.Response directly and should call context.RequestCompleted to stop the default behavior from executing.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task ApplyCryptographyResponse(ApplyCryptographyResponseContext context) => OnApplyCryptographyResponse(context);

        /// <summary>
        /// Called at the final stage of an incoming registration endpoint request before the execution continues on to the web application component 
        /// responsible for producing the response. Anything present in the OWIN pipeline following the Authorization Server may produce the
        /// response for the registration request. If running on IIS any ASP.NET technology running on the server may produce the response for the 
        /// registration request. If the web application wishes to produce the response directly in the HandleRegistrationRequest call it may write to the 
        /// context.Response directly and should call context.RequestCompleted to stop other handlers from executing.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task HandleRegistrationRequest(HandleRegistrationRequestContext context) => OnHandleRegistrationRequest(context);

        /// <summary>
        /// Called before the registration endpoint redirects its response to the caller.
        /// The response could contain the result of a registration, read, update, or delet request.
        /// If the web application wishes to produce the registration response directly in the ApplyRegistrationResponse call it may write to the 
        /// context.Response directly and should call context.RequestCompleted to stop other handlers from executing.
        /// This call may also be used to add additional response parameters to the registration response.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task ApplyRegistrationResponse(ApplyRegistrationResponseContext context) => OnApplyRegistrationResponse(context);

        /// <summary>
        /// Called at the final stage of a successful Token endpoint request.
        /// An application may implement this call in order to do any final 
        /// modification of the claims being used to issue access or refresh tokens. 
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task HandleTokenRequest(HandleTokenRequestContext context) => OnHandleTokenRequest(context);

        /// <summary>
        /// Called before the TokenEndpoint redirects its response to the caller.
        /// This call may also be used in order to add additional 
        /// response parameters to the JSON response payload.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task ApplyTokenResponse(ApplyTokenResponseContext context) => OnApplyTokenResponse(context);

        /// <summary>
        /// Called by applications to determine the status and metadata for a token.
        /// Validation conforms to the OAuth 2.0 Token Introspection specification with some additions. See documentation for details.
        /// An application may implement this call in order to do any final modification to the status and metadata.
        /// </summary>`
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task HandleIntrospectionRequest(HandleIntrospectionRequestContext context) => OnHandleIntrospectionRequest(context);

        /// <summary>
        /// Called before the authorization server starts emitting the status and metadata associated with the token received.
        /// Validation conforms to the OAuth 2.0 Token Introspection specification with some additions. See documentation for details.
        /// If the web application wishes to produce the status and metadata directly in this call, it may write to the 
        /// context.Response directly and should call context.RequestCompleted to stop the default behavior from executing.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task ApplyIntrospectionResponse(ApplyIntrospectionResponseContext context) => OnApplyIntrospectionResponse(context);

        /// <summary>
        /// Called to create a new authorization code. An application may use this context
        /// to replace the authentication ticket before it is serialized or to use its own code store
        /// and skip the default logic using <see cref="BaseNotification{OpenIdConnectServerOptions}.HandleResponse"/>.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task SerializeAuthorizationCode(SerializeAuthorizationCodeContext context) => OnSerializeAuthorizationCode(context);

        /// <summary>
        /// Called to create a new access token. An application may use this context
        /// to replace the authentication ticket before it is serialized or to use its own token format
        /// and skip the default logic using <see cref="BaseNotification{OpenIdConnectServerOptions}.HandleResponse"/>.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task SerializeAccessToken(SerializeAccessTokenContext context) => OnSerializeAccessToken(context);

        /// <summary>
        /// Called to create a new identity token. An application may use this context
        /// to replace the authentication ticket before it is serialized or to use its own token format
        /// and skip the default logic using <see cref="BaseNotification{OpenIdConnectServerOptions}.HandleResponse"/>.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task SerializeIdentityToken(SerializeIdentityTokenContext context) => OnSerializeIdentityToken(context);

        /// <summary>
        /// Called to create a new refresh token. An application may use this context
        /// to replace the authentication ticket before it is serialized or to use its own token format
        /// and skip the default logic using <see cref="BaseNotification{OpenIdConnectServerOptions}.HandleResponse"/>.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task SerializeRefreshToken(SerializeRefreshTokenContext context) => OnSerializeRefreshToken(context);

        /// <summary>
        /// Called when receiving an authorization code. An application may use this context
        /// to deserialize the code using a custom format and to skip the default logic using
        /// <see cref="BaseNotification{OpenIdConnectServerOptions}.HandleResponse"/>.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task DeserializeAuthorizationCode(DeserializeAuthorizationCodeContext context) => OnDeserializeAuthorizationCode(context);

        /// <summary>
        /// Called when receiving an access token. An application may use this context
        /// to deserialize the token using a custom format and to skip the default logic using
        /// <see cref="BaseNotification{OpenIdConnectServerOptions}.HandleResponse"/>.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task DeserializeAccessToken(DeserializeAccessTokenContext context) => OnDeserializeAccessToken(context);

        /// <summary>
        /// Called when receiving an identity token. An application may use this context
        /// to deserialize the token using a custom format and to skip the default logic using
        /// <see cref="BaseNotification{OpenIdConnectServerOptions}.HandleResponse"/>.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task DeserializeIdentityToken(DeserializeIdentityTokenContext context) => OnDeserializeIdentityToken(context);

        /// <summary>
        /// Called when receiving a refresh token. An application may use this context
        /// to deserialize the code using a custom format and to skip the default logic using
        /// <see cref="BaseNotification{OpenIdConnectServerOptions}.HandleResponse"/>.
        /// </summary>
        /// <param name="context">The context of the event carries information in and results out.</param>
        /// <returns>Task to enable asynchronous execution</returns>
        public virtual Task DeserializeRefreshToken(DeserializeRefreshTokenContext context) => OnDeserializeRefreshToken(context);

        public virtual Task CreateClientRegistration(CreateClientRegistrationContext context) => OnCreateClientRegistration(context);
        public virtual Task ReadClientRegistration(ReadClientRegistrationContext context) => OnReadClientRegistration(context);
        public virtual Task UpdateClientRegistration(UpdateClientRegistrationContext context) => OnUpdateClientRegistration(context);
        public virtual Task DeleteClientRegistration(DeleteClientRegistrationContext context) => OnDeleteClientRegistration(context);
    }
}
