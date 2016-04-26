using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Create.CSP.GitHub.Reporting.Database.Model;
using ReportingPortal.Domain;
using ReportingPortal.Mappers;
using DataManagers = Create.CSP.GitHub.Reporting.Database.Model.Managers;

namespace ReportingPortal.Managers
{
    public class MarketingCampaignManager
    {
        #region Constructor

        public MarketingCampaignManager()
        {
        }

        public Campaign Create(Campaign campaign)
        {
            Campaign campaignResult = null;
            MarketingCampaign marketingCampaign = MarketingCampaignMapper.MapFromSource(campaign);

            using (var dbContext = new CSPDatabaseModelEntities())
            {
                using (var dataCampaignManager = new DataManagers.MarketingCampaignManager(dbContext))
                {
                    MarketingCampaign campaignDb = dataCampaignManager.AddOrUpdate<MarketingCampaign>(marketingCampaign, c => c.Id.ToString() == campaign.CampaignId);
                    
                    MarketingCampaign campaignNew = dbContext.MarketingCampaigns.Include(c => c.MarketingCampaignsCustomers)
                        .Include(c => c.MarketingCampaignsCustomers.Select(x => x.Customer)).FirstOrDefault(c => c.Id == campaignDb.Id);
                    
                    campaignResult = MarketingCampaignMapper.MapFromSource(campaignNew);
                }
            }

            return campaignResult;
        }

        public List<Campaign> GetAll()
        {
            List<Campaign> campaigns = new List<Campaign>();

            using (var dbContext = new CSPDatabaseModelEntities())
            {
                using (var dataCampaignManager = new DataManagers.MarketingCampaignManager(dbContext))
                {
                    List<MarketingCampaign> campaignsDb = dataCampaignManager.GetAll<MarketingCampaign>(null, c => c.MarketingCampaignsCustomers).ToList();

                    campaigns = campaignsDb != null
                                   ? campaignsDb.Select(x => MarketingCampaignMapper.MapFromSource(x)).ToList()
                                   : null;
                }
            }

            return campaigns;
        }

        public Campaign GetCampaign(string campaignId)
        {
            Campaign campaign = new Campaign();

            using (var dbContext = new CSPDatabaseModelEntities())
            {
                using (var dataCampaignManager = new DataManagers.MarketingCampaignManager(dbContext))
                {
                    MarketingCampaign campaignsDb = dataCampaignManager.FirstOrDefaultBy<MarketingCampaign>(c => c.Id != null
                    && c.Id.ToString() == campaignId, c => c.MarketingCampaignsCustomers);

                    campaign = campaignsDb != null
                                   ? MarketingCampaignMapper.MapFromSource(campaignsDb)
                                   : null;
                }
            }

            return campaign;
        }

        public void DeleteCampaign(string campaignId)
        {
            ExceptionDispatchInfo capturedException = null;
            DbContextTransaction dbContextTransaction = null;

            Guid id;
            MarketingCampaign marketingCampaign = new MarketingCampaign()
            {
                Id = !string.IsNullOrEmpty(campaignId) && Guid.TryParse(campaignId, out id) ? id : Guid.Empty
            };
            try
            {
                using (var dbContext = new CSPDatabaseModelEntities())
                {
                    // Begin transaction
                    using (dbContextTransaction = dbContext.Database.BeginTransaction())
                    {
                        using (var dataCampaignsCustomerManager = new DataManagers.MarketingCampaignsCustomer(dbContext))
                        {
                            dataCampaignsCustomerManager.DeleteWhere<MarketingCampaignsCustomer>(c => c.MarketingCampaignId.ToString() == campaignId);
                        }

                        using (var dataCampaignManager = new DataManagers.MarketingCampaignManager(dbContext))
                        {
                            dataCampaignManager.DeleteWhere<MarketingCampaign>(c => c.Id.ToString() == campaignId);
                        }

                        dbContextTransaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);

                if (dbContextTransaction != null)
                {
                    // Rollback transaction
                    dbContextTransaction.Rollback();
                }
            }
        }

        #endregion
    }
}
