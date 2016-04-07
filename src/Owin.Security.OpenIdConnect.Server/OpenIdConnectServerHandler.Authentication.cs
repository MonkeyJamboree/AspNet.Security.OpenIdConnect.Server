/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OpenIdConnect.Server
 * for more information concerning the license and the contributors participating to this project.
 */

using System;
using System.Globalization;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Owin.Security.OpenIdConnect.Extensions;

namespace Owin.Security.OpenIdConnect.Server {
    internal partial class OpenIdConnectServerHandler : AuthenticationHandler<OpenIdConnectServerOptions> {
        private async Task<bool> InvokeAuthorizationEndpointAsync() {
            OpenIdConnectMessage request;

            if (Request.HasMethod(OpenIdConnectConstants.HttpMethods.Get)) {
                // Create a new authorization request using the
                // parameters retrieved from the query string.
                request = new OpenIdConnectMessage(Request.Query) {
                    RequestType = OpenIdConnectRequestType.AuthenticationRequest
                };
            }

            else if (Request.HasMethod(OpenIdConnectConstants.HttpMethods.Post)) {
                // See http://openid.net/specs/openid-connect-core-1_0.html#FormSerialization
                if (string.IsNullOrEmpty(Request.ContentType)) {
                    Options.Logger.WriteInformation("A malformed request has been received by the authorization endpoint.");

                    return await SendErrorPageAsync(new OpenIdConnectMessage {
                        Error = OpenIdConnectConstants.Errors.InvalidRequest,
                        ErrorDescription = "A malformed authorization request has been received: " +
                            "the mandatory 'Content-Type' header was missing from the POST request."
                    });
                }

                // May have media/type; charset=utf-8, allow partial match.
                if (!Request.HasContentType(OpenIdConnectConstants.ContentTypes.ApplicationXWwwFormUrlEncoded)) {
                    Options.Logger.WriteInformation("A malformed request has been received by the authorization endpoint.");

                    return await SendErrorPageAsync(new OpenIdConnectMessage {
                        Error = OpenIdConnectConstants.Errors.InvalidRequest,
                        ErrorDescription = "A malformed authorization request has been received: " +
                            "the 'Content-Type' header contained an unexcepted value. " +
                            "Make sure to use 'application/x-www-form-urlencoded'."
                    });
                }

                // Create a new authorization request using the
                // parameters retrieved from the request form.
                request = new OpenIdConnectMessage(await Request.ReadFormAsync()) {
                    RequestType = OpenIdConnectRequestType.AuthenticationRequest
                };
            }

            else {
                Options.Logger.WriteInformation("A malformed request has been received by the authorization endpoint.");

                return await SendErrorPageAsync(new OpenIdConnectMessage {
                    Error = OpenIdConnectConstants.Errors.InvalidRequest,
                    ErrorDescription = "A malformed authorization request has been received: " +
                                       "make sure to use either GET or POST."
                });
            }

            // Re-assemble the authorization request using the distributed cache if
            // a 'unique_id' parameter has been extracted from the received message.
            var identifier = request.GetRequestId();
            if (!string.IsNullOrEmpty(identifier)) {
                var buffer = await Options.Cache.GetAsync($"asos-request:{identifier}");
                if (buffer == null) {
                    Options.Logger.WriteInformation("A unique_id has been provided but no corresponding " +
                                                    "OpenID Connect request has been found in the cache.");

                    return await SendErrorPageAsync(new OpenIdConnectMessage {
                        Error = OpenIdConnectConstants.Errors.InvalidRequest,
                        ErrorDescription = "Invalid request: timeout expired."
                    });
                }

                using (var stream = new MemoryStream(buffer))
                using (var reader = new BinaryReader(stream)) {
                    // Make sure the stored authorization request
                    // has been serialized using the same method.
                    var version = reader.ReadInt32();
                    if (version != 1) {
                        await Options.Cache.RemoveAsync($"asos-request:{identifier}");

                        Options.Logger.WriteError("An invalid OpenID Connect request has been found in the cache.");

                        return await SendErrorPageAsync(new OpenIdConnectMessage {
                            Error = OpenIdConnectConstants.Errors.InvalidRequest,
                            ErrorDescription = "Invalid request: timeout expired."
                        });
                    }

                    for (int index = 0, length = reader.ReadInt32(); index < length; index++) {
                        var name = reader.ReadString();
                        var value = reader.ReadString();

                        // Skip restoring the parameter retrieved from the stored request
                        // if the OpenID Connect message extracted from the query string
                        // or the request form defined the same parameter.
                        if (!request.Parameters.ContainsKey(name)) {
                            request.SetParameter(name, value);
                        }
                    }
                }
            }
            
            // Store the authorization request in the OWIN context.
            Context.SetOpenIdConnectRequest(request);

            // client_id is mandatory parameter and MUST cause an error when missing.
            // See http://openid.net/specs/openid-connect-core-1_0.html#AuthRequest
            if (string.IsNullOrEmpty(request.ClientId)) {
                return await SendErrorPageAsync(new OpenIdConnectMessage {
                    Error = OpenIdConnectConstants.Errors.InvalidRequest,
                    ErrorDescription = "client_id was missing"
                });
            }

            // While redirect_uri was not mandatory in OAuth2, this parameter
            // is now declared as REQUIRED and MUST cause an error when missing.
            // See http://openid.net/specs/openid-connect-core-1_0.html#AuthRequest
            // To keep AspNet.Security.OpenIdConnect.Server compatible with pure OAuth2 clients,
            // an error is only returned if the request was made by an OpenID Connect client.
            if (string.IsNullOrEmpty(request.RedirectUri) && request.HasScope(OpenIdConnectConstants.Scopes.OpenId)) {
                return await SendErrorPageAsync(new OpenIdConnectMessage {
                    Error = OpenIdConnectConstants.Errors.InvalidRequest,
                    ErrorDescription = "redirect_uri must be included when making an OpenID Connect request"
                });
            }

            if (!string.IsNullOrEmpty(request.RedirectUri)) {
                // Note: when specified, redirect_uri MUST be an absolute URI.
                // See http://tools.ietf.org/html/rfc6749#section-3.1.2
                // and http://openid.net/specs/openid-connect-core-1_0.html#AuthRequest
                Uri uri;
                if (!Uri.TryCreate(request.RedirectUri, UriKind.Absolute, out uri)) {
                    return await SendErrorPageAsync(new OpenIdConnectMessage {
                        Error = OpenIdConnectConstants.Errors.InvalidRequest,
                        ErrorDescription = "redirect_uri must be absolute"
                    });
                }

                // Note: when specified, redirect_uri MUST NOT include a fragment component.
                // See http://tools.ietf.org/html/rfc6749#section-3.1.2
                // and http://openid.net/specs/openid-connect-core-1_0.html#AuthRequest
                else if (!string.IsNullOrEmpty(uri.Fragment)) {
                    return await SendErrorPageAsync(new OpenIdConnectMessage {
                        Error = OpenIdConnectConstants.Errors.InvalidRequest,
                        ErrorDescription = "redirect_uri must not include a fragment"
                    });
                }
            }

            // Reject requests using the unsupported request parameter.
            if (!string.IsNullOrEmpty(request.GetParameter(OpenIdConnectConstants.Parameters.Request))) {
                Options.Logger.WriteVerbose("The authorization request contained the unsupported request parameter.");

                return await SendErrorPageAsync(new OpenIdConnectMessage {
                    Error = OpenIdConnectConstants.Errors.RequestNotSupported,
                    ErrorDescription = "The request parameter is not supported."
                });
            }

            // Reject requests using the unsupported request_uri parameter.
            else if (!string.IsNullOrEmpty(request.RequestUri)) {
                Options.Logger.WriteVerbose("The authorization request contained the unsupported request_uri parameter.");

                return await SendErrorPageAsync(new OpenIdConnectMessage {
                    Error = OpenIdConnectConstants.Errors.RequestUriNotSupported,
                    ErrorDescription = "The request_uri parameter is not supported."
                });
            }

            // Reject requests missing the mandatory response_type parameter.
            else if (string.IsNullOrEmpty(request.ResponseType)) {
                Options.Logger.WriteVerbose("Authorization request missing required response_type parameter");

                return await SendErrorPageAsync(new OpenIdConnectMessage {
                    Error = OpenIdConnectConstants.Errors.InvalidRequest,
                    ErrorDescription = "response_type parameter missing"
                });
            }

            // Reject requests whose response_type parameter is unsupported.
            else if (!request.IsNoneFlow() && !request.IsAuthorizationCodeFlow() &&
                     !request.IsImplicitFlow() && !request.IsHybridFlow()) {
                Options.Logger.WriteVerbose("Authorization request contains unsupported response_type parameter");

                return await SendErrorPageAsync(new OpenIdConnectMessage {
                    Error = OpenIdConnectConstants.Errors.UnsupportedResponseType,
                    ErrorDescription = "response_type unsupported"
                });
            }

            // Reject requests whose response_mode is unsupported.
            else if (!request.IsFormPostResponseMode() && !request.IsFragmentResponseMode() && !request.IsQueryResponseMode()) {
                Options.Logger.WriteVerbose("Authorization request contains unsupported response_mode parameter");

                return await SendErrorPageAsync(new OpenIdConnectMessage {
                    Error = OpenIdConnectConstants.Errors.InvalidRequest,
                    ErrorDescription = "response_mode unsupported"
                });
            }

            // response_mode=query (explicit or not) and a response_type containing id_token
            // or token are not considered as a safe combination and MUST be rejected.
            // See http://openid.net/specs/oauth-v2-multiple-response-types-1_0.html#Security
            else if (request.IsQueryResponseMode() && (request.HasResponseType(OpenIdConnectConstants.ResponseTypes.IdToken) ||
                                                       request.HasResponseType(OpenIdConnectConstants.ResponseTypes.Token))) {
                Options.Logger.WriteVerbose("Authorization request contains unsafe response_type/response_mode combination");

                return await SendErrorPageAsync(new OpenIdConnectMessage {
                    Error = OpenIdConnectConstants.Errors.InvalidRequest,
                    ErrorDescription = "response_type/response_mode combination unsupported"
                });
            }

            // Reject OpenID Connect implicit/hybrid requests missing the mandatory nonce parameter.
            // See http://openid.net/specs/openid-connect-core-1_0.html#AuthRequest,
            // http://openid.net/specs/openid-connect-implicit-1_0.html#RequestParameters
            // and http://openid.net/specs/openid-connect-core-1_0.html#HybridIDToken.
            else if (string.IsNullOrEmpty(request.Nonce) && request.HasScope(OpenIdConnectConstants.Scopes.OpenId) &&
                                                           (request.IsImplicitFlow() || request.IsHybridFlow())) {
                Options.Logger.WriteVerbose("The 'nonce' parameter was missing");

                return await SendErrorPageAsync(new OpenIdConnectMessage {
                    Error = OpenIdConnectConstants.Errors.InvalidRequest,
                    ErrorDescription = "nonce parameter missing"
                });
            }

            // Reject requests containing the id_token response_mode if no openid scope has been received.
            else if (request.HasResponseType(OpenIdConnectConstants.ResponseTypes.IdToken) &&
                    !request.HasScope(OpenIdConnectConstants.Scopes.OpenId)) {
                Options.Logger.WriteVerbose("The 'openid' scope part was missing");

                return await SendErrorPageAsync(new OpenIdConnectMessage {
                    Error = OpenIdConnectConstants.Errors.InvalidRequest,
                    ErrorDescription = "openid scope missing"
                });
            }

            // Reject requests containing the code response_mode if the token endpoint has been disabled.
            else if (request.HasResponseType(OpenIdConnectConstants.ResponseTypes.Code) && !Options.TokenEndpointPath.HasValue) {
                Options.Logger.WriteVerbose("Authorization request contains the disabled code response_type");

                return await SendErrorPageAsync(new OpenIdConnectMessage {
                    Error = OpenIdConnectConstants.Errors.UnsupportedResponseType,
                    ErrorDescription = "response_type=code is not supported by this server"
                });
            }

            var validatingContext = new ValidateAuthorizationRequestContext(Context, Options, request);
            await Options.Provider.ValidateAuthorizationRequest(validatingContext);

            // Stop processing the request if Validated was not called.
            if (!validatingContext.IsValidated) {
                Options.Logger.WriteError("The authorization request was rejected.");

                return await SendErrorPageAsync(new OpenIdConnectMessage {
                    Error = validatingContext.Error ?? OpenIdConnectConstants.Errors.InvalidRequest,
                    ErrorDescription = validatingContext.ErrorDescription,
                    ErrorUri = validatingContext.ErrorUri
                });
            }

            identifier = request.GetRequestId();
            if (string.IsNullOrEmpty(identifier)) {
                // Generate a new 256-bits identifier and associate it with the authorization request.
                identifier = Options.RandomNumberGenerator.GenerateKey(length: 256 / 8);
                request.SetRequestId(identifier);

                using (var stream = new MemoryStream())
                using (var writer = new BinaryWriter(stream)) {
                    writer.Write(/* version: */ 1);
                    writer.Write(request.Parameters.Count);

                    foreach (var parameter in request.Parameters) {
                        writer.Write(parameter.Key);
                        writer.Write(parameter.Value);
                    }

                    // Serialize the authorization request.
                    var bytes = stream.ToArray();

                    // Store the authorization request in the distributed cache.
                    await Options.Cache.SetAsync($"asos-request:{identifier}", bytes, new DistributedCacheEntryOptions {
                        AbsoluteExpiration = Options.SystemClock.UtcNow + TimeSpan.FromHours(1)
                    });
                }
            }

            var notification = new HandleAuthorizationRequestContext(Context, Options, request);
            await Options.Provider.HandleAuthorizationRequest(notification);

            if (notification.HandledResponse) {
                return true;
            }

            return false;
        }

