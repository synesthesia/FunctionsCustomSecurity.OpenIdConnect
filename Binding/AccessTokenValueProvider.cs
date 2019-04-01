﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
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

        public AccessTokenValueProvider(HttpRequest request, IEnumerable<SecurityKey> signingKeys, string audience, string issuer)
        {
            _request = request;
            _signingKeys = signingKeys;
            _audience = audience;
            _issuer = issuer;
        }

        public Task<object> GetValueAsync()
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
                return Task.FromResult<object>(result);
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
