using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Create.CSP.GitHub.Reporting.Entities;
using PartnerSdkModels = Microsoft.Store.PartnerCenter.Models;

namespace Create.CSP.GitHub.Reporting.Models
{   
    public class ReportOutputItem
    {
        public Guid CorrelationId { get; set; }
        public DateTime Timestamp { get; set; }

        public PartnerSdkModels.Customers.Customer Customer { get; set; }
        public List<Domain> CustomerDomains { get; set; }

        public string GlobalActionType { get; set; }
        public string GlobalActionSubType { get; set; }

        public ReportOutputItem()
        {
            Customer = null;
            CustomerDomains = new List<Domain>();

            GlobalActionType = string.Empty;
            GlobalActionSubType = string.Empty;
        }
    }
}
