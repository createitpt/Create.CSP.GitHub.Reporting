using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Create.CSP.GitHub.Reporting.Entities
{
    public class AuthorizationToken
    {
        /// <summary>
        /// Captures when the token expires
        /// </summary>
        public DateTime ExpiresOn { get; set; }

        /// <summary>
        /// Access token
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Constructor for getting an authorization token
        /// </summary>
        /// <param name="access_Token">access token</param>
        /// <param name="expires_in">number of seconds the token is valid for</param>
        public AuthorizationToken(string access_Token, long expires_in)
        {
            this.AccessToken = access_Token;
            this.ExpiresOn = DateTime.UtcNow.AddSeconds(expires_in);
        }

        public AuthorizationToken(string accessToken, DateTime expiresOn)
        {
            this.AccessToken = accessToken;
            this.ExpiresOn = expiresOn.ToLocalTime();
        }

        /// <summary>
        /// Returns true if the authorization token is near expiracy.
        /// </summary>
        /// <returnsTtrue if the authorization token is near expiracy. False otherwise.</returns>
        public bool IsNearExpiracy()
        {
            //// if token is expiring in the next minute or expired, return true
            return DateTime.UtcNow.ToLocalTime() > this.ExpiresOn.AddMinutes(-1);
        }
    }
}
