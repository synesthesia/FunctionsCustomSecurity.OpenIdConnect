# OpenId Connect security binding for Azure Functions

Adapted from https://github.com/BenMorris/FunctionsCustomSercuity

Example of how to apply JWT security to API endpoints running in Azure Functions, including automatic refresh of signing keys at Function startup

Requires following application settings / local settings

|Setting|Value|
|-|-|
|Issuer|The authority URI of the IdP|
|Audience|The audience for which tokens are issued|


# Original README below

# Custom security binding for Azure Functions

This is a basic sample demonstrating how to use a custom input binding to support custom authentication for Azure Functions.

This sample allows a ClaimsPrincipal to be injected into function methods that has been validated against an externally-issued OAuth 2.0 access token.

A full explanation of the implementation is available on my blog here: https://www.ben-morris.com/custom-token-authentication-in-azure-functions/

