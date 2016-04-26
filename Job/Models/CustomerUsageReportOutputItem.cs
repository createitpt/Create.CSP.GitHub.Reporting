using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Create.CSP.GitHub.Reporting.Models
{
    public class CustomerUsageReportOutputItem : ReportOutputItem
    {
        public string OnMicrosoftDomain { get; set; }
        public string DefaultDomain { get; set; }
        public int TotalDomains { get; set; }

        public Guid SubscriptionId { get; set; }
        public string SubscriptionStatus { get; set; }
        public uint SubscriptionQuantity { get; set; }

        public string OfferName { get; set; }
        public string SKUPartNumber { get; set; }
        public int SKUTotalServices { get; set; }

        public string SKUService { get; set; }
        public string SKUServiceProvisioningStatus { get; set; }
        public long SKUAssignedSeats { get; set; }

        public long SKUServiceActiveUsers { get; set; }
        
        public CustomerUsageReportOutputItem()
            : base()
        {
            SubscriptionStatus = string.Empty;
            OfferName = string.Empty;
            SKUPartNumber = string.Empty;
            SKUService = string.Empty;
            SKUServiceProvisioningStatus = string.Empty;
        }
    }
}
