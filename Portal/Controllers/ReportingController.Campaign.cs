using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ReportingPortal.Domain;
using ReportingPortal.Managers;
using ReportingPortal.Models;

using PartnerSdkModels = Microsoft.Store.PartnerCenter.Models;

namespace ReportingPortal.Controllers
{
    public partial class ReportingController : Controller
    {
        public ActionResult Campaign(string campaignId = null)
        {
            ReportingCampaignViewModel model = new ReportingCampaignViewModel();

            CustomerManager customerManager = new CustomerManager();
            model.Customers = customerManager.GetAll();

            Campaign campaign = new Campaign();

            if (!string.IsNullOrEmpty(campaignId))
            {            
                MarketingCampaignManager marketingCampaignManager = new MarketingCampaignManager();
                model.Campaign = marketingCampaignManager.GetCampaign(campaignId);
            }  

            if(model.Campaign != null)
            {
                model.EditRecord = true;
            }
            else
            {
                model.EditRecord = false;
                model.Campaign = campaign;
            }          

            return View(model);
        }

        [HttpPost]
        public ActionResult Campaign(ReportingCampaignViewModel model)
        {
            MarketingCampaignManager marketingCampaignManager = new MarketingCampaignManager();
            List<PartnerSdkModels.Customers.Customer> customers = new List<PartnerSdkModels.Customers.Customer>();

            if (model.CustomersCampaign == null || model.CustomersCampaign.Count == 0)
            {
                //The url passed is unsupported
                throw new HttpException((int)HttpStatusCode.BadRequest, "Without Customers");
            }

            model.RecordIsAddedOrUpdated = false;

            customers = model.CustomersCampaign.Select(x => new PartnerSdkModels.Customers.Customer()
                {
                   Id = x.CustomerId 
                }).ToList();

            model.Campaign.Customers = customers;

            Campaign campaignAdded = marketingCampaignManager.Create(model.Campaign);

            if(campaignAdded != null && !string.IsNullOrEmpty(campaignAdded.CampaignId))
            {
                model.Campaign = campaignAdded;
                model.RecordIsAddedOrUpdated = true;
            }

            return Json(Url.Action("CampaignManagement", "Reporting"));
        }

        public ActionResult DeleteCampaign(string campaignId)
        {
            MarketingCampaignManager marketingCampaignManager = new MarketingCampaignManager();
            marketingCampaignManager.DeleteCampaign(campaignId);

            return RedirectToAction("CampaignManagement");
        }       
    }
}
