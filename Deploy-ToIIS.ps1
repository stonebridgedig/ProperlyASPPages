# Automated Publish to IIS Server
# This script publishes the application to the IIS server at 10.0.0.102

param(
    [string]$ServerIP = "10.0.0.102",
    [string]$Username = "Administrator",
    [string]$SiteName = "Default Web Site/ProperlyASPPages",
    [switch]$PromptForPassword = $false
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Publishing to IIS Server" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$projectPath = "$PSScriptRoot\ProperlyASPPages\ProperlyASPPages.csproj"
$publishProfile = "IIS-Local"

# Check if project exists
if (-not (Test-Path $projectPath)) {
    Write-Error "Project file not found at: $projectPath"
    exit 1
}

# Build the MSBuild arguments
$msbuildArgs = @(
    $projectPath,
    "/p:DeployOnBuild=true",
    "/p:PublishProfile=$publishProfile",
    "/p:Configuration=Release",
    "/p:MSDeployServiceURL=$ServerIP",
    "/p:DeployIisAppPath=`"$SiteName`"",
    "/p:Username=$Username"
)

# Add password if prompted
if ($PromptForPassword) {
    $securePassword = Read-Host "Enter password for $Username" -AsSecureString
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $password = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
    $msbuildArgs += "/p:Password=$password"
    Write-Host ""
}

try {
    Write-Host "Building and publishing application..." -ForegroundColor Yellow
    Write-Host "Target: $ServerIP" -ForegroundColor Gray
    Write-Host "Site: $SiteName" -ForegroundColor Gray
    Write-Host ""
    
    # Execute dotnet publish with MSBuild parameters
    & dotnet publish $msbuildArgs
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Green
        Write-Host "Publish Successful!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
        Write-Host "Application deployed to: http://$ServerIP" -ForegroundColor Green
    } else {
        Write-Error "Publish failed with exit code: $LASTEXITCODE"
    }
} catch {
    Write-Error "An error occurred during publish: $_"
    exit 1
}
