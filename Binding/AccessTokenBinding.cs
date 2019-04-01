using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.IdentityModel.Tokens;

namespace FunctionsCustomSecurity.OpenIdConnect.Binding
{
    /// <summary>
    /// Runs on every request and passes the function context (e.g. Http request and host configuration) to a value provider.
    /// </summary>
    public class AccessTokenBinding : IBinding
    {
        private readonly List<SecurityKey> _keys;
        private readonly string _userInfoEndPoint;
        private readonly HttpClient _client;

        public AccessTokenBinding(List<SecurityKey> keys, string userInfoEndPoint, HttpClient client)
        {
            _keys = keys;
            _userInfoEndPoint = userInfoEndPoint;
            _client = client;
        }

        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            // Get the HTTP request
            var request = context.BindingData["req"] as DefaultHttpRequest;

            // Get the configuration files for the OAuth token issuer
            var audience = Environment.GetEnvironmentVariable("Audience");
            var issuer = Environment.GetEnvironmentVariable("Issuer");

            return Task.FromResult<IValueProvider>(new AccessTokenValueProvider(request, _keys, audience, issuer, _client, _userInfoEndPoint));
        }

        public bool FromAttribute => true;

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context) => null;

        public ParameterDescriptor ToParameterDescriptor() => new ParameterDescriptor();
    }
}
