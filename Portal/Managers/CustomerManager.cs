using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Create.CSP.GitHub.Reporting.Database.Model;
using ReportingPortal.Mappers;
using DataManagers = Create.CSP.GitHub.Reporting.Database.Model.Managers;
using PartnerSdkModels = Microsoft.Store.PartnerCenter.Models;

namespace ReportingPortal.Managers
{
    public class CustomerManager
    {
        public CustomerManager()
        {
        }

        public List<PartnerSdkModels.Customers.Customer> GetAll()
        {
            List<PartnerSdkModels.Customers.Customer> customers = new List<PartnerSdkModels.Customers.Customer>();

            using (var dbContext = new CSPDatabaseModelEntities())
            {
                using (var dataCampaignManager = new DataManagers.MarketingCampaignManager(dbContext))
                {
                    List<Customer> customersDb = dataCampaignManager.GetAll<Customer>().ToList();

                    customers = customersDb != null
                                   ? customersDb.Select(x => CustomerMapper.MapFromSource(x)).ToList()
                                   : null;
                }
            }

            return customers;
        }
    }
}
