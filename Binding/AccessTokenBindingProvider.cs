using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.IdentityModel.Tokens;

namespace FunctionsCustomSecurity.OpenIdConnect.Binding
{
    /// <summary>
    /// Provides a new binding instance for the function host.
    /// </summary>
    public class AccessTokenBindingProvider : IBindingProvider
    {
        private readonly HttpClient _client;

        public AccessTokenBindingProvider(HttpClient client)
        {
            _client = client;
        }

        public async Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {

            var disco = await _client.GetDiscoveryDocumentAsync();
            var keys = (
                from webKey in disco.KeySet.Keys
                let e = Base64Url.Decode(webKey.E)
                let n = Base64Url.Decode(webKey.N)
                select new RsaSecurityKey(new RSAParameters {Exponent = e, Modulus = n}) {KeyId = webKey.Kid})
                .Cast<SecurityKey>().ToList();

            IBinding binding = new AccessTokenBinding(keys, disco.UserInfoEndpoint, _client);
            return binding;
        }
    }

}
