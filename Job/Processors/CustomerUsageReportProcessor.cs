using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Create.CSP.GitHub.Reporting.Entities;
using Create.CSP.GitHub.Reporting.Models;
using log4net;
using PartnerSdkModels = Microsoft.Store.PartnerCenter.Models;

namespace Create.CSP.GitHub.Reporting.Processors
{
    public class CustomerUsageReportProcessor : BaseReportProcessor
    {    
        private readonly ILog _logger = LogManager.GetLogger(typeof(ActivationReportProcessor));

        protected new readonly string _csvReportFilename = string.Format("{0}_{1}__CSP_CustomerUsageReport.csv",
                    DateTime.Now.ToShortDateString().Replace("/", "_"),
                    DateTime.Now.ToShortTimeString().Replace(":", ""));

        protected new readonly string _csvReportHeader = "CustomerId\tTenantId\tCompanyName\tOnMicrosoftDomain\tDefaultDomain\tSubscriptionID\t" +
                     "SubscriptionStatus\tSubscriptionQuantity\tOfferName\tSKUPartNumber\tSKUTotalServices\t" +
                     "SKUService\tSKUServiceProvisioningStatus\tSKUAssignedSeats\tSKUServiceActiveUsers\t" +
                     "ActionType\tActionSubType";

        public CustomerUsageReportProcessor(OutputChannel outputChannel)
            : base(outputChannel)
        {
        }

        protected override string GetCsvReportFilename()
        {
            return _csvReportFilename;
        }

        protected override string GetCsvReportHeader()
        {
            return _csvReportHeader;
        }
       
