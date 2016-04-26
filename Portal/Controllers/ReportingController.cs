namespace ReportingPortal.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using ReportingPortal.Domain;
    using ReportingPortal.Models;
    using Managers;
    public partial class ReportingController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ActivationReport()
        {
            return View();
        }

        public ActionResult CampaignManagement()
        {
            ReportingCampaignIndexViewModel model = new ReportingCampaignIndexViewModel();

            IList<Campaign> campaigns = new List<Campaign>();

            MarketingCampaignManager marketingCampaignManager = new MarketingCampaignManager();

            campaigns = marketingCampaignManager.GetAll();

            model.Campaigns = campaigns;

            return View(model);
        }

        public ActionResult EngineLog()
        {
            return View();
        }
    }
}