        private async Task<bool> HandleAuthorizationResponseAsync() {
            // request may be null when no authorization request has been received
            // or has been already handled by InvokeAuthorizationEndpointAsync.
            var request = Context.GetOpenIdConnectRequest();
            if (request == null) {
                return false;
            }

            // Stop processing the request if there's no response grant that matches
            // the authentication type associated with this middleware instance
            // or if the response status code doesn't indicate a successful response.
            var context = Helper.LookupSignIn(Options.AuthenticationType);
            if (context == null || Response.StatusCode != 200) {
                return false;
            }

            if (Context.Environment.ContainsKey("app.HeadersSent")) {
                Options.Logger.WriteCritical(
                    "OpenIdConnectServerHandler.TeardownCoreAsync cannot be called when " +
                    "the response headers have already been sent back to the user agent. " +
                    "Make sure the response body has not been altered and that no middleware " +
                    "has attempted to write to the response stream during this request.");

                return true;
            }

            if (!context.Principal.HasClaim(claim => claim.Type == ClaimTypes.NameIdentifier)) {
                Options.Logger.WriteError("The returned identity doesn't contain the mandatory ClaimTypes.NameIdentifier claim.");

                await SendNativeErrorPageAsync(new OpenIdConnectMessage {
                    Error = OpenIdConnectConstants.Errors.ServerError,
                    ErrorDescription = "The mandatory ClaimTypes.NameIdentifier claim was not found."
                });

                return true;
            }

            // redirect_uri is added to the response message since it's not a mandatory parameter
            // in OAuth 2.0 and can be set or replaced from the ValidateClientRedirectUri event.
            var response = new OpenIdConnectMessage {
                RedirectUri = request.RedirectUri,
                State = request.State
            };

            if (!string.IsNullOrEmpty(request.Nonce)) {
                // Keep the original nonce parameter for later comparison.
                context.Properties.Dictionary[OpenIdConnectConstants.Properties.Nonce] = request.Nonce;
            }

            if (!string.IsNullOrEmpty(request.RedirectUri)) {
                // Keep original the original redirect_uri for later comparison.
                context.Properties.Dictionary[OpenIdConnectConstants.Properties.RedirectUri] = request.RedirectUri;
            }

            // Always include the "openid" scope when the developer doesn't explicitly call SetScopes.
            // Note: the application is allowed to specify a different "scopes"
            // parameter when calling AuthenticationManager.SignInAsync: in this case,
            // don't replace the "scopes" property stored in the authentication ticket.
            if (!context.Properties.Dictionary.ContainsKey(OpenIdConnectConstants.Properties.Scopes) &&
                 request.HasScope(OpenIdConnectConstants.Scopes.OpenId)) {
                context.Properties.Dictionary[OpenIdConnectConstants.Properties.Scopes] = OpenIdConnectConstants.Scopes.OpenId;
            }

            string audiences;
            // When a "resources" property cannot be found in the authentication properties, infer it from the "audiences" property.
            if (!context.Properties.Dictionary.ContainsKey(OpenIdConnectConstants.Properties.Resources) &&
                 context.Properties.Dictionary.TryGetValue(OpenIdConnectConstants.Properties.Audiences, out audiences)) {
                context.Properties.Dictionary[OpenIdConnectConstants.Properties.Resources] = audiences;
            }

            // Determine whether an authorization code should be returned
            // and invoke SerializeAuthorizationCodeAsync if necessary.
            if (request.HasResponseType(OpenIdConnectConstants.ResponseTypes.Code)) {
                // Make sure to create a copy of the authentication properties
                // to avoid modifying the properties set on the original ticket.
                var properties = context.Properties.Copy();

                // properties.IssuedUtc and properties.ExpiresUtc are always
                // explicitly set to null to avoid aligning the expiration date
                // of the authorization code with the lifetime of the other tokens.
                properties.IssuedUtc = properties.ExpiresUtc = null;

                response.Code = await SerializeAuthorizationCodeAsync(context.Identity, properties, request, response);

                // Ensure that an authorization code is issued to avoid returning an invalid response.
                // See http://openid.net/specs/oauth-v2-multiple-response-types-1_0.html#Combinations
                if (string.IsNullOrEmpty(response.Code)) {
                    Options.Logger.WriteError("SerializeAuthorizationCodeAsync returned no authorization code");

                    await SendNativeErrorPageAsync(new OpenIdConnectMessage {
                        Error = OpenIdConnectConstants.Errors.ServerError,
                        ErrorDescription = "no valid authorization code was issued"
                    });

                    return true;
                }
            }

            // Determine whether an access token should be returned
            // and invoke SerializeAccessTokenAsync if necessary.
            if (request.HasResponseType(OpenIdConnectConstants.ResponseTypes.Token)) {
                // Make sure to create a copy of the authentication properties
                // to avoid modifying the properties set on the original ticket.
                var properties = context.Properties.Copy();

                string resources;
                if (!properties.Dictionary.TryGetValue(OpenIdConnectConstants.Properties.Resources, out resources)) {
                    Options.Logger.WriteInformation("No explicit resource has been associated with the authentication ticket: " +
                                                    "the access token will thus be issued without any audience attached.");
                }

                // Note: when the "resource" parameter added to the OpenID Connect response
                // is identical to the request parameter, setting it is not necessary.
                if (!string.IsNullOrEmpty(request.Resource) &&
                    !string.Equals(request.Resource, resources, StringComparison.Ordinal)) {
                    response.Resource = resources;
                }

                // Note: when the "scope" parameter added to the OpenID Connect response
                // is identical to the request parameter, setting it is not necessary.
                string scopes;
                properties.Dictionary.TryGetValue(OpenIdConnectConstants.Properties.Scopes, out scopes);
                if (!string.IsNullOrEmpty(request.Scope) &&
                    !string.Equals(request.Scope, scopes, StringComparison.Ordinal)) {
                    response.Scope = scopes;
                }

                response.TokenType = OpenIdConnectConstants.TokenTypes.Bearer;
                response.AccessToken = await SerializeAccessTokenAsync(context.Identity, properties, request, response);

                // Ensure that an access token is issued to avoid returning an invalid response.
                // See http://openid.net/specs/oauth-v2-multiple-response-types-1_0.html#Combinations
                if (string.IsNullOrEmpty(response.AccessToken)) {
                    Options.Logger.WriteError("SerializeAccessTokenAsync returned no access token.");

                    await SendNativeErrorPageAsync(new OpenIdConnectMessage {
                        Error = OpenIdConnectConstants.Errors.ServerError,
                        ErrorDescription = "no valid access token was issued"
                    });

                    return true;
                }

                // properties.ExpiresUtc is automatically set by SerializeAccessTokenAsync but the end user
                // is free to set a null value directly in the SerializeAccessToken event.
                if (properties.ExpiresUtc.HasValue && properties.ExpiresUtc > Options.SystemClock.UtcNow) {
                    var lifetime = properties.ExpiresUtc.Value - Options.SystemClock.UtcNow;
                    var expiration = (long) (lifetime.TotalSeconds + .5);

                    response.ExpiresIn = expiration.ToString(CultureInfo.InvariantCulture);
                }
            }

            // Determine whether an identity token should be returned
            // and invoke SerializeIdentityTokenAsync if necessary.
            if (request.HasResponseType(OpenIdConnectConstants.ResponseTypes.IdToken)) {
                // Make sure to create a copy of the authentication properties
                // to avoid modifying the properties set on the original ticket.
                var properties = context.Properties.Copy();

                response.IdToken = await SerializeIdentityTokenAsync(context.Identity, properties, request, response);

                // Ensure that an identity token is issued to avoid returning an invalid response.
                // See http://openid.net/specs/oauth-v2-multiple-response-types-1_0.html#Combinations
                if (string.IsNullOrEmpty(response.IdToken)) {
                    Options.Logger.WriteError("SerializeIdentityTokenAsync returned no identity token.");

                    await SendNativeErrorPageAsync(new OpenIdConnectMessage {
                        Error = OpenIdConnectConstants.Errors.ServerError,
                        ErrorDescription = "no valid identity token was issued"
                    });

                    return true;
                }
            }

            // Remove the OpenID Connect request from the cache.
            var identifier = request.GetRequestId();
            if (!string.IsNullOrEmpty(identifier)) {
                Options.Cache.Remove(identifier);
            }

            var ticket = new AuthenticationTicket(context.Identity, context.Properties);

            var notification = new ApplyAuthorizationResponseContext(Context, Options, ticket, request, response);
            await Options.Provider.ApplyAuthorizationResponse(notification);

            if (notification.HandledResponse) {
                return true;
            }

            return await ApplyAuthorizationResponseAsync(request, response);
        }

        private async Task<bool> HandleForbiddenResponseAsync() {
            // Stop processing the request if no OpenID Connect
            // message has been found in the current context.
            var request = Context.GetOpenIdConnectRequest();
            if (request == null) {
                return false;
            }

            // Stop processing the request if there's no challenge that matches
            // the authentication type associated with this middleware instance
            // or if the response status code doesn't indicate a challenge operation.
            var context = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);
            if (context == null || Response.StatusCode != 403) {
                return false;
            }

            return await SendErrorRedirectAsync(request, new OpenIdConnectMessage {
                Error = OpenIdConnectConstants.Errors.AccessDenied,
                ErrorDescription = "The authorization grant has been denied by the resource owner",
                RedirectUri = request.RedirectUri,
                State = request.State
            });
        }
    }
}