        public override async Task ReportItemProcessorFunctionAsync(
            BlockingCollection<ReportOutputItem> results,
            List<PartnerSdkModels.Customers.Customer> customers, PartnerSdkModels.Customers.Customer customer,
            int currentPage)
        {
            _logger.Info(string.Format("Thread: {0} Batch: {1}. Processing customer: {2} ({3}/{4}) / (ID: {5} - CID: {6} - TD: {7})",
                    Thread.CurrentThread.ManagedThreadId, currentPage,
                    customer.CompanyProfile.CompanyName, customers.IndexOf(customer) + 1, customers.Count,
                    customer.Id, customer.CommerceId, customer.CompanyProfile.Domain));

            CustomerUsageReportOutputItem reportOutputItem = new CustomerUsageReportOutputItem()
            {
                Customer = customer
            };

            #region Execute base customer validations

            if (ExecuteReportItemProcessorCustomerBaseValidations(reportOutputItem, results, customer))
            {
                // Customer matched one of the validations. Return
                return;
            }

            #endregion

            try
            {
                #region Get customer domains

                // NOTE: We always include the customer domains, to include this info in the report,
                // even if the customer does not have any subscribed SKUs

                _logger.Debug("GetCustomerDomains");
                List<Domain> customerDomains = Program.GetCustomerDomains(customer);
                // Extract domains information
                reportOutputItem.OnMicrosoftDomain = customerDomains.FirstOrDefault(domain => domain.IsInitial && domain.IsVerified).Name;
                reportOutputItem.DefaultDomain = customerDomains.FirstOrDefault(domain => domain.IsDefault && domain.IsVerified).Name;

                #endregion

                #region Get Subscribed Skus information

                _logger.Debug("GetCustomerSubscribedSkus");
                List<SubscribedSku> customerSubscribedSkus = await Program.GetCustomerSubscribedSkus(customer);

                #region Execute base subscribed Skus validations

                if (ExecuteReportItemProcessorSubscribedSkusBaseValidations(reportOutputItem, results, customer, customerSubscribedSkus))
                {
                    // Matched one of the validations. Return
                    return;
                }

                #endregion

                #endregion

                #region Get Subscriptions information

                _logger.Debug("GetCustomerSubscriptions");
                List<PartnerSdkModels.Subscriptions.Subscription> customerSubscriptions =
                    await Program.GetCustomerSubscriptions(customer);

                #region Execute base subscriptions validations

                if (ExecuteReportItemProcessorSubscriptionsBaseValidations(reportOutputItem, results, customer, customerSubscriptions))
                {
                    // Matched one of the validations. Return
                    return;
                }

                #endregion

                #endregion

                #region Extract information from each subscription

                foreach (PartnerSdkModels.Subscriptions.Subscription subscription in customerSubscriptions)
                {
                    _logger.Info(string.Format("Processing subscription: {0} ({1}/{2})",
                        subscription.FriendlyName, customerSubscriptions.IndexOf(subscription) + 1,
                        customerSubscriptions.Count));

                    reportOutputItem.SubscriptionId = Guid.Parse(subscription.Id);
                    reportOutputItem.SubscriptionStatus = subscription.Status.ToString();
                    reportOutputItem.SubscriptionQuantity = (uint)subscription.Quantity;

                    if (subscription.Status != PartnerSdkModels.Subscriptions.SubscriptionStatus.Active)
                    {
                        // Subscription is not active. Nothing to do
                        reportOutputItem.GlobalActionType = "ACTIVATION OPPORTUNITY";
                        reportOutputItem.GlobalActionSubType = "Subscription state: " + subscription.Status.ToString();
                        results.Add(Program.Clone(reportOutputItem));
                        return;
                    }

                    // Subscription Offer information
                    // This is necessary to match Subscribed SKU to a Subscription
                    // Subscribed SKU has the SKU Product ID field
                    // Subscription has the Offer ID field
                    // The Offer detail is the connection between them, because the Offer has the Offer ID and the Product ID
                    _logger.Debug("GetSubscriptionOffer");
                    PartnerSdkModels.Offers.Offer subscriptionOffer = await Program.GetOfferById(subscription.OfferId.ToString());
                    reportOutputItem.OfferName = subscriptionOffer.Name;

                    // Subscription SKU
                    SubscribedSku subscriptionSubscribedSku = customerSubscribedSkus.FirstOrDefault(sku =>
                            sku.SkuId.ToString().Equals(subscriptionOffer.Product.Id, StringComparison.InvariantCultureIgnoreCase));

                    if (subscriptionSubscribedSku == null)
                    {
                        reportOutputItem.GlobalActionType = "Not supported";
                        reportOutputItem.GlobalActionSubType = "Subscription Offer does not map to any CSP Subscribed SKU Product";
                        results.Add(Program.Clone(reportOutputItem));
                        continue;
                    }

                    reportOutputItem.SKUPartNumber = subscriptionSubscribedSku.SkuPartNumber;
                    reportOutputItem.SKUTotalServices = subscriptionSubscribedSku.ServicePlans.Count();
                    reportOutputItem.SKUAssignedSeats = subscriptionSubscribedSku.ConsumedUnits;

                    #region Extract global user's information (in terms of number of users, not detailed user's information)

                    // For tenant level information
                    // Ex: Subscription is for 20 seats, but only 5 are using each of the services?

                    foreach (ServicePlanInfo servicePlan in subscriptionSubscribedSku.ServicePlans)
                    {
                        _logger.Info(string.Format("Processing SKU Service Plan: {0} ({1}/{2})",
                            servicePlan.ServicePlanName, subscriptionSubscribedSku.ServicePlans.IndexOf(servicePlan) + 1,
                            subscriptionSubscribedSku.ServicePlans.Count));

                        reportOutputItem.SKUService = servicePlan.ServicePlanName;
                        reportOutputItem.SKUServiceProvisioningStatus = servicePlan.ProvisioningStatus.ToString();

                        switch (servicePlan.ServicePlanName)
                        {
                            case "EXCHANGE_S_STANDARD":
                            case "EXCHANGE_S_DESKLESS":
                                CustomerMailboxUsageReport customerMailboxUsageReport = Program.GetCustomerMailboxUsageReport(customer);
                                reportOutputItem.SKUServiceActiveUsers = customerMailboxUsageReport.TotalMailboxCount;
                                break;

                            case "MCOSTANDARD":
                                CustomerCsActiveUserReport customerCsActiveUserReport = Program.GetCustomerCsActiveUserReport(customer);
                                reportOutputItem.SKUServiceActiveUsers = customerCsActiveUserReport.ActiveUsers;
                                break;

                            case "SHAREPOINTSTANDARD":
                                CustomerSPOActiveUserReport customerSPOActiveUserReport = Program.GetCustomerSPOActiveUserReport(customer);
                                reportOutputItem.SKUServiceActiveUsers = customerSPOActiveUserReport.UniqueUsers;

                                // Also has OneDrive ?????
                                // CustomerSPOSkyDriveProDeployedReport customerSPOSkyDriveProDeployedReport = await Program.GetCustomerSPOSkyDriveProDeployedReport(customer);
                                //reportOutputItem.SKUServiceActiveUsers = customerSPOSkyDriveProDeployedReport.Active; 
                                break;

                            case "SHAREPOINTWAC":
                            case "OFFICE_BUSINESS":
                            case "OFFICESUBSCRIPTION":
                            case "ONEDRIVEENTERPRISE":
                            case "ONEDRIVESTANDARD":
                            case "SWAY":
                            case "INTUNE_O365":
                            case "YAMMER_ENTERPRISE":
                            default:
                                reportOutputItem.GlobalActionType = "Not supported";
                                reportOutputItem.GlobalActionSubType = "Service Plan information extraction not supported";
                                results.Add(Program.Clone(reportOutputItem));
                                continue;
                        }

                        if (reportOutputItem.SKUServiceActiveUsers < reportOutputItem.SKUAssignedSeats)
                        {
                            reportOutputItem.GlobalActionType = "ACTIVATION OPPORTUNITY";
                            reportOutputItem.GlobalActionSubType = "Not all users are active on the service";
                        }
                        else
                        {
                            reportOutputItem.GlobalActionType = "NO ACTION NEEDED";
                            reportOutputItem.GlobalActionSubType = "All users are active on the service";
                        }

                        results.Add(Program.Clone(reportOutputItem));
                    }

                    #endregion
                }

                #endregion
            }
            catch (Exception ex)
            {
                _logger.Warn("Error: " + ex.ToString());

                reportOutputItem.GlobalActionType = "ERROR";
                reportOutputItem.GlobalActionSubType = ex.ToString().Replace("\r\n", " ").Replace("\n", " ").Replace("\t", " ");
                results.Add(Program.Clone(reportOutputItem));
            }
        }             
            
