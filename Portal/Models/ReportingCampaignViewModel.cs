namespace ReportingPortal.Models
{
    using System.Collections.Generic;
    using ReportingPortal.Domain;

    using PartnerSdkModels = Microsoft.Store.PartnerCenter.Models;

    /// <summary>
    /// Reporting Campaign Index View Model
    /// </summary>
    public class ReportingCampaignViewModel
    {
        /// <summary>
        /// Gets or sets the campaign.
        /// </summary>
        /// <value>
        /// The campaign.
        /// </value>
        public Campaign Campaign { get; set; }

        /// <summary>
        /// Gets or sets the customers campaign.
        /// </summary>
        /// <value>
        /// The customers campaign.
        /// </value>
        public List<CustomersCampaign> CustomersCampaign { get; set; }

        /// <summary>
        /// Gets or sets the customers.
        /// </summary>
        /// <value>
        /// The customers.
        /// </value>
        public List<PartnerSdkModels.Customers.Customer> Customers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [record is added].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [record is added]; otherwise, <c>false</c>.
        /// </value>
        public bool RecordIsAddedOrUpdated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [edit record].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [edit record]; otherwise, <c>false</c>.
        /// </value>
        public bool EditRecord { get; set; }
    }
}
