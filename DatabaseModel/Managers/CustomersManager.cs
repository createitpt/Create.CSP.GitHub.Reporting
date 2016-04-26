using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Create.CSP.GitHub.Reporting.Database.Model.Managers
{
    public class CustomersManager : BaseManager
    {
        #region Constructor

        public CustomersManager(DbContext dbContext = null)
            : base(dbContext)
        {
        }

        #endregion

        #region Create

        public IEnumerable<Customer> AddOrUpdate(IEnumerable<Customer> customers)
        {
            List<Customer> result = new List<Customer>(customers.Count());

            foreach (var customer in customers)
            {
                result.Add(base.AddOrUpdate(customer, c => c.Id == customer.Id));
            }

            _dbContext.SaveChanges();
            return result;
        }

        #endregion
    }
}
