# Create.CSP.Reporting
|create|it| Microsoft Cloud Solution Provider (CSP) Reporting Samples

This is a CSP Reporting sample application, intended for Microsoft CSP Partners, that provides Customer Insights (CI) and Opportunities Identification (OI) for Office 365 (seat based licensing). 

The analysis made by the sample, complements and details the information that is available in the CSP Parter Center Portal, and also shows how with a basic set of rules the CSP customer basis can be segmented and classified in terms of:
* Activation Opportunities - Seat related opportunities (e.g. A customer purchased 10 E3 seats but only 5 of them are assigned)
* Usage Opportunities - Service usage opportunities (e.g From the 5 assigned E3 seats, 2 licenses are not using Skype for Business)

Supplied with this information, the CSP Partner can for example determine and prioritize the customers that should be targeted by a Marketing Campaign, and/or work with their Sales and Support teams, to get these customers to make a better use of their purchased licenses and services.

The sample is composed by:
 * A Reporting Job - Extracts and Analysis the customer information, that results in the Opportunities Identification. Can output information to CSV files or to a database, allowing in the case of the database to store and compare customer historic information.
 * A Reporting Portal (optional) - Provides a basic mechanism for managing the customers that are targeted by a campaign, allowing the filtering of the report to the campaign customer base. The actual management and sending of these campaigns are not in the scope of these samples.
 * Power BI Reports (optional) - Provides reporting samples that present the extracted and analysed information
 
# Installation
(Note: The instructions below require some prior technical knowledge to complete.)

1. Download the binary file from the latest project release
2. Uncompress the files to a folder
3. Configure the Reporting Job (see corresponding sections below for details):
   * CSP Tenant settings
   * CSP Reporting Database
4. (optional) With the provided SQL Script, create a new Reporting database (if you want to have historic information, you will need this. Otherwise you will only be able to export to CSV files)
5. (optional) Install the Reporting Portal to a IIS Website. .Net 4.6.1 and ASP .Net MVC 5 required. Configure the Reporting Portal (see corresponding sections below for details):
   * CSP Reporting Database
6. (optional) Install the Power BI reports to a Power BI enabled Office 365 tenant. Configure the reports:
    * CSP CSV files
    * CSP Reporting Database

# Configuration
### CSP Tenant Settings
The Reporting Job has several configurable settings in the app.config file. Set the values as appropriate for your tenant.

| Configuration Key  | Description |
| ------------- | ------------- |
| CSPTenantName  | The default domain of the CSP Tenant with Microsoft. (This is typically an "onmicrosoft.com" domain.)  |
| CSPTenantId  | The Microsoft Id of the CSP Tenant. This can be retrieved from the Partner Center Portal.  |
| AzureADAppId-NativeApplication  | The id of the native tenant application registered in the CSP AD directory with permissions to access the Partner Center SDK API (see https://msdn.microsoft.com/en-us/library/partnercenter/mt267552.aspx - section Enable API access; and/or https://msdn.microsoft.com/en-us/library/partnercenter/mt634709.aspx  - section Configure authentication for Partner Center APIs) |
| CSPAdminUsername  | Partner Center service account username. A Helpdesk agent account type is sufficient.  |
| CSPAdminPassword  | Partner Center service account password. The previous account password.|
| CSPCountryTwoLetterCode  | The country two letter code where the CSP tenant is registered. Example: US or UK or PT|

### CSP Reporting Database
The Reporting Job and the Reporting Portal both need a database connection to work. If you choose to not use a database you can still use the job to export the reporting information to CSV files.

| Configuration Key  | Description |
| ------------- | ------------- |
| CSPDatabaseModelEntities connection string | The connection string to the Reporting Database.  |
| Logging connection string | The connection string to the Reporting Database. tipically it refers to the same database as above.  |

# Usage
### Reporting Job
The Reporting Job is a console application that accepts a single parameter. That parameter can be:

| Parameter  | Description |
| ------------- | ------------- |
| activationCSV | Extracts and generates the Activation Opportunities report to a CSV file.  |
| customerUsageCSV | Extracts and generates the Usage Opportunities report to a CSV file.  |
| activationBD | Extracts and generates the Activation Opportunities report to the Reporting database.  |

The generated CSV files have an header line, all fields are separated by the tab (\t) character, and there is no text qualifier surrounding fields.

Note: It can take several hours for the job to process all customers. Also errors can occur while extracting and processing the customer information. In the case of error, the job tries to continue processing the remaining customers. More detailed information can be checked in the logs and on the exported information.

### Reporting Portal
Use a web browser and navigate to the homepage address of the Portal website installation.

You can use the Portal to:
 * List campaigns
 * Manage campaigns associated customers
 * Create a new campaign
 * Delete a campaign

Note: To be able to associate customers to campaigns, first you must guarantee that a run of the Reporting Job (with the database parameter) as completed with success. If not, the customer list will be empty or only partially complete.

### Power BI Reports
(to be completed)


Final disclaimer: The samples are provided freely and as a proof of concept with no support whatsoever.
http://www.create.pt




