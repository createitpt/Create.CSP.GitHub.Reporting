using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DatabaseModel = Create.CSP.GitHub.Reporting.Database.Model;
using Create.CSP.GitHub.Reporting.Entities;
using Create.CSP.GitHub.Reporting.Models;
using log4net;
using PartnerSdkModels = Microsoft.Store.PartnerCenter.Models;
using System.Data.Entity.Validation;
using Create.CSP.GitHub.Reporting.Mappers;
using System.IO;

namespace Create.CSP.GitHub.Reporting.Processors
{
    public class ActivationReportProcessor : BaseReportProcessor
    {   
        private readonly ILog _logger = LogManager.GetLogger(typeof(ActivationReportProcessor));

        protected new readonly string _csvReportFilename = string.Format("{0}_{1}__CSP_ActivationReport.csv",
                          DateTime.Now.ToShortDateString().Replace("/", "_"),
                          DateTime.Now.ToShortTimeString().Replace(":", ""));

        protected new readonly string _csvReportHeader = "CustomerId\tTenantId\tCompanyName\tOnMicrosoftDomain\tDefaultDomain\tTotalDomains\t" +
                "TotalSKUs\tOfferName\tSKUPartNumber\tSKUCapabilityStatus\tTotalSKUSubscriptions\tEarliestSKUSubscription\t" +
                "ActiveSeats\tInGracePeriodSeats\tDisabledSeats\tAssignedSeats\t" +
                "ActionType\tActionSubType";

        private DatabaseModel.Managers.CorrelationIdsManager _correlationIdsManager;

        public ActivationReportProcessor(OutputChannel outputChannel)
            : base(outputChannel)
        {
            _correlationIdsManager = new DatabaseModel.Managers.CorrelationIdsManager();
        }

        protected override string GetCsvReportFilename()
        {
            return _csvReportFilename;
        }

        protected override string GetCsvReportHeader()
        {
            return _csvReportHeader;
        }

        #region Initialization

        protected override async Task<Guid> OutputToDatabaseInitializeAsync()
        {
            // Add new run to the control table           
            DatabaseModel.CorrelationId correlationId = await _correlationIdsManager.AddNewRunAsync();
            _correlationId = correlationId.Id;

            return _correlationId;
        }

        #endregion

        #region Success

        protected override async Task OutputToDatabaseSuccessAsync()
        {
            await _correlationIdsManager.UpdateEndStatusAsync(_correlationId, "SUCCESS");
        }

        #endregion

        #region Error

        protected override async Task OutputToDatabaseErrorAsync()
        {
            await _correlationIdsManager.UpdateEndStatusAsync(_correlationId, "ERROR");
        }

        #endregion
             
