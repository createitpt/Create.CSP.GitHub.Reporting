using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Create.CSP.GitHub.Reporting.Database.Model.Managers
{
   public class SubscribedSKUsManager : BaseManager
    {    
        #region Constructor

        public SubscribedSKUsManager(DbContext dbContext = null)
            : base(dbContext)
        {    
        }

        #endregion

        #region Create

        public IEnumerable<SubscribedSKU> AddOrUpdate(IEnumerable<SubscribedSKU> subscribedSKUs)
        {
            List<SubscribedSKU> result = new List<SubscribedSKU>();

            if (subscribedSKUs == null || subscribedSKUs.Count() == 0)
            {
                // Nothing to do
                return result;
            }

            foreach (var subscribedSKU in subscribedSKUs)
            {
                result.Add(base.AddOrUpdate(subscribedSKU, s => s.Id == subscribedSKU.Id));
            }

            _dbContext.SaveChanges();
            return result;
        }

        #endregion
    }
}
