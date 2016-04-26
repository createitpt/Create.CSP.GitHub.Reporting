using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Create.CSP.GitHub.Reporting.Entities;
using DatabaseModel = Create.CSP.GitHub.Reporting.Database.Model;
using DomainEntities = Create.CSP.GitHub.Reporting.Entities;

namespace Create.CSP.GitHub.Reporting.Mappers
{
    public static class DomainMapper
    {
        public static Domain MapFromJson(string customerId, dynamic json)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentException("Customer Id must not be empty");
            }

            Domain domain = new Domain()
            {
                CustomerId = customerId,
                AuthenticationType = json.authenticationType,
                AvailabilityStatus = json.availabilityStatus,
                IsAdminManaged = json.isAdminManaged,
                IsDefault = json.isDefault,
                IsInitial = json.isInitial,
                IsRoot = json.isRoot,
                IsVerified = json.isVerified,
                Name = json.name,
            };

            return domain;
        }

        public static DatabaseModel.Domain MapFromSource(DomainEntities.Domain source, string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentNullException("Customer id cannot be empty or null");
            }

            Guid customerIdGuid;
            if (Guid.TryParse(customerId, out customerIdGuid) == false)
            {
                throw new ArgumentException("Customer Id must be a GUID");
            }

            return new DatabaseModel.Domain()
            {
                Id = source.ComputedId,
                Name = source.Name,
                CustomerId = customerIdGuid,
                IsDefault = source.IsDefault
            };
        }
    }
}
