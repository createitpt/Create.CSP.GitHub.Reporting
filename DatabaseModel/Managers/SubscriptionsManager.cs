using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Create.CSP.GitHub.Reporting.Database.Model.Managers
{
    public class SubscriptionsManager : BaseManager
    {
        #region Constructor

        public SubscriptionsManager(DbContext dbContext = null)
            : base(dbContext)
        {
        }

        #endregion

        #region Create

        public IEnumerable<Subscription> AddOrUpdate(IEnumerable<Subscription> subscriptions)
        {
            List<Subscription> result = new List<Subscription>();

            if (subscriptions == null || subscriptions.Count() == 0)
            {
                // Nothing to do
                return result;
            }

            foreach (var subscription in subscriptions)
            {
                result.Add(base.AddOrUpdate(subscription, s => s.Id == subscription.Id));
            }

            _dbContext.SaveChanges();
            return result;
        }

        #endregion
    }
}
