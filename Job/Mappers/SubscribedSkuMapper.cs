using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Create.CSP.GitHub.Reporting.Entities;
using GraphClient = Microsoft.Azure.ActiveDirectory.GraphClient;

namespace Create.CSP.GitHub.Reporting.Mappers
{
    public static class SubscribedSkuMapper
    {
        public static SubscribedSku MapFromSource(GraphClient.ISubscribedSku source)
        {
            return new SubscribedSku
            {
                ObjectId = source.ObjectId,
                SkuId = source.SkuId ?? Guid.Empty,
                SkuPartNumber = source.SkuPartNumber,
                ConsumedUnits = source.ConsumedUnits ?? 0,
                CapabilityStatus = source.CapabilityStatus,

                PrepaidUnits = SubscribedSkuMapper.MapFromSource(source.PrepaidUnits),
                ServicePlans = SubscribedSkuMapper.MapFromSource(source.ServicePlans),
            };
        }

        public static LicenseUnitsDetail MapFromSource(GraphClient.LicenseUnitsDetail source)
        {
            return new LicenseUnitsDetail
            {
                Enabled = source.Enabled ?? 0,
                Suspended = source.Suspended ?? 0,
                Warning = source.Warning ?? 0
            };
        }

        public static List<ServicePlanInfo> MapFromSource(IList<GraphClient.ServicePlanInfo> source)
        {
            return source.Select(item => new ServicePlanInfo()
            {
                ServicePlanId = item.ServicePlanId ?? Guid.Empty,
                ServicePlanName = item.ServicePlanName,
            }).ToList();
        }
    }
}
