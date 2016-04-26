using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Create.CSP.GitHub.Reporting.Entities;

namespace Create.CSP.GitHub.Reporting.Mappers
{
    public class CustomerSPOActiveUserReportMapper
    {
        public static CustomerSPOActiveUserReport MapFromObject(Guid customerId, dynamic source)
        {
            return new CustomerSPOActiveUserReport()
            {
                CustomerId = customerId,
                UniqueUsers = source.UniqueUsers,
                LicensesAssigned = source.LicensesAssigned,
                LicensesAcquired = source.LicensesAcquired,
                TotalUsers = source.TotalUsers,
            };

        }
    }
}
