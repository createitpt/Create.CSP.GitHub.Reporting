namespace ReportingPortal.Models
{
    using System.Collections.Generic;
    using ReportingPortal.Domain;

    /// <summary>
    /// Reporting Campaign Index View Model
    /// </summary>
    public class ReportingCampaignIndexViewModel
    {
        /// <summary>
        /// Gets or sets the campaigns.
        /// </summary>
        /// <value>
        /// The campaigns.
        /// </value>
        public IList<Campaign> Campaigns { get; set; }
    }
}
