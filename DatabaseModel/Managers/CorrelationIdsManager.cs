using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Create.CSP.GitHub.Reporting.Database.Model.Managers
{
    public class CorrelationIdsManager : BaseManager
    {
        public CorrelationIdsManager(DbContext dbContext = null)
            : base(dbContext)
        {
        }

        #region Create
              
        private async Task<CorrelationId> AddAsync(CorrelationId correlationId)
        {
            using (var dbContext = new CSPDatabaseModelEntities())
            {
                CorrelationId newCorrelationId = dbContext.CorrelationIds.Add(correlationId);
                await dbContext.SaveChangesAsync();

                return newCorrelationId;
            }
        }

        public Task<CorrelationId> AddNewRunAsync()
        {
            return this.AddAsync(new CorrelationId()
            {
                Id = Guid.NewGuid(),
                StartDateTime = DateTime.UtcNow,
                Status = "RUNNING"
            });
        }

        #endregion

        #region Update

        private async Task<CorrelationId> UpdateAsync(CorrelationId correlationId)
        {
            using (var dbContext = new CSPDatabaseModelEntities())
            {
                dbContext.Entry(correlationId).State = EntityState.Modified;
                await dbContext.SaveChangesAsync();

                return correlationId;
            }
        }

        public Task<CorrelationId> UpdateEndStatusAsync(Guid correlationId, string endStatus)
        {
            CorrelationId databaseCorrelationId = new CorrelationId()
            {
                Id = correlationId,
                EndDateTime = DateTime.UtcNow,
                Status = endStatus
            };

            return this.UpdateAsync(databaseCorrelationId);
        }

        #endregion

        public void SetSessionContextCorrelationId(Guid correlationId)
        {
            _dbContext.SetCorrelationIdToContext(correlationId.ToString());
        }

        public Guid GetSessionContextCorrelationId()
        {
            ObjectParameter correlationId = new ObjectParameter("CorrelationId", typeof(string));
            _dbContext.GetCorrelationIdFromContext(correlationId);

            return Guid.Parse(correlationId.Value as string);
        }
    }
}
