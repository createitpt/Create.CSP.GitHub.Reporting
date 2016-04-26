using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Create.CSP.GitHub.Reporting.Entities;

namespace Create.CSP.GitHub.Reporting.Mappers
{
    public class CustomerCsActiveUserReportMapper
    {
        public static CustomerCsActiveUserReport MapFromObject(Guid customerId, dynamic source)
        {
            return new CustomerCsActiveUserReport()
            {
                CustomerId = customerId,
                ActiveUsers = source.ActiveUsers,
                ActiveIMUsers = source.ActiveIMUsers,
                ActiveAudioUsers = source.ActiveAudioUsers,
                ActiveVideoUsers = source.ActiveVideoUsers,
            };

        }
    }
}
