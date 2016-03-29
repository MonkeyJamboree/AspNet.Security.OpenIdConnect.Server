using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Owin.Security.OpenIdConnect.Server
{
    internal partial class OpenIdConnectServerHandler {
        private async Task InvokeRegistrationEndpointAsync() {
            //TODO: Implement Registration request spec
            // See https://tools.ietf.org/html/rfc7591

            //TODO: Require POST method for Registration requests
            // See https://tools.ietf.org/html/rfc7591#section-3.1
            //  also http://openid.net/specs/openid-connect-registration-1_0.html#RegistrationRequest
            //TODO: Reject requests trying to register a client with an id in the parameter?
            //TODO: Reject non-registration requests that do not have a client_id in the URL or Query parameters.
            //TODO: Require GET method for Read requests
            // See https://tools.ietf.org/html/rfc7592#section-2.1
            //  also http://openid.net/specs/openid-connect-registration-1_0.html#ReadRequest
            //TODO: Require PUT method for Update requests
            // See https://tools.ietf.org/html/rfc7592#section-2.2
            //TODO: Require DELETE method for Delete/Invalidate requests
            // See https://tools.ietf.org/html/rfc7592#section-2.3
            //TODO: Reject HTTP Methods that are not POST, GET, PUT, DELETE.
            //TODO: Call Provider.{method} for each of the types of requests with appropriate params set on context.
            //TODO: Create the four contexts and implement the methods on the default provider.

            //TODO: Optionally accept initial access token
            // See https://tools.ietf.org/html/rfc7591#section-3.1

            //TODO: Allow for delegation of the endpoint to another framework (MVC/WebAPI) or middleware

            //TODO: Accept JSON body in POST, PUT requests
            //TODO: Set values on the RegistrationEndpointContext for the values sent

            // Note: if scope, software_id or software_version are provided, client is using OAuth2 DynReg.
            // Note: if application_type, sector_identifier_uri, subject_type,
            //  id_token_signed_response_alg, id_token_encrypted_response_alg, id_token_encrypted_response_enc,
            //  userinfo_signed_response_alg, userinfo_encrypted_response_alg, userinfo_encrypted_response_enc,
            //  request_object_signing_alg, request_object_encryption_alg, request_object_encryption_enc,
            //  token_endpoint_auth_signing_alg, default_max_age, require_auth_time, default_acr_values,
            //  initiate_login_uri, request_uris are provided, client is using OIDC DynReg.

            //TODO: IN AUTHORIZE ENDPOINT - MUST check that one of these values matches the redirect_uri parameter
            // See http://openid.net/specs/openid-connect-registration-1_0.html#ClientMetadata
            //TODO: Validate response_types.
            //Valid response_types:
            /*      code: authorization code flow (both specs)
                    token: implicit flow (Pure OAuth 2 with access token only)
                    id_token: implicit flow (with identity token only?)
                    id_token token: implicit flow (with both access and identity tokens)
                    code id_token: hybrid flow (auth code and identity token)
                    code token: hybrid flow (auth code and access token (how the hell...?))
                    code id_token token: hybrid flow (auth code, access token and identity token)
            */
            //Default if omitted response_types: code (both specs)

            //TODO: Verify that a client can't register itself in an inconsistent state
            //  -grant_types and response_types must match orthogonally as below:
            /*      grant_type              response_types
                    authorization_code      code, code id_token, code token, code token id_token
                    implicit                token, id_token, id_token token, code id_token, code token, code token id_token
                    password                (none)
                    client_credentials      (none)
                    refresh_token           (none)
                    urn:...:jwt-bearer      (none)
                    urn:...:saml2-bearer    (none)
            */

            //TODO: IN OTHER METHODS - If client was dynamically reg'd,
            //  Check that their grant_type is an allowed (registered) value for the client
            //Valid grant_types:
            /*      authorization_code (both)
                    implicit (both)
                    refresh_token (both)
                    password (OAuth2)
                    client_credentials (OAuth2)
                    refresh_token (OAuth2)
                    urn:ietf:params:oauth:grant-type:jwt-bearer (OAuth2)
                    urn:ietf:params:oauth:grant-type:saml2-bearer (OAuth2)
            */
            //Default if omitted grant_types: authorization_code (both specs)

            //TODO: Validate token_endpoint_auth_method:
            //Valid token_endpoint_auth_method values:
            /*      none (both)
                    client_secret_post (both)
                    client_secret_basic (both)
                    client_secret_jwt (OIDC)
                    private_key_jwt (OIDC)
            */
            //Default if omitted token_endpoint_auth_method: client_secret_basic (both specs)

            //TODO: If client_name is omitted, MAY use client_id. (OAuth2)
            //  IN ALL PLACES - Present registered client_name to End User

            //TODO: IN OTHER ENDPOINTS? MUST (OIDC) point to a valid web page. SHOULD Present client_uri in some clickable fashion to end-users if provided.

            //TODO: IN OTHER ENDPOINTS - MUST point to a valid image file. SHOULD Present the logo image at logo_uri if provided.

            //TODO: scope - If omitted, MAY allow default set of scopes for registration.

            //TODO: Validate contacts - Server MAY make these available for client support requests.
            //  Also, Server might use to enable web modification of Client info.
            // See https://tools.ietf.org/html/rfc7591#section-6

            //TODO: Validate tos_uri - SHOULD display to user if provided. MUST point to valid webpage.
            //TODO: Validate policy_uri - SHOULD display to user if provided. MUST point to valid webpage.

            //TODO: Validate jwks_uri - MUST point to a valid JWK Set document.
            //  Might be used for validating signed requests made to token endpoint when using JWTs for authentication
            //  Preferred over jwks parameter. MUST NOT be provided if jwks is provided.
            //  MUST NOT pass back jwks_uri in response if response has jwks param.

            //TODO: Validate jwks - MUST be a valid JWK Set. Intended for native clients that can't host JWK docs.
            //  MUST NOT be provided if jwks_uri is provided. MUST NOT pass back if jwks_uri is in response.

            //TODO: Validate software_id - Assigned by client dev or publisher.
            //  Validate it's a UUID if present. SHOULD be the same for different instances of software.
            //TODO: software_version - SHOULD change on any update, but what constitutes an update is up to software itself.
            //  Not intended for human readability, usually opaque to the client and auth server.

            //TODO: Validate application_type - native or web. if omitted, web.
            //  Web Clients using Implicit Grant MUST register https URLs for redirect_uris, no localhost in hostname.
            //  Native clients MUST register custom URI schemes or http with localhost host name for redirect_uris.
            //  Auth servers MAY place additional constraints on native clients.
            //  Auth servers MAY reject http other than localhost for native clients.
            //  Auth server MUST verify all redirect_uris conform to constraints.

            //TODO: Validate jwks and doc at jwks_uri
            //  See http://openid.net/specs/openid-connect-registration-1_0.html#ClientMetadata
            //  Also https://tools.ietf.org/html/rfc7517

            //TODO: If sector_identifier_uri is sent, retrieve and validate the JSON document
            //  Url using https scheme, if pairwise sub values SHOULD use this in Subject Identifer calculation.

            //TODO: Validate subject_type - pairwise or public.
            //  subject_types_supported Discovery endpoint param should have list of supported values for server.

            //TODO: Validate id_token_signed_response_alg - "alg" for signing id tokens for this client.
            //  none MUST NOT be used unless client doesn't have id_token response type. Default if omitted - RS256
            //  public key for validating sig retrieved from jwks_uri in Discovery.

            //TODO: Validate id_token_encrypted_response_alg - "alg" for encrypting id tokens for this client.
            //  If provided, response will be signed then encrypted as nested JWT. Default if omitted - no encryption.

            //TODO: Validate id_token_encrypted_response_enc - "enc" for encrypting id tokens for this client.
            //  If id_token_encrypted_response_alg is given, this default is A128CBC-HS256.
            //  If id_token_encrypted_response_enc is given, id_token_encrypted_response_alg MUST be provided.

            //TODO: Validate userinfo_signed_response_alg - "alg" for signing userinfo responses for this client.
            //  If given, response will be JWT serialized and signed using JWS. Default if omitted is for userinfo to
            //  return application/json UTF-8 content-type JSON object.

            //TODO: Validate userinfo_encrypted_response_alg - "alg" for encrypting userinfo responses for this client.
            //  If both this and userinfo_signed_response_alg are provided, response will be signed then encrypted as nested JWT.
            //  Default if omitted - no encryption performed.

            //TODO: Validate userinfo_encrypted_response_enc - "enc" for encrypting userinfo responses for this client.
            //  If userinfo_encrypted_response_alg is given, default is A128CBC-HS256.
            //  If userinfo_encrypted_response_enc is given, userinfo_encrypted_response_alg MUST be provided.

            //TODO: Validate request_object_signing_alg - "alg" for signing Request Objects sent to AS/OP.
            //  All Request Objects from client that aren't signed with the alg MUST be rejected.
            //  This alg MUST be used for both "request" params and objects referenced by "request_uri" params.
            //  Servers SHOULD support RS256. "none" MAY be used. Default if omitted - any alg supported by OP and RP MAY be used.

            //TODO: Validate request_object_encryption_alg - "alg" the RP may use for encrypting Request Objects.
            //  SHOULD be included when symmetric encryption is used so OP derives symmetric key from client_secret.
            //  RP MAY use other supported enc algs or send unencrypted Request Objects even when this param is provided.
            //  If both request_object_signing_alg and this are provided, Request Objects will be signed then encrypted
            //  as nested JWT. if omitted, RP isn't saying if it'll encrypt Request Objects.

            //TODO: Validate request_object_encryption_enc - "enc" the RP may use for encrypting Request Objects.
            //  If request_object_encryption_alg is given, default is A128CBC-HS256.
            //  When this is given, request_object_encryption_alg MUST be provided.

            //TODO: Validate token_endpoint_signing_alg - "alg" used for signing JWT used for Token endpoint calls.
            //  Supported for private_key_jwt and client_secret_jwt authentication methods.
            //  Token requests MUST be rejected if JWT is not signed with this alg.
            //  Servers SHOULD support RS256. "none" MUST NOT be used. Default if omitted is that any alg
            //  supported by OP and RP MAY be used.

            //TODO: Validate default_max_age - Says the user MUST be actively authenticated if user was authenticated
            //  longer than X seconds ago. max_age overrides this default value. Default if omitted: no max age specified.

            //TODO: Validate require_auth_time - Boolean saying if auth_time claim in id_token is REQUIRED.
            //  Default if omitted: false

            //TODO: Validate default_acr_values
            /*  OPTIONAL. Default requested Authentication Context Class Reference values. Array of strings that specifies
                the default acr values that the OP is being requested to use for processing requests from this Client, with
                the values appearing in order of preference. The Authentication Context Class satisfied by the authentication
                performed is returned as the acr Claim Value in the issued ID Token. The acr Claim is requested as a Voluntary
                Claim by this parameter. The acr_values_supported discovery element contains a list of the supported acr values
                supported by this server. Values specified in the acr_values request parameter or an individual acr Claim
                request override these default values.
            */

            //TODO: Validate initiate_login_uri - Uses https scheme
            //  (Mostly client specs noted otherwise about MUST understand login_hint and iss, and SHOULD target_link_uri)
            //  (Location at URI MUST accept GET and POST)

            //TODO: Validate request_uris - OPs can require that request_uri vals be preregistered with
            //  the require_request_uri_registration discovery param.
            //  Servers MAY cache contents of files referenced by these URIs instead of pulling them when used in request.
            //  If the contents of the files could change, the URI values SHOULD include base64url SHA-256 hash values of the
            //  file contents referenced. If the fragment value changes, that tells the server that the cached doc is invalid.

            //TODO: Add support for multiple languages for member names and values for params such as:
            //  client_name, tos_uri, policy_uri, logo_uri, client_uri etc.

            //TODO: SHOULD support open registration without Initial Access Tokens.
            //TODO: Allow sending of Initial Access Tokens provisioned out of band to limit only authorized clients/devs

            //TODO: Provide rate-limiting on endpoint?

            //TODO: Optionally accept a software statement in the request (Requires JWS support)
            // Note: no mention of software statements in the OIDC DynReg spec.
            // See https://tools.ietf.org/html/rfc7591#section-2.3
            //  also https://tools.ietf.org/html/rfc7591#section-3.1.1
            //TODO: Use software statement values before explicit reg values            

            //TODO: Add JWKS support (Outside of scope?)

            //TODO: Generate client_id and (optional) client_secret
            //TODO: Possibly generate a Registration Access Token for subsequent Reg endpoint calls.
            //TODO: Add registration_access_token to response if generated.
            //TODO: Add registration_client_uri to response. RETURN BOTH registration_access_token AND
            //  registration_client_uri OR NEITHER.
            //TODO: Add client_id_issued_at to response as JSON seconds from 1970-01-01T0:0:0Z in UTC.
            //TODO: Add client_secret_expires_at to response, REQUIRED if client_secret is issued.
            //  0 if no expiration, seconds from 1970-01-01T0:0:0Z in UTC until it expires.
            //TODO: Add all parameters used in registration to response

            //TODO: Write responses (201 CREATED, application/json)

            //ERROR DESCRIPTIONS:
            /*  InvalidRedirectUri: "The value of one or more redirection URIs is invalid."
                InvalidClientMetadata: "The value of one of the client metadata fields is invalid and the" +
                    "server has rejected this request. Note that an authorization" +
                    "server MAY choose to substitute a valid value for any requested" +
                    "parameter of a client's metadata."
                InvalidSoftwareStatement: "The software statement presented is invalid."
                UnapprovedSoftwareStatement: "The software statement presented is not approved for use by this" +
                    "authorization server."
            */

            //TODO: Write all metadata used to register client
            //  Including: metadata passed to endpoint, default values used on AS
            throw new NotImplementedException();

        }
    }
}
