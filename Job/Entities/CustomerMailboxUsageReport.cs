using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Create.CSP.GitHub.Reporting.Entities
{
    public class CustomerMailboxUsageReport
    {
        public Guid CustomerId { get; set; }

        public long TotalMailboxCount { get; set; }
        public long TotalInactiveMailboxCount { get; set; }
        public long MailboxesOverWarningSize { get; set; }
        public long MailboxesUsedLessthan25Percent { get; set; }
    }
}
