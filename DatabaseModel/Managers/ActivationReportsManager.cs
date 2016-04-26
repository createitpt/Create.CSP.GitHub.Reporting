using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Create.CSP.GitHub.Reporting.Database.Model.Managers
{
    public class ActivationReportsManager : BaseManager
    {
        #region Constructor

        public ActivationReportsManager(DbContext dbContext = null)
            : base(dbContext)
        {
        }

        #endregion

        #region Create

        public IEnumerable<ActivationReport> AddActivationReports(IEnumerable<ActivationReport> activationReports)
        {
            return _dbContext.ActivationReports.AddRange(activationReports);
        }

        #endregion
    }
}
