using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseEntities = Create.CSP.GitHub.Reporting.Database.Model;
using PartnerSdkModels = Microsoft.Store.PartnerCenter.Models;
using DomainEntities = ReportingPortal.Domain;

namespace ReportingPortal.Mappers
{
    public static class MarketingCampaignMapper
    {
        public static DomainEntities.Campaign MapFromSource(DatabaseEntities.MarketingCampaign source)
        {
            return new DomainEntities.Campaign()
            {
                CampaignId = source.Id != null && !string.IsNullOrEmpty(source.Id.ToString()) ? source.Id.ToString() : string.Empty,
                Customers = source.MarketingCampaignsCustomers != null ? source.MarketingCampaignsCustomers
                .Where(x => x.Customer != null)
                .Select(x => CustomerMapper.MapFromSource(x.Customer)).ToList() : null,
                EndDate = source.EndDate != null ? source.EndDate.ToShortDateString() : string.Empty,
                Name = !string.IsNullOrEmpty(source.Name) ? source.Name : string.Empty,
                StartDate = source.StartDate != null ? source.StartDate.ToShortDateString() : string.Empty
            };
        }

        public static DatabaseEntities.MarketingCampaign MapFromSource(DomainEntities.Campaign source)
        {
            DateTime endDate;
            DateTime startDate;
            Guid id;

            return new DatabaseEntities.MarketingCampaign()
            {
                Id = source.CampaignId != null && !string.IsNullOrEmpty(source.CampaignId) 
                && Guid.TryParse(source.CampaignId, out id) ? id : Guid.Empty,
                MarketingCampaignsCustomers = source.Customers != null ? source.Customers.Select(x => MarketingCampaignCustomerMapper.MapFromSource(x)).ToList() : null,
                EndDate = source.EndDate != null && DateTime.TryParse(source.EndDate, out endDate) ? endDate : DateTime.Now.Date,
                Name = !string.IsNullOrEmpty(source.Name) ? source.Name : string.Empty,
                StartDate = source.StartDate != null && DateTime.TryParse(source.StartDate, out startDate) ? startDate : DateTime.Now.Date
            };
        }
    }
}
