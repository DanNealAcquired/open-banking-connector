﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    /// <summary>
    ///     Fluent context for entity created in external (i.e. bank) database only.
    /// </summary>
    /// <typeparam name="TPublicRequest"></typeparam>
    /// <typeparam name="TPublicResponse"></typeparam>
    public interface IExternalEntityContext<in TPublicRequest, TPublicResponse> :
        ICreateContext<TPublicRequest, TPublicResponse>,
        IReadContext<TPublicResponse>
        where TPublicResponse : class { }
}
