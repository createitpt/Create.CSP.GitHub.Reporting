using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Create.CSP.GitHub.Reporting.Entities
{
    public class Domain
    {     
        public string CustomerId { get; set; }

        /// <summary>
        /// Concatenates the Name with the Customer Id
        /// </summary>
        public string ComputedId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CustomerId))
                {
                    return string.Join("_", Name, string.Empty);
                }

                if (string.IsNullOrWhiteSpace(Name))
                {
                    return string.Join("_", string.Empty, CustomerId);
                }

                return string.Join("_", Name, CustomerId);
            }
        }

        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public bool IsInitial { get; set; }
        public bool IsRoot { get; set; }
        public bool IsVerified { get; set; }
        public bool IsAdminManaged { get; set; }
        public bool? AvailabilityStatus { get; set; }
        public string AuthenticationType { get; set; }
    }
}
