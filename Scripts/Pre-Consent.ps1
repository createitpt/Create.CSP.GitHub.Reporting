# Replace this with the AppId of the Application you want to enable PreConsent
$appId = "<replace>"

# Start Azure AD PowerShell session
Connect-MsolService

# Fetch your TenantId for querying Graph later
$tenantId = (Get-MsolCompanyInformation).ObjectId.toString()

# Generate a random guid string
$random = [Guid]::NewGuid().toString()

# Create a service principal using the random string as DisplayName and Password
$servicePrincipal = New-MsolServicePrincipal -DisplayName $random -Type Password -Value $random

# Assign service principal to Tenant Admin role
Add-MsolRoleMember -RoleName "Company Administrator" -RoleMemberType ServicePrincipal -RoleMemberObjectId ($servicePrincipal.ObjectId)

# Sleep for 30 seconds
Start-Sleep -s 30

# Construct params for auth request
$authParams = @{grant_type='client_credentials'; client_id=($servicePrincipal.AppPrincipalId); client_secret=$random; resource="https://graph.windows.net/"}

# Request an auth token for the service principal from Azure AD Token endpoint
$authResponse = Invoke-RestMethod -Method POST -Uri ("https://login.microsoftonline.com/{0}/oauth2/token" -f $tenantId) -ContentType "application/x-www-form-urlencoded" -body $authParams

# Extract access token from auth response
$bearerToken = $authResponse.access_token

# Make a Graph query to search for the Application object by appId
$graphResponse = Invoke-RestMethod -Method GET -Uri ("https://graph.windows.net/{0}/applications?api-version=1.6&`$filter=appId eq `'{1}`'" -f $tenantId, $appId) -ContentType "application/json" -Headers @{"Authorization" = ($authResponse.access_token)}

# Get Application's ObjectId
$appObjectId = $graphResponse.value.ObjectId

# Make a Graph query to enable Pre-Consent on the Application object
$graphResponse = Invoke-RestMethod -Method PATCH -Uri ("https://graph.windows.net/{0}/applications/{1}?api-version=1.6" -f $tenantId, $appObjectId) -ContentType "application/Json" -Headers @{"Authorization" = ($authResponse.access_token)} -Body '{"recordConsentConditions":"SilentConsentForPartnerManagedApp"}'

# Delete servicePrincipal object
$servicePrincipal | Remove-MsolServicePrincipal