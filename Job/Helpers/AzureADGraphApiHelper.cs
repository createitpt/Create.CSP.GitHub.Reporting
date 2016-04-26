using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphClient = Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Create.CSP.GitHub.Reporting.Entities;
using System.Net;
using System.IO;

namespace Create.CSP.GitHub.Reporting.Helpers
{
    public class AzureADGraphApiHelper
    {
        #region Members/Properties

        private string _forCustomerTenant = null;
        private AuthorizationToken _tokenForRequests = null;
        private GraphClient.ActiveDirectoryClient _activeDirectoryClient = null;

        #endregion

        #region Constructor & Client Initialization

        /// <summary>
        /// Get a object instance with the token for requests initialized for the received tenant or the CSP tenant
        /// (if empty parameter).
        /// </summary>
        public AzureADGraphApiHelper(string forCustomerTenant = null)
        {
            // If empty, initialize for CSP Tenant authentication
            if (string.IsNullOrWhiteSpace(forCustomerTenant))
            { // Use CSP Tenant context
                _forCustomerTenant = Constants.CSP_TENANT_NAME;
            }
            else
            { // Initialize for customer tenant authentication
                _forCustomerTenant = forCustomerTenant;
            }

            // Get the AD token for the requests
            _tokenForRequests = GetADTokenForRequests(_forCustomerTenant).Result;
        }

        /// <summary>
        /// Gets the active directory C# client, initialized for the proper tenant.
        /// </summary>   
        public GraphClient.ActiveDirectoryClient GetActiveDirectoryClient()
        {
            if (_activeDirectoryClient != null)
            { // Already initialized
                return _activeDirectoryClient;
            }

            // else. Initialize and return
            string domain = !string.IsNullOrWhiteSpace(_forCustomerTenant) ?
                _forCustomerTenant : Constants.CSP_TENANT_NAME;

            Uri servicePointUri = new Uri(Constants.GRAPH_RESOURCE_URL);
            Uri serviceRoot = new Uri(servicePointUri, domain);

            _activeDirectoryClient = new GraphClient.ActiveDirectoryClient(serviceRoot,
                    async () => (await GetADTokenForRequests(_forCustomerTenant)).AccessToken);

            return _activeDirectoryClient;
        }

        /// <summary>
        /// Gets the AD token for the requests, for the received customer tenant.
        /// </summary>
        public async Task<AuthorizationToken> GetADTokenForRequests(string customerTenant)
        {
            if (_tokenForRequests != null)
            {
                // already initialized
                return _tokenForRequests;
            }

            AuthenticationContext _authenticationContext = new AuthenticationContext(string.Format(Constants.AAD_INSTANCE,
                customerTenant));

            UserCredential _userCredential = new UserCredential(Constants.CSP_ADMIN_USERNAME,
                Constants.CSP_ADMIN_PASSWORD);

            // else. Initialize and return
            AuthenticationResult authenticationResult = await _authenticationContext.AcquireTokenAsync(
                  Constants.GRAPH_RESOURCE_URL,
                  Constants.AZURE_AD_APP_ID_NATIVE_APP,
                  _userCredential);

            _tokenForRequests = new AuthorizationToken(authenticationResult.AccessToken,
             authenticationResult.ExpiresOn.DateTime);

            return _tokenForRequests;
        }

        #endregion

        #region Request builders/helpers

        /// <summary>
        /// Generates a request with the received parameters, for the beta Graph API version.
        /// </summary>   
        public static HttpWebRequest GenerateRequest(string method, string requestUri, string token)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(
                Constants.GRAPH_RESOURCE_URL + requestUri + "?api-version=beta");

            request.Method = method;
            request.ContentType = "application/json";
            request.Accept = "application/json";

            request.Headers.Add("x-ms-correlation-id", Guid.NewGuid().ToString());
            request.Headers.Add("x-ms-tracking-id", Guid.NewGuid().ToString());
            request.Headers.Add("Authorization", "Bearer " + token);

            return request;
        }

        /// <summary>
        /// Generates a GET request.
        /// </summary>   
        public static HttpWebRequest GenerateGetRequest(string requestUri, string token)
        {
            return GenerateRequest("GET", requestUri, token);
        }

        /// <summary>
        /// Generates a POST request.
        /// </summary>   
        public static HttpWebRequest GeneratePostRequest(string requestUri, string token)
        {
            return GenerateRequest("POST", requestUri, token);
        }

        /// <summary>
        /// Generates a DELETE request.
        /// </summary>     
        public static HttpWebRequest GenerateDeleteRequest(string requestUri, string token)
        {
            return GenerateRequest("DELETE", requestUri, token);
        }

        #endregion

        #region Azure AD Graph API request builders/helpers

        public HttpWebRequest BuildGetCustomerDomainsRequest()
        {
            return GenerateGetRequest(
              string.Format("/{0}/domains", _forCustomerTenant),
              _tokenForRequests.AccessToken);
        }

        /// <summary>
        /// Generates a CREATE customer domain request.
        /// </summary>    
        public HttpWebRequest BuildCreateCustomerDomainRequest(string domainToCreate)
        {
            HttpWebRequest request = GeneratePostRequest(
              string.Format("/{0}/domains", _forCustomerTenant),
              _tokenForRequests.AccessToken);

            string content = string.Format("{{\"name\": \"{0}\"}}", domainToCreate);

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(content);
            }

            return request;
        }

        public HttpWebRequest BuildGetCustomerDomainVerificationRecordsRequest(string domainToGet)
        {
            return GenerateGetRequest(
              string.Format("/{0}/domains('{1}')/verificationDnsRecords", _forCustomerTenant, domainToGet),
              _tokenForRequests.AccessToken);
        }

        public HttpWebRequest BuildVerifyCustomerDomainRequest(string domainToVerify)
        {
            HttpWebRequest request = GeneratePostRequest(
              string.Format("/{0}/domains('{1}')/verify", _forCustomerTenant, domainToVerify),
              _tokenForRequests.AccessToken);

            request.ContentLength = 0;

            return request;
        }

        #endregion
    }
}
