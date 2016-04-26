using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Create.CSP.GitHub.Reporting.Entities;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Create.CSP.GitHub.Reporting.Helpers
{
    public class AzureADApiHelper
    {
        protected Guid _forCustomerTenantId = Guid.Empty;
        protected AuthorizationToken _AADTokenForRequests = null;
        protected AuthenticationContext _authenticationContext;
        protected ClientCredential _clientCredential;
        protected UserCredential _userCredential;
        protected bool _isTokenForUserPlusApplication;
        protected string _userPlusApplicationAppId;

        /// <summary>
        /// The resource url to authenticate for.
        /// </summary>
        protected string _resourceUrl;

        public AzureADApiHelper(Guid? forCustomerTenantId = null, bool isTokenForUserPlusApplication = false)
        {
            #region Member initialization

            _isTokenForUserPlusApplication = isTokenForUserPlusApplication;

            #endregion

            #region Determine the authentication context / tenant to use

            if (forCustomerTenantId == null || forCustomerTenantId == Guid.Empty)
            { // Use CSP Tenant context
                _forCustomerTenantId = Guid.Parse(Constants.CSP_TENANT_ID);
            }
            else
            { // Use customer tenant id received as parameter
                _forCustomerTenantId = forCustomerTenantId.Value;
            }

            _authenticationContext = new AuthenticationContext(string.Format(Constants.AAD_INSTANCE,
                _forCustomerTenantId));

            #endregion
        }

        /// <summary>
        /// Initializes and returns a new API helper of type T, with a token for the App Only authentication flow.
        /// </summary>
        /// <typeparam name="T">The sub-class type.</typeparam>
        /// <param name="forCustomerTenant">The customer tenant for authentication to.</param>
        /// <returns>A new API helper properly initialized.</returns>
        public static Task<T> GetNewAzureADApiHelperForApplicationRequests<T>(Guid? forCustomerTenantId = null) where T : AzureADApiHelper
        {
            return GetNewAzureADApiHelperForRequestsAsync<T>(forCustomerTenantId, false);
        }

        /// <summary>
        /// Initializes and returns a new API helper of type T, with a token for the App + User authentication flow.
        /// </summary>
        /// <typeparam name="T">The sub-class type.</typeparam>
        /// <param name="forCustomerTenant">The customer tenant for authentication to.</param>
        /// <returns>A new API helper properly initialized.</returns>
        public static Task<T> GetNewAzureADApiHelperForUserPlusApplicationRequests<T>(Guid? forCustomerTenantId = null) where T : AzureADApiHelper
        {
            return GetNewAzureADApiHelperForRequestsAsync<T>(forCustomerTenantId, true);
        }

        /// <summary>
        /// Initializes and returns a new API helper of type T, with a token for the received customer tenant and received authentication flow.
        /// </summary>
        /// <typeparam name="T">The sub-class type.</typeparam>
        /// <param name="forCustomerTenant">The customer tenant for authentication to.</param>
        /// <param name="isTokenForUserPlusApplication">The authentication flow. True, App + User. False, App Only.</param>
        /// <returns>A new API helper properly initialized.</returns>
        protected static async Task<T> GetNewAzureADApiHelperForRequestsAsync<T>(Guid? forCustomerTenantId, bool isTokenForUserPlusApplication) where T : AzureADApiHelper
        {
            // Create object
            var args = new object[] { forCustomerTenantId, isTokenForUserPlusApplication };
            var result = (T)Activator.CreateInstance(typeof(T), args);

            // Initialize for requests
            await result.GetAADTokenForRequestsAsync();

            return result;
        }

        /// <summary>
        /// Gets a token for executing API requests.
        /// The returned token is initialized according to the configuration of the member variables of the class.
        /// Ex: Authentication flow App Only or App + User.
        /// </summary>
        /// <returns>A token initialized according to the class parameters.</returns>
        /// <remarks>This method implements tha lazy load pattern. If a token is already present, and it is not about to expire,
        /// the previous token is returned.</remarks>
        public async Task<AuthorizationToken> GetAADTokenForRequestsAsync()
        {
            // If token not initialized or near expiracy
            if (_AADTokenForRequests != null && !_AADTokenForRequests.IsNearExpiracy())
            {
                // already initialized
                return _AADTokenForRequests;
            }

            // else. Initialize a new token and return it
            AuthenticationResult authenticationResult = null;

            if (_isTokenForUserPlusApplication)
            {
                // Get token for user + application credentials
                authenticationResult = await _authenticationContext.AcquireTokenAsync(
                  _resourceUrl,
                  _userPlusApplicationAppId,
                  _userCredential);
            }
            else
            {
                // Get an access token from Azure AD using client credentials.
                // ADAL includes an in memory cache, so this call will only send a message to the server 
                // if the cached token is expired.
                authenticationResult = await _authenticationContext.AcquireTokenAsync(
                    _resourceUrl,
                    _clientCredential);
            }

            _AADTokenForRequests = new AuthorizationToken(authenticationResult.AccessToken,
             authenticationResult.ExpiresOn.DateTime);

            return _AADTokenForRequests;
        }
    }
}
