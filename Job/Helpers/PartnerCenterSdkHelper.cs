using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Create.CSP.GitHub.Reporting.Entities;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Store.PartnerCenter;
using Microsoft.Store.PartnerCenter.Extensions;

namespace Create.CSP.GitHub.Reporting.Helpers
{
    public class PartnerCenterSdkHelper : AzureADApiHelper
    {
        private IAggregatePartner _partnerCenterSdkClient = null;

        public PartnerCenterSdkHelper(Guid? forCustomerTenantId = null, bool isTokenForUserPlusApplication = false)
            : base(forCustomerTenantId, isTokenForUserPlusApplication)
        {
            // Set partner center sdk api url
            _resourceUrl = Constants.PARTNER_CENTER_SDK_RESOURCE_URL;

            //_clientCredential = new ClientCredential(SettingsHelper.PARTNER_CENTER_SDK_APP_ID,
            //    SettingsHelper.PARTNER_CENTER_SDK_APP_KEY);

            _userCredential = new UserCredential(Constants.CSP_ADMIN_USERNAME,
                Constants.CSP_ADMIN_PASSWORD);

            _userPlusApplicationAppId = Constants.AZURE_AD_APP_ID_NATIVE_APP;
        }

        public async Task<IAggregatePartner> GetPartnerCenterSdkClientAsync()
        {
            if (_partnerCenterSdkClient != null)
            { // Already initialized
                return _partnerCenterSdkClient;
            }

            // else. Initialize and return
            await InitializePartnerCenterSdkClient();
            return _partnerCenterSdkClient;
        }

        private async Task InitializePartnerCenterSdkClient()
        {
            // Get token
            AuthorizationToken token = await GetAADTokenForRequestsAsync();

            IPartnerCredentials credentials = null;
            if (_isTokenForUserPlusApplication)
            {
                // Authenticate by user context with the partner service
                credentials = await PartnerCredentials.Instance.GenerateByUserCredentialsAsync(
                    _userPlusApplicationAppId,
                    new AuthenticationToken(
                        token.AccessToken,
                        token.ExpiresOn),
                    async delegate
                    {
                        // token has expired, re-Login to Azure Active Directory
                        var aadToken = await GetAADTokenForRequestsAsync();
                        return new AuthenticationToken(aadToken.AccessToken, aadToken.ExpiresOn);
                    });
            }
            else
            {
                //credentials = await PartnerCredentials.Instance.GenerateByApplicationCredentialsAsync(
                //    SettingsHelper.PARTNER_CENTER_SDK_APP_ID, SettingsHelper.PARTNER_CENTER_SDK_APP_KEY,
                //    SettingsHelper.CSP_TENANT_NAME);
            }

            // Create the partner operations
            _partnerCenterSdkClient = PartnerService.Instance.CreatePartnerOperations(credentials);
        }
    }
}
