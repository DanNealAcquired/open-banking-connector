﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.AuthContext;

[ApiController]
[ApiExplorerSettings(GroupName = "auth-contexts")]
[Tags("Post-Auth Redirects")]
[Route("auth")]
public class RedirectsController : ControllerBase
{
    private readonly IRequestBuilder _requestBuilder;

    public RedirectsController(IRequestBuilder requestBuilder)
    {
        _requestBuilder = requestBuilder;
    }

    /// <summary>
    ///     Delegate endpoint for forwarding data captured elsewhere from post-auth (OAuth 2/OpenID Connect) redirect
    /// </summary>
    /// <param name="idToken">ID token provided by post-auth redirect</param>
    /// <param name="code">code provided by post-auth redirect</param>
    /// <param name="state">state provided by post-auth redirect</param>
    /// <param name="error"></param>
    /// <param name="responseMode">response mode used by post-auth redirect (for checking), one of {"query", "fragment"}</param>
    /// <param name="modifiedBy">user or comment for any database updates</param>
    /// <param name="redirectUrl">redirect URL used by post-auth redirect (for checking)</param>
    /// <param name="appSessionId">app session ID associated with post-auth redirect (for checking)</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [HttpPost("redirect-delegate")]
    [Consumes("application/x-www-form-urlencoded")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<AuthContextUpdateAuthResultResponse>> RedirectDelegatePostAsync(
        [Required] [FromForm(Name = "state")]
        string state,
        [FromForm(Name = "code")]
        string? code,
        [FromForm(Name = "id_token")]
        string? idToken,
        [FromForm(Name = "error")]
        string? error,
        [FromForm(Name = "response_mode")]
        string? responseMode,
        [FromForm(Name = "modified_by")]
        string? modifiedBy,
        [FromForm(Name = "redirect_uri")]
        string? redirectUrl,
        [FromForm(Name = "app_session_id")]
        string? appSessionId)
    {
        // Parse response_mode (ASP.NET model binding will only do simple conversion)
        OAuth2ResponseMode? oAuth2ResponseMode = responseMode switch
        {
            null => null,
            "query" => OAuth2ResponseMode.Query,
            "fragment" => OAuth2ResponseMode.Fragment,
            "form_post" => OAuth2ResponseMode.FormPost,
            _ => throw new ArgumentOutOfRangeException(
                nameof(responseMode),
                responseMode,
                "Unknown value for response_mode.")
        };

        // Operation
        var authResult =
            new AuthResult
            {
                ResponseMode = oAuth2ResponseMode,
                RedirectUrl = redirectUrl,
                AppSessionId = appSessionId,
                State = state,
                OAuth2RedirectOptionalParameters = new OAuth2RedirectOptionalParameters
                {
                    Error = error,
                    IdToken = idToken,
                    Code = code
                }
            };
        AuthContextUpdateAuthResultResponse fluentResponse = await _requestBuilder
            .AuthContexts
            .UpdateAuthResultAsync(authResult, modifiedBy);

        return Created("about:blank", fluentResponse);
    }
}