        protected override string GetCsvReportItemFormated(ReportOutputItem reportOutputItem)
        {
            if (reportOutputItem.GetType() != typeof(CustomerUsageReportOutputItem))
            {
                throw new ArgumentException("Report Output Item must be of type CustomerUsageReportOutputItem");
            }

            CustomerUsageReportOutputItem customerUsageReportOutputItem = reportOutputItem as CustomerUsageReportOutputItem;

            StringBuilder result = new StringBuilder();
                      
            result.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}",
                            customerUsageReportOutputItem.Customer.Id, customerUsageReportOutputItem.Customer.CompanyProfile.TenantId,
                            customerUsageReportOutputItem.Customer.CompanyProfile.CompanyName, customerUsageReportOutputItem.OnMicrosoftDomain,
                            customerUsageReportOutputItem.DefaultDomain, customerUsageReportOutputItem.SubscriptionId,
                            customerUsageReportOutputItem.SubscriptionStatus, customerUsageReportOutputItem.SubscriptionQuantity,
                            customerUsageReportOutputItem.OfferName, customerUsageReportOutputItem.SKUPartNumber,
                            customerUsageReportOutputItem.SKUTotalServices,
                            customerUsageReportOutputItem.SKUService, customerUsageReportOutputItem.SKUServiceProvisioningStatus,
                            customerUsageReportOutputItem.SKUAssignedSeats.ToString(), customerUsageReportOutputItem.SKUServiceActiveUsers.ToString(),
                            customerUsageReportOutputItem.GlobalActionType, customerUsageReportOutputItem.GlobalActionSubType);

            return result.ToString();
        }

        #region Database Output Channel

        protected override Task<Guid> OutputToDatabaseInitializeAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task OutputToDatabaseSuccessAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task OutputToDatabaseErrorAsync()
        {
            throw new NotImplementedException();
        }

        protected override Task OutputToDatabaseBatchAsync(BlockingCollection<ReportOutputItem> reportOutputItems)
        {
            throw new NotImplementedException();
        }

        #endregion     
    }
}
