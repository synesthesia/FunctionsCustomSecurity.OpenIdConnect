using System;
using Microsoft.Azure.WebJobs.Description;

namespace FunctionsCustomSecurity.OpenIdConnect.Binding
{
    /// <summary>
    /// A custom attribute that lets you pass a <see cref="ClaimsPrincipal"/> into an function definition.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    [Binding]
    public sealed class AccessTokenAttribute : Attribute
    {
    }
}
