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
            CorrelationId newCorrelationId = _dbContext.CorrelationIds.Add(correlationId);
            await _dbContext.SaveChangesAsync();

            return newCorrelationId;
        }

        public Task<CorrelationId> AddNewRunAsync()
        {
            return this.AddAsync(new CorrelationId()
            {
                Id = Guid.NewGuid(),
                StartDateTime = DateTime.UtcNow,
                Status = "RUNNING",
                EndDateTime = null
            });
        }

        #endregion

        #region Update

        private async Task<CorrelationId> UpdateAsync(CorrelationId correlationId)
        {

            _dbContext.Entry(correlationId).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return correlationId;
        }

        public Task<CorrelationId> UpdateEndStatusAsync(Guid correlationId, string endStatus)
        {

            // Get object to update
            var existentCorrelation = _dbContext.CorrelationIds.FirstOrDefault(c => c.Id == correlationId);
            if (existentCorrelation == null)
            {
                throw new ArgumentException("Could not find existent correlation id: " + correlationId);
            }

            // else. Update
            existentCorrelation.EndDateTime = DateTime.UtcNow;
            existentCorrelation.Status = endStatus;

            return this.UpdateAsync(existentCorrelation);

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