        public override async Task ReportItemProcessorFunctionAsync(
            BlockingCollection<ReportOutputItem> results,
            List<PartnerSdkModels.Customers.Customer> customers, PartnerSdkModels.Customers.Customer customer,
            int currentPage)
        {
            _logger.Info(string.Format("Thread: {0} Batch: {1}. Processing customer: {2} ({3}/{4}) / (ID: {5} - CID: {6} - TD: {7})",
                     Thread.CurrentThread.ManagedThreadId, currentPage,
                     customer.CompanyProfile.CompanyName, customers.IndexOf(customer) + 1, customers.Count,
                     customer.Id, customer.CommerceId, customer.CompanyProfile.Domain));

            ActivationReportOutputItem reportOutputItem = new ActivationReportOutputItem()
            {
                Customer = customer
            };

            #region Execute base customer validations

            if (ExecuteReportItemProcessorCustomerBaseValidations(reportOutputItem, results, customer))
            {
                // Matched one of the validations. Return
                return;
            }

            #endregion

            try
            {
                #region Get customer domains

                // NOTE: We always include the customer domains, to include this info in the report,
                // even if the customer does not have any subscribed SKUs

                _logger.Debug("GetCustomerDomains");
                reportOutputItem.CustomerDomains = Program.GetCustomerDomains(customer);

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

                #region Get Subscription Offers information

                // This is necessary to match Subscribed SKU to a Subscription
                // Subscribed SKU has the SKU Product ID field
                // Subscription has the Offer ID field
                // The Offer detail is the connection between them, because the Offer has the Offer ID and the Product ID
                _logger.Debug("GetSubscriptionsOffers");
                List<PartnerSdkModels.Offers.Offer> subscriptionsOffers =
                    await Program.GetCustomerSubscriptionsOffers(customerSubscriptions);

                #endregion

                #region Process each subscribed Sku

                foreach (SubscribedSku subscribedSku in customerSubscribedSkus)
                {
                    #region Extract Sku information

                    _logger.Info(string.Format("Processing subscribedSku: {0} ({1}/{2})",
                        subscribedSku.SkuPartNumber, customerSubscribedSkus.IndexOf(subscribedSku) + 1,
                        customerSubscribedSkus.Count));

                    // Create new subscribed sku report output item
                    ActivationReportSubscribedSKUOutputItem skuAndSubscriptions = new ActivationReportSubscribedSKUOutputItem()
                    {
                        SubscribedSKU = subscribedSku
                    };

                    // Try to match subscribedSku with corresponding subscription offer
                    // to get subscribed sku subscriptions
                    // Get the offer detail that matches the subscribed SKU product ID 
                    PartnerSdkModels.Offers.Offer skuOfferDetail = subscriptionsOffers.FirstOrDefault(offer =>
                        offer.Product.Id.Equals(subscribedSku.SkuId.ToString(),
                        StringComparison.InvariantCultureIgnoreCase));

                    if (skuOfferDetail != null)
                    {
                        // Extract offer and subscribed Sku subscriptions
                        skuAndSubscriptions.Offer = skuOfferDetail;

                        // Filter all the SKU subscriptions by the offer id
                        skuAndSubscriptions.SKUSubscriptions = customerSubscriptions.Where(subscription =>
                            subscription.OfferId.ToString().Equals(skuOfferDetail.Id,
                            StringComparison.InvariantCultureIgnoreCase)).ToList();
                    }
                    else
                    {
                        // No offer detail for the subscribed sku. Could not match subscriptions to subscribed Sku
                        skuAndSubscriptions.Offer = null;
                        skuAndSubscriptions.ActionType = "ACTIVATION OPPORTUNITY";
                        skuAndSubscriptions.ActionSubType = "Not a CSP offer";
                        reportOutputItem.CustomerSubscribedSkuAndSubscriptions.Add(Program.Clone(skuAndSubscriptions));
                        continue;
                    }

                    skuAndSubscriptions.ActiveSeats = subscribedSku.PrepaidUnits.Enabled;
                    skuAndSubscriptions.InGracePeriodSeats = subscribedSku.PrepaidUnits.Warning;
                    skuAndSubscriptions.DisabledSeats = subscribedSku.PrepaidUnits.Suspended;
                    skuAndSubscriptions.AssignedSeats = subscribedSku.ConsumedUnits;

                    if (!subscribedSku.CapabilityStatus.Equals("Enabled", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // Sku capability is different from enabled
                        skuAndSubscriptions.ActionType = "ACTIVATION OPPORTUNITY";
                        skuAndSubscriptions.ActionSubType = "SKU capability status not enabled";
                        // Add processed subscribed SKU to customer 
                        reportOutputItem.CustomerSubscribedSkuAndSubscriptions.Add(Program.Clone(skuAndSubscriptions));
                        continue;
                    }

                    #endregion

                    #region Execute activation action analysis logic

                    #region Check when no seats are assigned

                    if (skuAndSubscriptions.AssignedSeats == 0)
                    {

                        #region Check when no seats are assigned, and no seats are active

                        if (skuAndSubscriptions.ActiveSeats == 0)
                        {
                            // No seats assigned, and not active seats

                            if (skuAndSubscriptions.InGracePeriodSeats == 0)
                            {
                                if (skuAndSubscriptions.DisabledSeats == 0)
                                {
                                    skuAndSubscriptions.ActionType = "ACTIVATION OPPORTUNITY";
                                    skuAndSubscriptions.ActionSubType = "No seats active, assigned, about to expire or disabled";
                                }

                                if (skuAndSubscriptions.DisabledSeats > 0)
                                {
                                    skuAndSubscriptions.ActionType = "NO ACTION NEEDED";
                                    skuAndSubscriptions.ActionSubType = "Waiting for subscription life cycle to de-provision subscription";
                                }
                            }

                            if (skuAndSubscriptions.InGracePeriodSeats > 0)
                            {
                                skuAndSubscriptions.ActionType = "ACTIVATION OPPORTUNITY";
                                skuAndSubscriptions.ActionSubType = "No seats assigned yet, all licenses are about to expire";
                            }

                        }

                        #endregion

                        #region Check when no seats are assigned, but there are active seats

                        if (skuAndSubscriptions.ActiveSeats > 0)
                        {
                            // No seats assigned, but there are active seats

                            if (skuAndSubscriptions.InGracePeriodSeats == 0)
                            {
                                // There are active seats, none about to expire, but no assigned seats
                                skuAndSubscriptions.ActionType = "ACTIVATION OPPORTUNITY";
                                skuAndSubscriptions.ActionSubType = "No seats assigned yet";
                            }

                            if (skuAndSubscriptions.InGracePeriodSeats > 0)
                            {
                                // There are active and about to expire seats, but no assigned seats
                                skuAndSubscriptions.ActionType = "ACTIVATION OPPORTUNITY";
                                skuAndSubscriptions.ActionSubType = "No seats assigned yet, some licenses are about to expire";
                            }
                        }

                        #endregion
                    }

                    #endregion

                    #region Check when there are assigned seats

                    if (skuAndSubscriptions.AssignedSeats > 0)
                    {
                        // Seats are assigned. Check other conditions        

                        #region Check comparison between active and assigned seats

                        if (skuAndSubscriptions.ActiveSeats == skuAndSubscriptions.AssignedSeats &&
                                 skuAndSubscriptions.InGracePeriodSeats == 0)
                        {
                            // There are active seats and all are assigned
                            // No seats are disabled or about to expire
                            skuAndSubscriptions.ActionType = "NO ACTION NEEDED";
                            skuAndSubscriptions.ActionSubType = "All active licenses are assigned";                            
                            reportOutputItem.CustomerSubscribedSkuAndSubscriptions.Add(Program.Clone(skuAndSubscriptions));
                            continue;
                        }

                        if ((skuAndSubscriptions.ActiveSeats + skuAndSubscriptions.InGracePeriodSeats) == skuAndSubscriptions.AssignedSeats)
                        {
                            // The sum of the active and about to expire seats is equal to the assigned seats
                            // These are assigned aeats, and also disabled seats
                            skuAndSubscriptions.ActionType = "ACTION NEEDED";
                            skuAndSubscriptions.ActionSubType = "All licenses assigned. Some licenses are about to expire";
                            reportOutputItem.CustomerSubscribedSkuAndSubscriptions.Add(Program.Clone(skuAndSubscriptions));
                            continue;
                        }

                        if ((skuAndSubscriptions.ActiveSeats + skuAndSubscriptions.InGracePeriodSeats) > skuAndSubscriptions.AssignedSeats)
                        {
                            // The sum of the active and about to expire seats is greater than the assigned seats and
                            // There are assigned seats and no disabled seats
                            skuAndSubscriptions.ActionType = "ACTIVATION OPPORTUNITY";
                            skuAndSubscriptions.ActionSubType = "Not all seats have been assigned yet";
                            reportOutputItem.CustomerSubscribedSkuAndSubscriptions.Add(Program.Clone(skuAndSubscriptions));
                            continue;
                        }

                        if ((skuAndSubscriptions.ActiveSeats + skuAndSubscriptions.InGracePeriodSeats) < skuAndSubscriptions.AssignedSeats)
                        {
                            // The are assigned seats and they are fewer than the active plus the about to expire seats
                            skuAndSubscriptions.ActionType = "ACTION NEEDED";
                            skuAndSubscriptions.ActionSubType = "Customer has more users with licenses than licenses available";
                            reportOutputItem.CustomerSubscribedSkuAndSubscriptions.Add(Program.Clone(skuAndSubscriptions));
                            continue;
                        }

                        #endregion
                    }

                    #endregion

                    // If it didnt match with anything 
                    if (string.IsNullOrWhiteSpace(skuAndSubscriptions.ActionType))
                    {
                        skuAndSubscriptions.ActionType = "Scenario not defined";
                    }

                    // Add processed subscribed SKU to customer 
                    reportOutputItem.CustomerSubscribedSkuAndSubscriptions.Add(Program.Clone(skuAndSubscriptions));

                    #endregion
                }

                // Add customer processing to all results
                results.Add(Program.Clone(reportOutputItem));

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
            if (reportOutputItem.GetType() != typeof(ActivationReportOutputItem))
            {
                throw new ArgumentException("Report Output Item must be of type Activation Report.");
            }

            ActivationReportOutputItem activationReportOutputItem = reportOutputItem as ActivationReportOutputItem;

            StringBuilder result = new StringBuilder();

            // For reference
            //  return "CustomerId\tTenantId\tCompanyName\tOnMicrosoftDomain\tDefaultDomain\tTotalDomains\t" +
            //"TotalSKUs\tOfferName\tSKUPartNumber\tSKUCapabilityStatus\tTotalSKUSubscriptions\tEarliestSKUSubscription\t" +
            //"ActiveSeats\tInGracePeriodSeats\tDisabledSeats\tAssignedSeats\t" +
            //"ActionType\tActionSubType";

            if (activationReportOutputItem.CustomerSubscribedSkuAndSubscriptions.Count == 0)
            {
                // No subscribed Skus. Output at global level
                result.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\n",
                    // Customer / Company info #######
                    activationReportOutputItem.Customer.Id, activationReportOutputItem.Customer.CompanyProfile.TenantId,
                    activationReportOutputItem.Customer.CompanyProfile.CompanyName ?? string.Empty,

                    // Domains info #######
                    // On Microsoft Domain (Initial domain)
                    activationReportOutputItem.CustomerDomains.Count > 0 ? activationReportOutputItem.CustomerDomains
                        .FirstOrDefault(domain => domain.IsInitial && domain.IsVerified).Name : string.Empty,
                    // Default domain
                    activationReportOutputItem.CustomerDomains.Count > 0 ? activationReportOutputItem.CustomerDomains
                        .FirstOrDefault(domain => domain.IsDefault && domain.IsVerified).Name : string.Empty,
                    // Total domains
                    activationReportOutputItem.CustomerDomains.Count(),
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    activationReportOutputItem.GlobalActionType ?? string.Empty,
                    activationReportOutputItem.GlobalActionSubType ?? string.Empty);
            }
            else
            {
                foreach (ActivationReportSubscribedSKUOutputItem subscribedSku in activationReportOutputItem.CustomerSubscribedSkuAndSubscriptions)
                {
                    result.AppendFormat("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\n",

                    // Customer / Company info #######
                    activationReportOutputItem.Customer.Id, activationReportOutputItem.Customer.CompanyProfile.TenantId,
                    activationReportOutputItem.Customer.CompanyProfile.CompanyName ?? string.Empty,

                    // Domains info #######
                        // On Microsoft Domain (Initial domain)
                    activationReportOutputItem.CustomerDomains.Count > 0 ? activationReportOutputItem.CustomerDomains
                        .FirstOrDefault(domain => domain.IsInitial && domain.IsVerified).Name : string.Empty,
                        // Default domain
                    activationReportOutputItem.CustomerDomains.Count > 0 ? activationReportOutputItem.CustomerDomains
                        .FirstOrDefault(domain => domain.IsDefault && domain.IsVerified).Name : string.Empty,
                        // Total domains
                    activationReportOutputItem.CustomerDomains.Count(),

                    // Subscribed Skus info #######
                        // Total SKUs
                        activationReportOutputItem.CustomerSubscribedSkuAndSubscriptions.Count(),
                        // Sku Offer Name
                        subscribedSku.Offer != null ? subscribedSku.Offer.Name : string.Empty,
                        // Sku Part Number
                        subscribedSku.SubscribedSKU.SkuPartNumber ?? string.Empty,
                        // Sku Capability Status
                        subscribedSku.SubscribedSKU.CapabilityStatus ?? string.Empty,
                        // Total Sku Subscriptions
                        subscribedSku.SKUSubscriptions.Count(),
                        // Earliest Sku Subscription
                        subscribedSku.SKUSubscriptions.Count > 0 ?
                            subscribedSku.SKUSubscriptions.OrderBy(subscription => subscription.CreationDate).FirstOrDefault().CreationDate.ToShortDateString() :
                            string.Empty,
                        // Active and In Grace Period Seats
                        subscribedSku.ActiveSeats, subscribedSku.InGracePeriodSeats,
                        // Disabled and Assigned Seats
                        subscribedSku.DisabledSeats, subscribedSku.AssignedSeats,

                        // Action Type
                        subscribedSku.ActionType ?? string.Empty,
                        // Action Subtype
                        subscribedSku.ActionSubType ?? string.Empty);
                }
            }

            return result.ToString();
        }

        protected override async Task OutputToDatabaseBatchAsync(BlockingCollection<ReportOutputItem> reportOutputItems)
        {
            ExceptionDispatchInfo capturedException = null;
            DbContextTransaction dbContextTransaction = null;

            _logger.Info(string.Format("Outputting report items total of: {0}", reportOutputItems.Count));

            if (reportOutputItems.Count == 0)
            {
                // Nothing to output
                return;
            }

            // Cast to proper type
            List<ActivationReportOutputItem> activationReportOutputItems = reportOutputItems.Select(
                item => item as ActivationReportOutputItem).ToList();

            try
            {
                // Create database context with the current correlation id
                using (var dbContext = new DatabaseModel.CSPDatabaseModelEntities(_correlationId))
                {
                    // Begin transaction
                    using (dbContextTransaction = dbContext.Database.BeginTransaction())
                    {
                        #region Batch insert in each table

                        // Customers
                        _logger.Info("Outputting customers ...");

                        // Customers
                        using (var dataCustomersManager = new DatabaseModel.Managers.CustomersManager(dbContext))
                        {
                            dataCustomersManager.AddOrUpdate(activationReportOutputItems
                                .Select(reportItem => CustomerMapper.MapFromSource(reportItem.Customer)));
                        }

                        // Domains
                        _logger.Info("Outputting customers domains ...");
                        using (var dataDomainsManager = new DatabaseModel.Managers.DomainsManager(dbContext))
                        {
                            dataDomainsManager.AddOrUpdate(activationReportOutputItems
                                .SelectMany(
                                reportItem => reportItem.CustomerDomains.Select(customerDomain =>
                                    DomainMapper.MapFromSource(customerDomain, reportItem.Customer.Id))));
                        }

                        // Subscribed SKUs
                        _logger.Info("Outputting customers subscribed skus ...");
                        using (var dataSubscribedSKUsManager = new DatabaseModel.Managers.SubscribedSKUsManager(dbContext))
                        {
                            dataSubscribedSKUsManager.AddOrUpdate(activationReportOutputItems
                                .SelectMany(
                                reportItem => reportItem.CustomerSubscribedSkuAndSubscriptions
                                .Select(customerSubscribedSku => new DatabaseModel.SubscribedSKU()
                                {                                 
                                    Id = customerSubscribedSku.SubscribedSKU.ObjectId,
                                    SkuBusinessId = customerSubscribedSku.SubscribedSKU.SkuId,
                                    PartNumber = customerSubscribedSku.SubscribedSKU.SkuPartNumber,
                                    OfferName =
                                        // Check offer first
                                        customerSubscribedSku.Offer != null ? customerSubscribedSku.Offer.Name : (
                                        // Check subscribed sku part number
                                        !string.IsNullOrWhiteSpace(customerSubscribedSku.SubscribedSKU.SkuPartNumber) ?
                                        customerSubscribedSku.SubscribedSKU.SkuPartNumber : null),
                                    CapabilityStatus = customerSubscribedSku.SubscribedSKU.CapabilityStatus,
                                    CustomerId = Guid.Parse(reportItem.Customer.Id),
                                    ActiveSeats = customerSubscribedSku.ActiveSeats,
                                    InGracePeriodSeats = customerSubscribedSku.InGracePeriodSeats,
                                    DisabledSeats = customerSubscribedSku.DisabledSeats,
                                    AssignedSeats = customerSubscribedSku.AssignedSeats
                                })));
                        }

                        // Subscriptions
                        _logger.Info("Outputting customers subscribed skus associated subscriptions ...");
                        using (var dataSubscriptionsManager = new DatabaseModel.Managers.SubscriptionsManager(dbContext))
                        {
                            dataSubscriptionsManager.AddOrUpdate(activationReportOutputItems
                                .SelectMany(
                                reportItem => reportItem.CustomerSubscribedSkuAndSubscriptions
                                .SelectMany(customerSubscribedSku => customerSubscribedSku.SKUSubscriptions
                                .Select(subscribedSkuSubscription => SubscriptionMapper.MapFromSource(subscribedSkuSubscription,
                                    reportItem.Customer.Id, customerSubscribedSku.SubscribedSKU.ObjectId)))));
                        }

                        // ActivationReport
                        _logger.Info("Outputting customers activation report ...");

                        // At the subscribed Skus level
                        using (var dataActivationReportsManager = new DatabaseModel.Managers.ActivationReportsManager(dbContext))
                        {
                            dataActivationReportsManager.AddActivationReports(activationReportOutputItems
                                .SelectMany(
                                reportItem => reportItem.CustomerSubscribedSkuAndSubscriptions
                                .Select(customerSubscribedSku => new DatabaseModel.ActivationReport()
                                {
                                    CorrelationId = reportItem.CorrelationId,
                                    CustomerId = Guid.Parse(reportItem.Customer.Id),
                                    SubscribedSKUId = customerSubscribedSku.SubscribedSKU.ObjectId,
                                    ActionType = customerSubscribedSku.ActionType,
                                    ActionSubType = customerSubscribedSku.ActionSubType
                                })));

                            // At the customer (global) level
                            dataActivationReportsManager.AddActivationReports(activationReportOutputItems
                               .Where(
                               reportItem => !string.IsNullOrWhiteSpace(reportItem.GlobalActionType))
                               .Select(reportItem => new DatabaseModel.ActivationReport()
                               {
                                   CorrelationId = reportItem.CorrelationId,
                                   CustomerId = Guid.Parse(reportItem.Customer.Id),
                                   ActionType = reportItem.GlobalActionType,
                                   ActionSubType = reportItem.GlobalActionSubType
                               }));
                        }

                        #endregion

                        // Save changes  andCommit transaction
                        await dbContext.SaveChangesAsync();
                        dbContextTransaction.Commit();
                    }
                }
            }
            catch (DbEntityValidationException ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);

                _logger.Warn("DbEntityValidationException: ");

                foreach (var entityError in ex.EntityValidationErrors)
                {
                    _logger.Warn("Entity error: " + string.Join(" / ", entityError.ValidationErrors.Select(e => e.PropertyName + " - " + e.ErrorMessage)));
                }

                if (dbContextTransaction != null)
                {
                    // Rollback transaction
                    dbContextTransaction.Rollback();
                }
            }
            catch (Exception ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);

                _logger.Warn("Error: " + ex.ToString());
                _logger.Warn("Inner Exception: " + ex.InnerException != null ? ex.InnerException.ToString() : "(empty)");

                if (dbContextTransaction != null)
                {
                    // Rollback transaction
                    dbContextTransaction.Rollback();
                }
            }
            finally
            {
            }
        }

    }
}
