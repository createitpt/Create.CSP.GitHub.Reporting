using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Create.CSP.GitHub.Reporting.Entities
{
    public class SubscribedSku
    {
        public string ObjectId { get; set; }
        public Guid SkuId { get; set; }
        public string SkuPartNumber { get; set; }
        public string CapabilityStatus { get; set; }
        public LicenseUnitsDetail PrepaidUnits { get; set; }
        public List<ServicePlanInfo> ServicePlans { get; set; }
        public int ConsumedUnits { get; set; }

        public SubscribedSku()
        {
            ObjectId = string.Empty;
            SkuId = Guid.Empty;
            SkuPartNumber = string.Empty;
            CapabilityStatus = string.Empty;
            PrepaidUnits = new LicenseUnitsDetail();
            ServicePlans = new List<ServicePlanInfo>();
        }
    }

    public class LicenseUnitsDetail
    {
        public int Enabled { get; set; }
        public int Suspended { get; set; }
        public int Warning { get; set; }
    }
      
    public class ServicePlanInfo
    {
        public Guid ServicePlanId { get; set; }
        public string ServicePlanName { get; set; }
        public AppliesTo AppliesTo { get; set; }
        public ProvisioningStatus ProvisioningStatus { get; set; }

        public ServicePlanInfo()
        {
            ServicePlanId = Guid.Empty;
            ServicePlanName = string.Empty;
            AppliesTo = Entities.AppliesTo.NotDefined;
            ProvisioningStatus = Entities.ProvisioningStatus.NotDefined;
        }
    }

    public enum ProvisioningStatus
    {
        NotDefined,
        Success,
        PendingActivation,
    }

    public enum AppliesTo
    {
        NotDefined,
        User,
        Company,
    }
}
