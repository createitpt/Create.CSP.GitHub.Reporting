using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Create.CSP.GitHub.Reporting.Entities;
using PartnerSdkModels = Microsoft.Store.PartnerCenter.Models;

namespace Create.CSP.GitHub.Reporting.Models
{
    public class ActivationReportOutputItem : ReportOutputItem
    {
        public List<ActivationReportSubscribedSKUOutputItem> CustomerSubscribedSkuAndSubscriptions { get; set; }

        public ActivationReportOutputItem()
            : base()
        {
            CustomerSubscribedSkuAndSubscriptions = new List<ActivationReportSubscribedSKUOutputItem>();
        }
    }

    public class ActivationReportSubscribedSKUOutputItem
    {
        public SubscribedSku SubscribedSKU { get; set; }
        public List<PartnerSdkModels.Subscriptions.Subscription> SKUSubscriptions { get; set; }

        public PartnerSdkModels.Offers.Offer Offer { get; set; }

        public int ActiveSeats { get; set; }
        public int InGracePeriodSeats { get; set; }
        public int DisabledSeats { get; set; }
        public int AssignedSeats { get; set; }

        public string ActionType { get; set; }
        public string ActionSubType { get; set; }

        public ActivationReportSubscribedSKUOutputItem()
        {
            SubscribedSKU = new SubscribedSku();
            SKUSubscriptions = new List<PartnerSdkModels.Subscriptions.Subscription>();

            ActionType = string.Empty;
            ActionSubType = string.Empty;
        }
    }
}
