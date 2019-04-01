using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Host.Config;

namespace FunctionsCustomSecurity.OpenIdConnect.Binding
{
    /// <summary>
    /// Wires up the attribute to the custom binding.
    /// </summary>
    public class AccessTokenExtensionProvider : IExtensionConfigProvider
    {
        private static HttpClient _client;
        public void Initialize(ExtensionConfigContext context)
        {
            var issuer = Environment.GetEnvironmentVariable("Issuer");
            if (string.IsNullOrWhiteSpace(issuer))
            {
                throw new ArgumentNullException("Issuer", "Must define token issuer url in settings");
            }
            _client = _client ?? new HttpClient{BaseAddress = new Uri(issuer) };

            // Creates a rule that links the attribute to the binding
            var provider = new AccessTokenBindingProvider(_client);
            var rule = context.AddBindingRule<AccessTokenAttribute>().Bind(provider);
        }
    }
}
