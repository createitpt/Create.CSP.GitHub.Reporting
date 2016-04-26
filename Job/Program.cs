using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Create.CSP.GitHub.Reporting.Models;
using log4net;
using System.Configuration;
using System.Diagnostics;
using Create.CSP.GitHub.Reporting.Entities;
using PartnerSdkModels = Microsoft.Store.PartnerCenter.Models;
using Create.CSP.GitHub.Reporting.Helpers;
using Create.CSP.GitHub.Reporting.Processors;
using System.Runtime.ExceptionServices;
using System.Net;
using Create.CSP.GitHub.Reporting.Mappers;
using Database = Create.CSP.GitHub.Reporting.Database;
using Microsoft.Store.PartnerCenter;
using GraphClient = Microsoft.Azure.ActiveDirectory.GraphClient;
using Newtonsoft.Json;
using System.IO;

namespace Create.CSP.GitHub.Reporting
{
    public class Program
    {
        #region Members/Properties

        private static AzureADGraphApiHelper _azureADGraphApiHelper { get; set; }
        private static PartnerCenterSdkHelper _partnerCenterSdkHelper { get; set; }
        private static IAggregatePartner _partnerOperations { get; set; }

        private static readonly ILog _logger = LogManager.GetLogger(typeof(Program));
        private static readonly int MAXIMUM_NUMBER_OF_TASKS = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["MaximumNumberOfTasks"]) ?
                                                    2 : int.Parse(ConfigurationManager.AppSettings["MaximumNumberOfTasks"]);
     
        private static readonly int MAXIMUM_CUSTOMERS_PER_BATCH = MAXIMUM_NUMBER_OF_TASKS * 4;
        private static Stopwatch _stopwatch = new Stopwatch();

        private static BlockingCollection<ReportOutputItem> _reportOutputItems = new BlockingCollection<ReportOutputItem>();
        private static BaseReportProcessor _reportProcessor = null;
        private static Guid _correlationId = Guid.Empty;

