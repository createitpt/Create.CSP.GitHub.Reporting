# Create.CSP.Reporting
Create IT Microsoft Cloud Solution Provider (CSP) Reporting Samples

This is a CSP Reporting sample application, intended for Microsoft CSP Partners, that provides Customer Insights (CI) and Opportunities Identification (OI) for Office 365 (seat based licensing). 

The analysis made by the sample, complements and details the information that is available in the CSP Partner Center Portal, and also shows how with a basic set of rules the CSP customer basis can be segmented and classified in terms of:
* Activation Opportunities - Seat related opportunities (e.g. A customer purchased 10 E3 seats but only 5 of them are assigned)
* Usage Opportunities - Last 30 days' service usage opportunities (e.g. From the 5 assigned E3 seats, 2 licenses have not used Skype for Business in the last 30 days)

(At the end of this document, you can find more detail about the opportunity types that are identified.)

Supplied with this information, the CSP Partner can for example determine and prioritize the customers that should be targeted by a Marketing Campaign, and/or work with their Sales and Support teams, to get these customers to make a better use of their purchased licenses and services.

The sample is composed by:
 * A Reporting Job - Extracts and Analysis the customer information, that results in the Opportunities Identification. Can output information to CSV files or to a database, allowing in the case of the database to store and compare customer historic information.
 * Power BI Reports (optional) - Provides reporting samples that present the extracted and analyzed information
 * A Reporting Portal (optional) - Provides a basic mechanism for managing the customers that are targeted by a campaign, allowing the filtering of the report to the campaign customer base. The actual management and sending of these campaigns are not in the scope of these samples.
 
 
# Requirements

### Minimum
The minimum installation corresponds to the deployment of the Reporting Job, supporting only the output of information to CSV files. For this you will need:
* .NET Framework 4.6.1
* Internet Access

You can also use this configuration if you just want to check/test these samples.

### Recommended
The recommended configuration corresponds to the deployment of the Reporting Job and the Power BI Reports. Optionally, you can also deploy the Reporting Portal if you think that filtering the customers by associated campaign is going to be useful for your scenario. For this you will need:
* Reporting Job:
  * .NET Framework 4.6.1
  * SQL Server 2014 Database or SQL Azure V12 Database
  * Internet Access
  * Note: Can also run as an Azure Web Job
* Power BI Reports:
  * Power BI Desktop application 
  * (optional) Office 365 Power BI enabled tenant, if you wish to publish the reports, and make sharing easier
* (optional) Reporting Portal
  * .NET Framework 4.6.1
  * IIS Web Server
  * Note: Can also run as an Azure Web App

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
The Reporting Job and the Reporting Portal both need a database connection to work. If you choose to not use a database, you can still use the job to export the reporting information to CSV files.

| Configuration Key  | Description |
| ------------- | ------------- |
| CSPDatabaseModelEntities connection string | The connection string to the Reporting Database.  |
| Logging connection string | The connection string to the Reporting Database. Typically, it refers to the same database as above.  |

# Usage
### Reporting Job
The Reporting Job is a console application that accepts a single parameter. That parameter can be:

| Parameter  | Description |
| ------------- | ------------- |
| activationCSV | Extracts and generates the Activation Opportunities report to a CSV file.  |
| customerUsageCSV | Extracts and generates the Usage Opportunities report to a CSV file.  |
| activationBD | Extracts and generates the Activation Opportunities report to the Reporting database.  |

The generated CSV files have a header line, all fields are separated by the tab (\t) character, and there is no text qualifier surrounding fields.

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
To work with the Power BI reports you will need to install and use the Power BI Desktop Application, available at: https://powerbi.microsoft.com/en-us/documentation/powerbi-desktop-get-the-desktop/#to-download-and-install-power-bi-desktop

The Power BI Report files are named:
 * CSP_CSV_Reports.pbix - For the CSV generated analysis
 * CSP_Database_Reports.pbix - For the Database generated analysis

#### Configuring / Updating the reports data sources

##### CSP CSV files
To configure the report, you must first run the Reporting Job and generate the Activation and the Usage CSV files.
Afterwards, follow these steps on Power BI Desktop:

1. Open the report file
2. On the ribbon, "Home" tab, click "Edit Queries" <br/>
<img src="https://github.com/createitpt/Create.CSP.GitHub.Reporting/blob/master/WikiImages/PowerBI_EditQueries.png" alt="PowerBI_EditQueries" width="60%" />
3. On left side, select the "Customers" table <br/>
<img src="https://github.com/createitpt/Create.CSP.GitHub.Reporting/blob/master/WikiImages/PowerBI_SelectTable.png" alt="PowerBI_SelectTable" width="10%" />
4. On the right side, click on the “Source” icon <br/>
<img src="https://github.com/createitpt/Create.CSP.GitHub.Reporting/blob/master/WikiImages/PowerBI_ChangeSource.png" alt="PowerBI_ChangeSource" width="10%" />
5. Select a new Path to the appropriate *.csv report (note the settings presented in the table and on the picture) 

