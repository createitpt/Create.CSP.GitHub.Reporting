using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PartnerSdkModels = Microsoft.Store.PartnerCenter.Models;
using DatabaseModel = Create.CSP.GitHub.Reporting.Database.Model;

namespace Create.CSP.GitHub.Reporting.Mappers
{
    public static class CustomerMapper
    {
        public static DatabaseModel.Customer MapFromSource(PartnerSdkModels.Customers.Customer source)
        {
            Guid tenantId;

            return new DatabaseModel.Customer()
            {
                Id = Guid.Parse(source.Id),
                TenantId = source.CompanyProfile != null && !string.IsNullOrWhiteSpace(source.CompanyProfile.TenantId)
                && Guid.TryParse(source.CompanyProfile.TenantId, out tenantId) ? tenantId : Guid.Empty,
                CompanyName = source.CompanyProfile != null && !string.IsNullOrWhiteSpace(source.CompanyProfile.TenantId) ?
                source.CompanyProfile.CompanyName : string.Empty,
                AddressLine1 = source.BillingProfile != null && source.BillingProfile.DefaultAddress != null &&
                                    !string.IsNullOrWhiteSpace(source.BillingProfile.DefaultAddress.AddressLine1) ?
                                    source.BillingProfile.DefaultAddress.AddressLine1 : string.Empty,
                AddressLine2 = source.BillingProfile != null && source.BillingProfile.DefaultAddress != null &&
                                    !string.IsNullOrWhiteSpace(source.BillingProfile.DefaultAddress.AddressLine2) ?
                                    source.BillingProfile.DefaultAddress.AddressLine2 : string.Empty,
                City = source.BillingProfile != null && source.BillingProfile.DefaultAddress != null &&
                                    !string.IsNullOrWhiteSpace(source.BillingProfile.DefaultAddress.City) ?
                                    source.BillingProfile.DefaultAddress.City : string.Empty,
                Country = source.BillingProfile != null && source.BillingProfile.DefaultAddress != null &&
                                    !string.IsNullOrWhiteSpace(source.BillingProfile.DefaultAddress.Country) ?
                                    source.BillingProfile.DefaultAddress.Country : string.Empty,
                PostalCode = source.BillingProfile != null && source.BillingProfile.DefaultAddress != null &&
                                    !string.IsNullOrWhiteSpace(source.BillingProfile.DefaultAddress.PostalCode) ?
                                    source.BillingProfile.DefaultAddress.PostalCode : string.Empty,
                Region = source.BillingProfile != null && source.BillingProfile.DefaultAddress != null &&
                                    !string.IsNullOrWhiteSpace(source.BillingProfile.DefaultAddress.Region) ?
                                    source.BillingProfile.DefaultAddress.Region : string.Empty,
                State = source.BillingProfile != null && source.BillingProfile.DefaultAddress != null &&
                                    !string.IsNullOrWhiteSpace(source.BillingProfile.DefaultAddress.State) ?
                                    source.BillingProfile.DefaultAddress.State : string.Empty,
            };
        }

        public static PartnerSdkModels.Customers.Customer MapFromSource(DatabaseModel.Customer source)
        {
            PartnerSdkModels.Customers.CustomerCompanyProfile companyProfile = new PartnerSdkModels.Customers.CustomerCompanyProfile();
            companyProfile.TenantId = source.TenantId != null && !string.IsNullOrWhiteSpace(source.TenantId.ToString()) ? source.TenantId.ToString() : string.Empty;
            companyProfile.CompanyName = !string.IsNullOrWhiteSpace(source.CompanyName) ? source.CompanyName : string.Empty;

            return new PartnerSdkModels.Customers.Customer()
            {
                Id = source.Id != null && !string.IsNullOrWhiteSpace(source.Id.ToString()) ? source.Id.ToString() : string.Empty,
                CompanyProfile = companyProfile
            };
        }
    }
}
