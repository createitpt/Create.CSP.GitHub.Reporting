using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Create.CSP.GitHub.Reporting.Database.Model.Managers
{
    public class MarketingCampaignsCustomer : BaseManager
    {
        #region Constructor

        public MarketingCampaignsCustomer(DbContext dbContext = null)
            : base(dbContext)
        {
        }
        
        #endregion

    }
}
