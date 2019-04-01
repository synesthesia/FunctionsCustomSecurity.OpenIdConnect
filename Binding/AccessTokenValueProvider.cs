using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.IdentityModel.Tokens;

namespace FunctionsCustomSecurity.OpenIdConnect.Binding
{
    /// <summary>
    /// Creates a <see cref="ClaimsPrincipal"/> instance for the supplied header and configuration values.
    /// </summary>
    /// <remarks>
    /// This is where the actual authentication happens - replace this code to implement a different authentication solution.
    /// </remarks>
    public class AccessTokenValueProvider : IValueProvider
    {
        private const string AUTH_HEADER_NAME = "Authorization";
        private const string BEARER_PREFIX = "Bearer ";
        private HttpRequest _request;
        private readonly IEnumerable<SecurityKey> _signingKeys;
        private readonly string _audience;
        private readonly string _issuer;
        private readonly HttpClient _client;
        private readonly string _userInfoEndPoint;

        public AccessTokenValueProvider(HttpRequest request, IEnumerable<SecurityKey> signingKeys, string audience,
            string issuer, HttpClient client, string userInfoEndPoint)
        {
            _request = request;
            _signingKeys = signingKeys;
            _audience = audience;
            _issuer = issuer;
            _client = client;
            _userInfoEndPoint = userInfoEndPoint;
        }

        public async  Task<object> GetValueAsync()
        {
            // Get the token from the header
            if(_request.Headers.ContainsKey(AUTH_HEADER_NAME) && 
               _request.Headers[AUTH_HEADER_NAME].ToString().StartsWith(BEARER_PREFIX))
            {
                var token = _request.Headers["Authorization"].ToString().Substring(BEARER_PREFIX.Length);

                // Create the parameters
                var tokenParams = new TokenValidationParameters()
                {
                    RequireSignedTokens = true,
                    ValidAudience = _audience,
                    ValidateAudience = true,
                    ValidIssuer = _issuer,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKeys = _signingKeys
                };

                // Validate the token
                var handler = new JwtSecurityTokenHandler();
                var result = handler.ValidateToken(token, tokenParams, out var securityToken);

                // Augment with claims from UserInfo endpoint
                var userInfo = await _client.GetUserInfoAsync(new UserInfoRequest {Address = _userInfoEndPoint, Token = token});
                if (!(result.Identity is ClaimsIdentity identity)) return result;
                var identity2 = new ClaimsIdentity(identity, userInfo.Claims);
                var principal = new ClaimsPrincipal(identity2);
                return principal;
            }
            else
            {
                throw new SecurityTokenException("No access token submitted.");
            }
        }

        public Type Type => typeof(ClaimsPrincipal);

        public string ToInvokeString() => string.Empty;
    }
}
