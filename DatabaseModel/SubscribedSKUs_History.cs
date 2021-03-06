//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Create.CSP.GitHub.Reporting.Database.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class SubscribedSKUs_History
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SubscribedSKUs_History()
        {
            this.Subscriptions_History = new HashSet<Subscriptions_History>();
        }
    
        public System.Guid CorrelationId { get; set; }
        public string Id { get; set; }
        public System.Guid CustomerId { get; set; }
        public System.Guid SkuBusinessId { get; set; }
        public string PartNumber { get; set; }
        public string OfferName { get; set; }
        public string CapabilityStatus { get; set; }
        public int ActiveSeats { get; set; }
        public int InGracePeriodSeats { get; set; }
        public int DisabledSeats { get; set; }
        public int AssignedSeats { get; set; }
    
        public virtual CorrelationId CorrelationId1 { get; set; }
        public virtual Customers_History Customers_History { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Subscriptions_History> Subscriptions_History { get; set; }
    }
}