| Table  | CSV File to choose |
| ------------- | ------------- |
| Customers | Usage |
| Subscriptions | Usage |
| Skus | Activation |
<img src="https://github.com/createitpt/Create.CSP.GitHub.Reporting/blob/master/WikiImages/PowerBI_BrowseCSV.png" alt="PowerBI_BrowseCSV" width="60%" />
6. Repeat the last three steps and update the Subscriptions and Skus tables also

##### CSP Reporting Database
To configure the report, follow these steps on Power BI Desktop:

1. Open the report file
2. On the ribbon, "Home" tab, click "Edit Queries" <br/>
<img src="https://github.com/createitpt/Create.CSP.GitHub.Reporting/blob/master/WikiImages/PowerBI_EditQueries.png" alt="PowerBI_EditQueries" width="60%" />
3. On left side, select the "ActivationReport" table <br/>
<img src="https://github.com/createitpt/Create.CSP.GitHub.Reporting/blob/master/WikiImages/PowerBI_DB_SelectTables.png" alt="PowerBI_SelectTable" width="10%" />
4. On the right side, click on the “Source” icon <br/>
<img src="https://github.com/createitpt/Create.CSP.GitHub.Reporting/blob/master/WikiImages/PowerBI_DB_ChangeSource.png" alt="PowerBI_ChangeSource" width="10%" />
5. Complete with your SQL Server database connection information <br/>
<img src="https://github.com/createitpt/Create.CSP.GitHub.Reporting/blob/master/WikiImages/PowerBI_DB_ConfigureServer.png" alt="PowerBI_DB_ConfigureServer" width="30%" />
6. Repeat the last three steps and update all the other tables also
7. Complete with your server credentials <br/>
<img src="https://github.com/createitpt/Create.CSP.GitHub.Reporting/blob/master/WikiImages/PowerBI_DB_ServerCredentials.png" alt="PowerBI_DB_ServerCredentials" width="30%" /> <br/>
<img src="https://github.com/createitpt/Create.CSP.GitHub.Reporting/blob/master/WikiImages/PowerBI_DB_ServerCredentials2.png" alt="PowerBI_DB_ServerCredentials" width="30%" /> 
8. If necessary, update your data privacy configuration <br/>
<img src="https://github.com/createitpt/Create.CSP.GitHub.Reporting/blob/master/WikiImages/PowerBI_DB_DataPrivacy.png" alt="PowerBI_DB_DataPrivacy" width="30%" /> <br/>
<img src="https://github.com/createitpt/Create.CSP.GitHub.Reporting/blob/master/WikiImages/PowerBI_DB_DataPrivacy2.png" alt="PowerBI_DB_DataPrivacy" width="30%" /> 

NOTE: The Power BI reports can be published to an Office 365 tenant with Power BI enabled, for easier sharing within a team.

# Opportunity types
The opportunities are classified by an Action Type and a Sub-Type detail. The detail is explanatory, indicating the reason for that classification.

### Activation Opportunities
The following types of opportunities are identified:

| Action Type  | Action Sub Type |
| ------------- | ------------- |
| NO ACTION NEEDED | <ul><li>Customer is deleted or without relationship.</li><li>Waiting for subscription life cycle to de-provision subscription</li><li>All active licenses are assigned</li></ul>  |
| ACTION NEEDED | <ul><li>All licenses assigned. Some licenses are about to expire</li><li>Customer has more users with licenses than licenses available</li></ul> |
| ACTIVATION OPPORTUNITY |  <ul><li>Customer relationship to partner is different from reseller</li><li>Customer does not have any subscribed SKUs</li><li>Customer does not have any subscriptions</li><li>No seats active, assigned, about to expire or disabled</li><li>No seats assigned yet, all licenses are about to expire</li><li>Not a CSP offer</li><li>SKU capability status not enabled</li><li>No seats assigned yet</li><li>No seats assigned yet, some licenses are about to expire</li><li>Not all seats have been assigned yet</li></ul> |
| Scenario not defined | If this occurs to you, please contact us :) |

### Usage Opportunities
The usage is analyzed taking into consideration the user's activity in the last 30 days.
Currently, only usage referring to the following Office 365 services are supported:
* Exchange Online
* Skype for Business Online
* SharePoint Online

The following types of opportunities are identified:

| Action Type  | Action Sub Type |
| ------------- | ------------- |
| NO ACTION NEEDED | <ul><li>Customer is deleted or without relationship</li><li>All users are active on the service</li></ul>  |
| ACTIVATION OPPORTUNITY |  <ul><li>Customer relationship to partner different from reseller</li><li>Customer does not have any subscribed SKUs</li><li>Customer does not have any subscriptions</li><li>Subscription state: [state detail]</li><li>Not all users are active on the service</li></ul> |
| Not supported | <ul><li>Subscription Offer does not map to any CSP Subscribed SKU Product</li><li>Service Plan information extraction not supported</li></ul> |
| Scenario not defined | If this occurs to you, please contact us :) |

Final disclaimer: These samples are provided freely as a proof of concept as-is, with no support whatsoever.

http://www.create.pt




