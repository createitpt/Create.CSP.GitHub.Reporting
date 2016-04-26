using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Create.CSP.GitHub.Reporting.Entities
{
    public class CustomerCsActiveUserReport
    {
        public Guid CustomerId { get; set; }

        public long ActiveUsers { get; set; }
        public long ActiveIMUsers { get; set; }
        public long ActiveAudioUsers { get; set; }
        public long ActiveVideoUsers { get; set; }

    }
}