        #endregion

        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("################################");
                    Console.WriteLine("Help");
                    Console.WriteLine("Extract activation information to CSV: " + System.AppDomain.CurrentDomain.FriendlyName + " activationCSV");
                    Console.WriteLine("Extract customer usage information to CSV: " + System.AppDomain.CurrentDomain.FriendlyName + " customerUsageCSV");
                    Console.WriteLine("Extract activation information to Database: " + System.AppDomain.CurrentDomain.FriendlyName + " activationBD");
                    Console.WriteLine("################################");
                    return;
                }

                _stopwatch.Start();

                _logger.Info("#### Start date: " + DateTime.Now);

                #region Initialize APIs & Tokens

                _azureADGraphApiHelper = new AzureADGraphApiHelper();
                _partnerCenterSdkHelper = new PartnerCenterSdkHelper(isTokenForUserPlusApplication: true);
                _partnerOperations = _partnerCenterSdkHelper.GetPartnerCenterSdkClientAsync().ConfigureAwait(false).GetAwaiter().GetResult();

                #endregion

                #region Parse command parameter

                if (args[0] == "activationCSV")
                {
                    _reportProcessor = new ActivationReportProcessor(OutputChannel.CSV);
                }
                if (args[0] == "customerUsageCSV")
                {
                    _reportProcessor = new CustomerUsageReportProcessor(OutputChannel.CSV);
                }
                if (args[0] == "activationBD")
                {
                    _reportProcessor = new ActivationReportProcessor(OutputChannel.DATABASE);
                }

                ExtractReportInformationWithProcessor().ConfigureAwait(false).GetAwaiter().GetResult();

                #endregion

            }
            catch (Exception ex)
            {
                _logger.Error("Error: " + ex.ToString());
            }
            finally
            {
                _stopwatch.Stop();

                _logger.Info("#### End date: " + DateTime.Now + ". Duration: " + FormatStopwatch());

                //Console.WriteLine("Press any key to end.");
                //Console.ReadLine();
            }
        }

        private static string FormatStopwatch()
        {
            return string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                                    _stopwatch.Elapsed.Hours,
                                    _stopwatch.Elapsed.Minutes,
                                    _stopwatch.Elapsed.Seconds,
                                    _stopwatch.Elapsed.Milliseconds / 10);
        }

        private static async Task ExtractReportInformationWithProcessor()
        {
            ExceptionDispatchInfo capturedException = null;

            try
            {
                #region Output Channel initialization

                _logger.Info("Initializating ...");
                _correlationId = await  _reportProcessor.OutputToChannelInitializeAsync();

                #endregion

                #region Extract report information

                // Process customers in batches
                ListOfEntityResult<PartnerSdkModels.Customers.Customer> customers =
                    new ListOfEntityResult<PartnerSdkModels.Customers.Customer>();
                int currentPage = 1;
                int totalProcessedCustomers = 0;

                do
                {
                    _logger.Info("Current total of processed customers: " + totalProcessedCustomers);
                    _logger.Info("Current elapsed time: " + FormatStopwatch());

                    _logger.Info("Get Batch of Customers: Page = " + currentPage + " Count = " + MAXIMUM_CUSTOMERS_PER_BATCH);

                    // Get batch of customers
                    customers = await GetCustomers(currentPage, MAXIMUM_CUSTOMERS_PER_BATCH, customers.ContinuationToken);
                    totalProcessedCustomers += customers.Items.Count();

                    List<PartnerSdkModels.Customers.Customer> batchOfCustomers = customers.Items.ToList();

                    // Basic threading mechanism, for processing more than one customer at a time           
                    // A MAXIMUM_NUMBER_OF_TASKS will be used
                    // The partition is to guarantee approximate execution order of customers
                    Partitioner.Create(batchOfCustomers).AsParallel().AsOrdered()
                        .WithDegreeOfParallelism(MAXIMUM_NUMBER_OF_TASKS).ForAll(customer =>
                            _reportProcessor.ReportItemProcessorFunctionAsync(
                            _reportOutputItems,
                            batchOfCustomers, customer, currentPage)
                            .ConfigureAwait(false).GetAwaiter().GetResult());

                    // Fill items with correlation Id
                    foreach (var item in _reportOutputItems)
                    {
                        item.CorrelationId = _correlationId;
                        item.Timestamp = DateTime.UtcNow;
                    }

                    // Write this batch to output and clear outputted lines
                    await _reportProcessor.OutputToChannelBatchAsync(_reportOutputItems);

                    _reportOutputItems.Dispose();
                    _reportOutputItems = new BlockingCollection<ReportOutputItem>();

                    // Increase page
                    currentPage++;
                }
                // Check if more customers available
                while (string.IsNullOrWhiteSpace(customers.ContinuationToken) == false);
                
                // Update end status with success  
                await _reportProcessor.OutputToChannelSuccessAsync();    

                _logger.Info("Number of total processed customers: " + totalProcessedCustomers);
            }
            catch (Exception e)
            {
                capturedException = ExceptionDispatchInfo.Capture(e);
            }

            // Because await is not supported in catch block
            if (capturedException != null && _correlationId != null)
            {
                // Update end status with error
                await _reportProcessor.OutputToChannelErrorAsync();               
                capturedException.Throw();
            }

                #endregion
        }
     
        #region API methods

        public static async Task<ListOfEntityResult<PartnerSdkModels.Customers.Customer>> GetCustomers(
            int page, int count, string continuationToken, bool includeCustomerDetails = true)
        {
            // Read customers in page and with received count
            // Max count is 500
            PartnerSdkModels.SeekBasedResourceCollection<PartnerSdkModels.Customers.Customer> customers;
            if (string.IsNullOrWhiteSpace(continuationToken))
            {
                customers = await _partnerOperations.Customers.QueryAsync(
                    PartnerSdkModels.Query.QueryFactory.Instance.BuildIndexedQuery(count, page,
                    token: continuationToken));
            }
            else
            {
                // Use continuation token
                customers = await _partnerOperations.Customers.QueryAsync(
                    PartnerSdkModels.Query.QueryFactory.Instance.BuildSeekQuery(
                    PartnerSdkModels.Query.SeekOperation.Next, token: continuationToken));
            }

            var result = new ListOfEntityResult<PartnerSdkModels.Customers.Customer>()
            {
                Items = customers.Items != null ? customers.Items.ToList()
                        : new List<PartnerSdkModels.Customers.Customer>(0),

                // The last page does not have a next link
                ContinuationToken = customers.Links != null && customers.Links.Next != null ? customers.ContinuationToken : string.Empty
            };

            // Include Customer Details
            if (includeCustomerDetails)
            {
                // Load each customer details
                var tasks = result.Items.Select(async customer => await GetCustomerAsync(Guid.Parse(customer.Id)));
                result.Items = new List<PartnerSdkModels.Customers.Customer>(await Task.WhenAll(tasks));
            }

            // Map to our domain
            return result;
        }

        public static async Task<PartnerSdkModels.Customers.Customer> GetCustomerAsync(Guid customerId)
        {
            return await _partnerOperations.Customers.ById(customerId.ToString()).GetAsync();
        }

        public static List<Domain> GetCustomerDomains(PartnerSdkModels.Customers.Customer customer)
        {
            List<Domain> result = new List<Domain>();

            // Initialize the Azure AD Graph API helper with the customer tenant
            _azureADGraphApiHelper = new AzureADGraphApiHelper(customer.Id);
            // Get request client
            HttpWebRequest request = _azureADGraphApiHelper.BuildGetCustomerDomainsRequest();
            // Execute request
            dynamic requestResult = request.TryCatchRequest();

            foreach (dynamic domain in requestResult.value)
            {
                result.Add(DomainMapper.MapFromJson(customer.Id, domain));
            }

            return result;
        }

        public static async Task<List<PartnerSdkModels.Subscriptions.Subscription>> GetCustomerSubscriptions(
            PartnerSdkModels.Customers.Customer customer)
        {
            // Get the subscriptions for the customer
            return (await _partnerOperations.Customers.ById(customer.Id.ToString()).Subscriptions.GetAsync())
                         .Items.ToList();
        }

        public static async Task<List<PartnerSdkModels.Offers.Offer>> GetCustomerSubscriptionsOffers(
            List<PartnerSdkModels.Subscriptions.Subscription> customerSubscriptions)
        {
            List<PartnerSdkModels.Offers.Offer> result = new List<PartnerSdkModels.Offers.Offer>();

            // Get offer detail for each subscription
            foreach (PartnerSdkModels.Subscriptions.Subscription subscription in customerSubscriptions)
            {
                result.Add(await GetOfferById(subscription.OfferId.ToString()));
            }

            return result;
        }

        public static async Task<List<SubscribedSku>> GetCustomerSubscribedSkus(
            PartnerSdkModels.Customers.Customer customer)
        {
            AzureADGraphApiHelper azureADGraphApiHelper = new AzureADGraphApiHelper(customer.Id);
            GraphClient.ActiveDirectoryClient activeDirectoryClient = azureADGraphApiHelper.GetActiveDirectoryClient();

            GraphClient.Extensions.IPagedCollection<GraphClient.ISubscribedSku> subscribedSkus = await activeDirectoryClient.SubscribedSkus.ExecuteAsync();

            List<SubscribedSku> result = new List<SubscribedSku>();

            do
            {
                foreach (GraphClient.ISubscribedSku subscribedSku in subscribedSkus.CurrentPage.ToList())
                {
                    result.Add(SubscribedSkuMapper.MapFromSource(subscribedSku));
                }

                subscribedSkus = await subscribedSkus.GetNextPageAsync();
            }
            while (subscribedSkus != null && subscribedSkus.MorePagesAvailable);

            return result;
        }

        public static async Task<PartnerSdkModels.Offers.Offer> GetOfferByOfferUri(string offerUri)
        {
            // Extract offer Id from Offer Uri and get offer by id
            return await GetOfferById(offerUri.Split('/').Last());
        }

        public static async Task<PartnerSdkModels.Offers.Offer> GetOfferById(string offerId)
        {
            return await _partnerOperations.Offers.ByCountry(Constants.CSP_COUNTRY_TWO_LETTER_CODE).ById(offerId).GetAsync();
        }

        public static CustomerCsActiveUserReport GetCustomerCsActiveUserReport(
            PartnerSdkModels.Customers.Customer customer)
        {
            string script = string.Format("Get-CsActiveUserReport -ReportType Monthly -ResultSize unlimited -StartDate {0} -EndDate {1}",
                         DateTime.Now.AddDays(-30).ToString("MM/dd/yyyy"), DateTime.Now.ToString("MM/dd/yyyy"));

            var result = PowerShellHelper.ExecuteExchangeOnlinePowerShellScript(customer.Id, script);

            if (result.Count() > 0)
            {
                return CustomerCsActiveUserReportMapper.MapFromObject(Guid.Parse(customer.Id), result[0]);
            }

            // else. Return empty object
            return new CustomerCsActiveUserReport()
            {
                CustomerId = Guid.Parse(customer.Id)
            };
        }

        public static CustomerSPOActiveUserReport GetCustomerSPOActiveUserReport(
            PartnerSdkModels.Customers.Customer customer)
        {
            string script = string.Format("Get-SPOActiveUserReport -ReportType Monthly -ResultSize unlimited -StartDate {0} -EndDate {1}",
                       DateTime.Now.AddDays(-30).ToString("MM/dd/yyyy"), DateTime.Now.ToString("MM/dd/yyyy"));

            var result = PowerShellHelper.ExecuteExchangeOnlinePowerShellScript(customer.Id, script);

            if (result.Count() > 0)
            {
                return CustomerSPOActiveUserReportMapper.MapFromObject(Guid.Parse(customer.Id), result[0]);
            }

            // else. Return empty object
            return new CustomerSPOActiveUserReport()
            {
                CustomerId = Guid.Parse(customer.Id)
            };
        }

        public static CustomerMailboxUsageReport GetCustomerMailboxUsageReport(
            PartnerSdkModels.Customers.Customer customer)
        {
            string script = string.Format("Get-MailboxUsageReport -ResultSize unlimited -StartDate {0} -EndDate {1}",
                          DateTime.Now.AddDays(-30).ToString("MM/dd/yyyy"), DateTime.Now.ToString("MM/dd/yyyy"));

            var result = PowerShellHelper.ExecuteExchangeOnlinePowerShellScript(customer.Id, script);

            if (result.Count() > 0)
            {
                // Only use the most recent day
                return CustomerMailboxUsageReportMapper.MapFromObject(Guid.Parse(customer.Id), result[0]);
            }

            // else. Return empty object
            return new CustomerMailboxUsageReport()
            {
                CustomerId = Guid.Parse(customer.Id)
            };
        }

        public static T Clone<T>(T source) where T : class
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject(serialized, source.GetType()) as T;
        }

        #endregion
    }
}
