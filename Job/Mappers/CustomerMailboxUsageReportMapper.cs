using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Create.CSP.GitHub.Reporting.Entities;

namespace Create.CSP.GitHub.Reporting.Mappers
{
    public class CustomerMailboxUsageReportMapper
    {
        public static CustomerMailboxUsageReport MapFromObject(Guid customerId, dynamic source)
        {
            return new CustomerMailboxUsageReport()
            {
                CustomerId = customerId,
                TotalMailboxCount = source.TotalMailboxCount,
                TotalInactiveMailboxCount = source.TotalInactiveMailboxCount,
                MailboxesOverWarningSize = source.MailboxesOverWarningSize,
                MailboxesUsedLessthan25Percent = source.MailboxesUsedLessthan25Percent,
            };
        }
    }
}
