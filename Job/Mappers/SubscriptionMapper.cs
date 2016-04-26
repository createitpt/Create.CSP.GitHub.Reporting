using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartnerSdkModels = Microsoft.Store.PartnerCenter.Models;
using DatabaseModel = Create.CSP.GitHub.Reporting.Database.Model;

namespace Create.CSP.GitHub.Reporting.Mappers
{
    public static class SubscriptionMapper
    {
        public static DatabaseModel.Subscription MapFromSource(PartnerSdkModels.Subscriptions.Subscription source,
            string customerId, string subscribedSkuId)
        {
            return new DatabaseModel.Subscription()
            {
                Id = Guid.Parse(source.Id),
                Status = source.Status.ToString(),
                Quantity = source.Quantity,
                CustomerId = Guid.Parse(customerId),
                SubscribedSKUId = subscribedSkuId,
                CreationDate = source.CreationDate
            };
        }
    }
}
