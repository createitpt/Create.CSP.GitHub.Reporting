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
    public static class CustomerMapper
    {
        public static DatabaseEntities.Customer MapFromSource(PartnerSdkModels.Customers.Customer source)
        {
            Guid tenantId;

            return new DatabaseEntities.Customer()
            {
                Id = Guid.Parse(source.Id),
                TenantId = source.CompanyProfile != null && !string.IsNullOrEmpty(source.CompanyProfile.TenantId) 
                && Guid.TryParse(source.CompanyProfile.TenantId, out tenantId) ? tenantId : Guid.Empty,
                CompanyName = source.CompanyProfile != null && !string.IsNullOrEmpty(source.CompanyProfile.TenantId) ?
                source.CompanyProfile.CompanyName : string.Empty
            };
        }

        public static PartnerSdkModels.Customers.Customer MapFromSource(DatabaseEntities.Customer source)
        {
            PartnerSdkModels.Customers.CustomerCompanyProfile companyProfile = new PartnerSdkModels.Customers.CustomerCompanyProfile();
            companyProfile.TenantId = source.TenantId != null && !string.IsNullOrEmpty(source.TenantId.ToString()) ? source.TenantId.ToString() : string.Empty;
            companyProfile.CompanyName = !string.IsNullOrEmpty(source.CompanyName) ? source.CompanyName : string.Empty;

            return new PartnerSdkModels.Customers.Customer()
            { 
                Id = source.Id != null && !string.IsNullOrEmpty(source.Id.ToString()) ? source.Id.ToString() : string.Empty,
                CompanyProfile = companyProfile
            };
        }
    }
}
