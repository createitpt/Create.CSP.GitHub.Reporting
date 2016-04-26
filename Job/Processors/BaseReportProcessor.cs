using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Create.CSP.GitHub.Reporting.Entities;
using Create.CSP.GitHub.Reporting.Models;
using log4net;
using PartnerSdkModels = Microsoft.Store.PartnerCenter.Models;

namespace Create.CSP.GitHub.Reporting.Processors
{
    public enum OutputChannel
    {
        CSV,
        DATABASE
    };

    public abstract class BaseReportProcessor
    {
        private OutputChannel _outputChannel;
        protected Guid _correlationId;

        private readonly ILog _logger = LogManager.GetLogger(typeof(BaseReportProcessor));

        protected readonly string _csvReportFilename = string.Empty;
        protected readonly string _csvReportHeader = string.Empty;

        public BaseReportProcessor(OutputChannel outputChannel)
        {
            _outputChannel = outputChannel;
        }

        public async Task<Guid> OutputToChannelInitializeAsync()
        {
            switch (_outputChannel)
            {
                case OutputChannel.CSV:
                    _correlationId = await OutputToCsvInitializeAsync();
                    break;

                case OutputChannel.DATABASE:
                    _correlationId = await OutputToDatabaseInitializeAsync();
                    break;

                default:
                    throw new ArgumentException("Output channel not valid.");
            }

            return _correlationId;
        }

        public Task OutputToChannelSuccessAsync()
        {
            switch (_outputChannel)
            {
                case OutputChannel.CSV:
                    return OutputToCsvSuccessAsync();

                case OutputChannel.DATABASE:
                    return OutputToDatabaseSuccessAsync();

                default:
                    throw new ArgumentException("Output channel not valid.");
            }
        }

        public Task OutputToChannelErrorAsync()
        {
            switch (_outputChannel)
            {
                case OutputChannel.CSV:
                    return OutputToCsvErrorAsync();

                case OutputChannel.DATABASE:
                    return OutputToDatabaseErrorAsync();

                default:
                    throw new ArgumentException("Output channel not valid.");
            }
        }

        public Task OutputToChannelBatchAsync(BlockingCollection<ReportOutputItem> reportOutputItems)
        {
            switch (_outputChannel)
            {
                case OutputChannel.CSV:
                    return OutputToCsvBatchAsync(reportOutputItems);

                case OutputChannel.DATABASE:
                    return OutputToDatabaseBatchAsync(reportOutputItems);

                default:
                    throw new ArgumentException("Output channel not valid.");
            }
        }

        public abstract Task ReportItemProcessorFunctionAsync(
                             BlockingCollection<ReportOutputItem> results,
                             List<PartnerSdkModels.Customers.Customer> customers,
                             PartnerSdkModels.Customers.Customer customer,
                             int currentPage);

        protected bool ExecuteReportItemProcessorCustomerBaseValidations(
            ReportOutputItem reportOutputItem,
            BlockingCollection<ReportOutputItem> results,
            PartnerSdkModels.Customers.Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.Id))
            {
                reportOutputItem.GlobalActionType = "NO ACTION NEEDED";
                reportOutputItem.GlobalActionSubType = "Customer is deleted or without relationship";
                results.Add(Program.Clone(reportOutputItem));
                return true;
            }

            if (customer.RelationshipToPartner != PartnerSdkModels.Customers.CustomerPartnerRelationship.Reseller)
            {
                // TODO: Check other kind of relationships
                reportOutputItem.GlobalActionType = "ACTIVATION OPPORTUNITY";
                reportOutputItem.GlobalActionSubType = "Customer relationship to partner different from reseller";
                results.Add(Program.Clone(reportOutputItem));
                return true;
            }

