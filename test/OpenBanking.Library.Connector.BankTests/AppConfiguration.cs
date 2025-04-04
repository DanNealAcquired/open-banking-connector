﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests;

/// <summary>
///     Get app configuration outside of .NET Generic Host dependency injection.
///     This is used for test discovery in static context and when
///     configuring "Plain" app tests.
/// </summary>
public static class AppConfiguration
{
    static AppConfiguration()
    {
        // Create dummy builder to get configuration
        string[] args = Array.Empty<string>();

        WebApplicationBuilder builder =
            WebApplication.CreateBuilder(
                new WebApplicationOptions
                {
                    Args = args,
                    // Ensure correct application name to allow loading of user secrets
                    ApplicationName = typeof(AppContextFixture).GetTypeInfo().Assembly.GetName().Name
                });

        Configuration = builder.Configuration;

        // Get requests directory
        var userSecretsId = "aa921213-9461-4f9e-8fec-153624ec67ad"; // from .csproj file
        string secretsPath = PathHelper.GetSecretsPathFromSecretsId(userSecretsId);
        string secretsDirectory = Path.GetDirectoryName(secretsPath) ?? throw new InvalidOperationException();
        RequestsDirectory = Path.Combine(secretsDirectory, "Requests");
        if (!Directory.Exists(RequestsDirectory))
        {
            throw new DirectoryNotFoundException("Cannot find Requests directory.");
        }
    }

    public static string RequestsDirectory { get; }

    public static IConfiguration Configuration { get; }

    public static TSettings GetSettings<TSettings>()
        where TSettings : class, ISettings<TSettings>, new() =>
        ServiceCollectionExtensions.GetSettings<TSettings>(Configuration);
}
