// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox
{
    public partial class BankProfileDefinitions
    {
        public BankProfile HsbcOld { get; }

        private BankProfile GetHsbcOld()
        {
            BankProfileHiddenProperties bankProfileHiddenProperties =
                GetRequiredBankProfileHiddenProperties(BankProfileEnum.HsbcOld);
            return new BankProfile(
                BankProfileEnum.HsbcOld,
                bankProfileHiddenProperties.GetRequiredIssuerUrl(),
                bankProfileHiddenProperties.GetRequiredFinancialId(),
                bankProfileHiddenProperties.GetRequiredClientRegistrationApiVersion(),
                null,
                new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion = bankProfileHiddenProperties
                        .GetRequiredPaymentInitiationApiVersion(),
                    BaseUrl = bankProfileHiddenProperties
                        .GetRequiredPaymentInitiationApiBaseUrl()
                },
                null)
            {
                BankConfigurationApiSettings = new BankConfigurationApiSettings
                {
                    BankRegistrationAdjustments = registration =>
                    {
                        registration.IssuerUrl = null;
                        registration.RegistrationEndpoint = "unused";
                        registration.TokenEndpoint = bankProfileHiddenProperties.GetAdditionalProperty1();
                        registration.AuthorizationEndpoint = bankProfileHiddenProperties.GetAdditionalProperty3();
                        registration.TokenEndpointAuthMethod = TokenEndpointAuthMethod.PrivateKeyJwt;
                        registration.ExternalApiObject = new ExternalApiBankRegistration
                        {
                            ExternalApiId = bankProfileHiddenProperties.GetAdditionalProperty4()
                        };
                        BankRegistrationPostCustomBehaviour bankRegistrationPost =
                            (registration.CustomBehaviour ??= new CustomBehaviourClass())
                            .BankRegistrationPost ??= new BankRegistrationPostCustomBehaviour();
                        ConsentAuthGetCustomBehaviour accountAccessConsentAuthGet =
                            registration.CustomBehaviour.AccountAccessConsentAuthGet ??=
                                new ConsentAuthGetCustomBehaviour();

                        accountAccessConsentAuthGet.IssClaim = bankProfileHiddenProperties.GetAdditionalProperty1();
                        accountAccessConsentAuthGet.AudClaim = bankProfileHiddenProperties.GetAdditionalProperty2();
                        return registration;
                    },
                },
            };
        }
    }
}