            // Didnt matched any validation
            return false;
        }

        protected bool ExecuteReportItemProcessorSubscribedSkusBaseValidations(
           ReportOutputItem reportOutputItem,
           BlockingCollection<ReportOutputItem> results,
           PartnerSdkModels.Customers.Customer customer,
           List<SubscribedSku> customerSubscribedSkus)
        {
            if (customerSubscribedSkus.Count == 0)
            {
                // Customer does not have any subscribed SKUs
                reportOutputItem.GlobalActionType = "ACTIVATION OPPORTUNITY";
                reportOutputItem.GlobalActionSubType = "Customer does not have any subscribed SKUs";
                results.Add(Program.Clone(reportOutputItem));
                return true;
            }

            // Didnt matched any validation
            return false;
        }

        protected bool ExecuteReportItemProcessorSubscriptionsBaseValidations(
         ReportOutputItem reportOutputItem,
         BlockingCollection<ReportOutputItem> results,
         PartnerSdkModels.Customers.Customer customer,
         List<PartnerSdkModels.Subscriptions.Subscription> customerSubscriptions)
        {
            if (customerSubscriptions.Count == 0)
            {
                // Customer does not have any subscribed SKUs
                reportOutputItem.GlobalActionType = "ACTIVATION OPPORTUNITY";
                reportOutputItem.GlobalActionSubType = "Customer does not have any subscriptions";
                results.Add(Program.Clone(reportOutputItem));
                return true;
            }

            // Didnt matched any validation
            return false;
        }

        #region For CSV Output Channel

        protected async Task<Guid> OutputToCsvInitializeAsync()
        {
            _correlationId = Guid.NewGuid();

            // Clear / Create new file
            using (var w = new StreamWriter(GetCsvReportFilename()))
            {
                // Append headers
                await w.WriteLineAsync(GetCsvReportHeader());
                w.Flush();
            }

            return _correlationId;
        }
        protected Task OutputToCsvSuccessAsync()
        {
            return Task.Delay(0);
        }

        protected Task OutputToCsvErrorAsync()
        {
            return Task.Delay(0);
        }

        protected async Task OutputToCsvBatchAsync(BlockingCollection<ReportOutputItem> reportOutputItems)
        {
            if (reportOutputItems.Count() == 0)
            {
                // Nothing to output
                return;
            }

            _logger.Info("Outputting report lines to filename: " + GetCsvReportFilename());

            // Write report output
            using (var w = new StreamWriter(GetCsvReportFilename(), true))
            {
                foreach (ReportOutputItem reportOutputItem in reportOutputItems)
                {
                    try
                    {                     
                        _logger.Info(string.Format("Outputting customer: {0} ({1}/{2})",
                            reportOutputItem.Customer.CompanyProfile.CompanyName,
                            reportOutputItems.ToList().IndexOf(reportOutputItem) + 1,
                            reportOutputItems.Count));

                        await w.WriteLineAsync(GetCsvReportItemFormated(reportOutputItem));
                    }
                    catch (Exception ex)
                    {
                        _logger.Warn("Error: " + ex.ToString());

                        w.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}",
                           reportOutputItem.Customer.Id, reportOutputItem.Customer.CompanyProfile.TenantId,
                           reportOutputItem.Customer.CompanyProfile.CompanyName,
                           ex.Message.Replace("\r\n", " ").Replace("\n", " ").Replace("\t", " ")));
                    }
                }

                w.Flush();
            }
        }

        protected abstract string GetCsvReportFilename();

        protected abstract string GetCsvReportHeader();
      
        protected abstract string GetCsvReportItemFormated(ReportOutputItem reportOutputItem);
      
        #endregion

        #region For Database Output Channel

        protected abstract Task<Guid> OutputToDatabaseInitializeAsync();
        protected abstract Task OutputToDatabaseSuccessAsync();
        protected abstract Task OutputToDatabaseErrorAsync();

        protected abstract Task OutputToDatabaseBatchAsync(BlockingCollection<ReportOutputItem> reportOutputItems);

        #endregion

    }
}
