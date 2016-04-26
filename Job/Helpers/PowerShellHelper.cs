using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Create.CSP.GitHub.Reporting.Helpers
{
    public static class PowerShellHelper
    {
        public static Collection<PSObject> ExecutePowerShellScript(string scriptToExecute)
        {
            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                string username = Constants.CSP_ADMIN_USERNAME;
                string password = Constants.CSP_ADMIN_PASSWORD;
                var securePassword = new SecureString();
                Array.ForEach(password.ToCharArray(), securePassword.AppendChar);
                PSCredential credentials = new PSCredential(username, securePassword);

                PowerShellInstance.AddCommand("Set-Variable");
                PowerShellInstance.AddParameter("Name", "credentials");
                PowerShellInstance.AddParameter("Value", credentials);

                // this script 
                PowerShellInstance.AddScript(scriptToExecute);

                // invoke execution on the pipeline (collecting output)
                Collection<PSObject> PSOutput = PowerShellInstance.Invoke();

                // check the other output streams (for example, the error stream)
                if (PowerShellInstance.Streams.Error.Count > 0)
                {
                    // error records were written to the error stream.
                    // do something with the items found.
                    throw new Exception(
                        string.Join("/",
                        PowerShellInstance.Streams.Error.Select(error => error.ToString())));
                }

                return PSOutput;
            }
        }

        public static Collection<PSObject> ExecuteMSOnlinePowerShellScript(string scriptToExecute)
        {
            // Encapsulate received script in MSOnline execution
            string script = "import-module MSOnline; Connect-MsolService -Credential $credentials;";

            // Insert the received script to execute
            script += scriptToExecute;

            // Execute the received script
            return PowerShellHelper.ExecutePowerShellScript(script);
        }

        public static Collection<PSObject> ExecuteExchangeOnlinePowerShellScript(string tenantId, string scriptToExecute)
        {
            // Encapsulate received script in exchange online session with delegated permissions
            // https://technet.microsoft.com/en-us/library/dn705740.aspx
            string script = "Try { $Session = New-PSSession -ConfigurationName Microsoft.Exchange -ConnectionUri https://ps.outlook.com/PowerShell-LiveID?DelegatedOrg=";
            script += string.Format("{0} -Credential $credentials -Authentication Basic –AllowRedirection -ErrorAction Stop; ", tenantId);

            // Insert the received script to execute
            script += string.Format("Invoke-Command -Session $Session -ScriptBlock {{ {0} -ErrorAction Stop;}} -ErrorAction Stop; }} Finally {{ Remove-PSSession $Session; }}",
                scriptToExecute);

            // Execute the received script in the session
            return PowerShellHelper.ExecutePowerShellScript(script);
        }
    }
}
