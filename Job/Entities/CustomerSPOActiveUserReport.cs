using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Create.CSP.GitHub.Reporting.Entities
{
    public class CustomerSPOActiveUserReport
    {
        public Guid CustomerId { get; set; }

        public long UniqueUsers { get; set; }
        public long LicensesAssigned { get; set; }
        public long LicensesAcquired { get; set; }
        public long TotalUsers { get; set; }
    }
}
