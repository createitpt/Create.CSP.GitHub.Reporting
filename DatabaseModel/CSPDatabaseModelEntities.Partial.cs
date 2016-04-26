using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Create.CSP.GitHub.Reporting.Database.Model.Managers;

namespace Create.CSP.GitHub.Reporting.Database.Model
{
    /// <summary>
    /// NOTE: The following tables should have in the model the primary key property
    /// "StoreGeneratedPattern" configured with "Identity"
    ///    - MarketingCampaign.Id
    ///    - MarketingCampaignsCustomer.Id
    ///    - ActivationReport.Id
    /// </summary>
    public partial class CSPDatabaseModelEntities : DbContext
    {
        private Guid _correlationId = Guid.Empty;

        public CSPDatabaseModelEntities(Guid? correlationId = null)
            : base()
        {
            _correlationId = correlationId ?? Guid.Empty;

            if (_correlationId != Guid.Empty)
            {
                // Explicitly open the connection and set the correlation id as session context
                this.Database.Connection.Open();

                // Set session correlation id context
                using (var dataCorrelationIdsManager = new CorrelationIdsManager(this))
                {
                    dataCorrelationIdsManager.SetSessionContextCorrelationId(_correlationId);
                }
            }
        }

        public new void Dispose()
        {
            if (_correlationId != Guid.Empty)
            {
                // Explicitly close the opened connection
                this.Database.Connection.Close();
            }

            base.Dispose();
        }
    }
}