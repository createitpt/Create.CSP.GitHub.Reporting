using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseEntities = Create.CSP.GitHub.Reporting.Database.Model;
using PartnerSdkModels = Microsoft.Store.PartnerCenter.Models;

namespace ReportingPortal.Mappers
{
    public class MarketingCampaignCustomerMapper
    {
        public static DatabaseEntities.MarketingCampaignsCustomer MapFromSource(PartnerSdkModels.Customers.Customer source)
        {
            Guid customerId;

            return new DatabaseEntities.MarketingCampaignsCustomer()
            {
                CustomerId = !string.IsNullOrEmpty(source.Id) && Guid.TryParse(source.Id, out customerId) ? customerId : Guid.Empty
            };
        }
    }
}
