using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;

namespace Create.CSP.GitHub.Reporting
{
    public class Constants
    {
        public static string AAD_INSTANCE = ConfigurationManager.AppSettings["AADInstance"];
        public static string CSP_TENANT_NAME = ConfigurationManager.AppSettings["CSPTenantName"];
        public static string CSP_TENANT_ID = ConfigurationManager.AppSettings["CSPTenantId"];
        
        public static string GRAPH_RESOURCE_URL = ConfigurationManager.AppSettings["GraphResourceUrl"];
        public static string PARTNER_CENTER_SDK_RESOURCE_URL = ConfigurationManager.AppSettings["PartnerCenterSdkResourceUrl"];

        public static string AZURE_AD_APP_ID_NATIVE_APP = ConfigurationManager.AppSettings["AzureADAppId-NativeApplication"];
    
        public static string CSP_ADMIN_USERNAME = ConfigurationManager.AppSettings["CSPAdminUsername"];
        public static string CSP_ADMIN_PASSWORD = ConfigurationManager.AppSettings["CSPAdminPassword"];

        public static string CSP_COUNTRY_TWO_LETTER_CODE = ConfigurationManager.AppSettings["CSPCountryTwoLetterCode"];        
    }
}
