using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Create.CSP.GitHub.Reporting.Database.Model.Managers
{
    public class DomainsManager : BaseManager
    {
        #region Constructor

        public DomainsManager(DbContext dbContext = null)
            : base(dbContext)
        {
        }

        #endregion

        #region Create

        public IEnumerable<Domain> AddOrUpdate(IEnumerable<Domain> domains)
        {
            List<Domain> result = new List<Domain>(domains.Count());

            foreach (var domain in domains)
            {
                result.Add(base.AddOrUpdate(domain, d => d.Id == domain.Id));
            }

            _dbContext.SaveChanges();
            return result;
        }

        #endregion
    }
}
